using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	/// <summary>
	/// Class to provide Admin functions - called from Admin AppModule.
	/// </summary>
	public class AdminHelper {
		AppModule module;

		/// <summary>
		/// Create AdminHelper for supplied module
		/// </summary>
		public AdminHelper(AppModule module) {
			this.module = module;
		}

		public void EditSettings() {
			Form form = new Form(module, typeof(Settings));
			form.Data = module.Settings.ToJToken();
			DirectoryInfo skinFolder = module.Server.DirectoryInfo("skin");
			form.Replace(form.IndexOf("Skin"), new SelectAttribute(skinFolder.EnumerateFiles("*.css")
						.Where(f => File.Exists(Path.ChangeExtension(f.FullName, ".js")))
						.Select(f => new JObject().AddRange("value", Path.GetFileNameWithoutExtension(f.Name))), true) { Data = "Skin" });
			form.Add(new FieldAttribute() {
				Data = "AppVersion",
				Type = "string"
			});
			form.Show();
		}

		public AjaxReturn EditSettingsPost(JObject json) {
			module.Database.BeginTransaction();
			json["idSettings"] = 1;
			module.Database.Update("Settings", json);
			module.ReloadSettings();
			module.Database.Commit();
			return new AjaxReturn() { message = "Settings saved", redirect = "/Admin" };
		}

		/// <summary>
		/// Return the status of the given batch job.
		/// </summary>
		public AjaxReturn BatchStatus(int id) {
			AjaxReturn result = new AjaxReturn();
			AppModule.BatchJob batch = AppModule.GetBatchJob(id);
			if (batch == null) {
				module.Log("Invalid batch id");
				result.error = "Invalid batch id";
			} else {
				if (batch == null) {
					module.Log("Invalid batch id");
					result.error = "Invalid batch id";
				} else {
					module.Log("Batch {0}:{1}%:{2}", batch.Id, batch.PercentComplete, batch.Status);
					result.data = batch;
					if (batch.Finished) {
						result.error = batch.Error;
						result.redirect = batch.Redirect;
						module.Log("Batch finished - redirecting to {0}", batch.Redirect);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Backup the database
		/// </summary>
		public void Backup() {
			module.Database.Logging = LogLevel.None;
			module.Database.BeginTransaction();
			DateTime now = Utils.Now;
			JObject result = new JObject().AddRange("BackupDate", now.ToString("yyyy-MM-dd HH:mm:ss"));
			foreach (string name in module.Database.TableNames) {
				result.Add(name, module.Database.Query("SELECT * FROM " + name));
			}
			module.Response.AddHeader("Content-disposition", "attachment; filename=Backup-" + now.ToString("yyyy-MM-dd-HH-mm-ss") + ".json");
			module.WriteResponse(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented), "application/json", System.Net.HttpStatusCode.OK);
		}

		/// <summary>
		/// Restore the database
		/// </summary>
		public void Restore() {
			if (module.PostParameters != null && module.PostParameters["file"] != null) {
				new AppModule.BatchJob(module, delegate () {
					module.Batch.Status = "Loading new data";
					UploadedFile data = module.PostParameters.As<UploadedFile>("file");
					module.Database.Logging = LogLevel.None;
					module.Database.BeginTransaction();
					JObject d = data.Content.JsonTo<JObject>();
					List<Table> tables = module.Database.TableNames.Select(n => module.Database.TableFor(n)).ToList();
					module.Batch.Records = tables.Count * 4;
					foreach (Table t in tables) {
						if (d[t.Name] != null) {
							module.Batch.Records += ((JArray)d[t.Name]).Count;
						}
					}
					module.Batch.Status = "Deleting existing data";
					TableList orderedTables = new TableList(tables);
					foreach (Table t in orderedTables) {
						module.Database.Execute("DELETE FROM " + t.Name);
						module.Batch.Record += 4;
					}
					module.Database.Logging = LogLevel.None;
					foreach (Table t in orderedTables.Reverse<Table>()) {
						if (d[t.Name] != null) {
							module.Batch.Status = "Restoring " + t.Name + " data";
							foreach (JObject record in (JArray)d[t.Name]) {
								module.Database.Insert(t.Name, record);
								module.Batch.Record++;
							}
						}
					}
					module.Batch.Status = "Checking database version";
					module.Database.Upgrade();
					module.Database.Commit();
					module.Batch.Status = "Compacting database";
					module.Database.Clean();
					module.ReloadSettings();
					module.Batch.Status = module.Message = "Database restored successfully";
				});
			}
		}

		public void Users() {
			Table t = module.Database.TableFor("User");
			DataTableForm f = new DataTableForm(module, t.Type) {
				Select = "/admin/edituser.html"
			};
			f.Remove("Password");
			AccessLevel levels = module.Server.NamespaceDef.GetAccessLevel();
			SelectAttribute level = new SelectAttribute(levels.Select(), true) {
				Data = "AccessLevel",
				Type = "select"
			};
			f.Replace(f.IndexOf("AccessLevel"), level);
			f.Show();
		}

		public JObjectEnumerable UsersListing() {
			return module.Database.Query("SELECT * FROM User ORDER BY Login");
		}

		public void EditUser(int id) {
			User user = module.Database.Get<User>(id);
			user.Password = "";
			HeaderDetailForm f = new HeaderDetailForm(module, user.GetType(), typeof(Permission));
			AccessLevel levels = module.Server.NamespaceDef.GetAccessLevel();
			SelectAttribute level = new SelectAttribute(levels.Select(), true) {
				Data = "AccessLevel"
			};
			f.Header.Replace(f.Header.IndexOf("AccessLevel"), level);
			f.Detail.Replace(f.Detail.IndexOf("AccessLevel"), level);
			f.Options["canDelete"] = id > 1 || (id == 1 && module.Database.QueryOne("SELECT idUser FROM User where idUser > 1") == null);
			if (id == 1 || module.Database.QueryOne("SELECT idUser FROM User") == null) {
				// This has to be the admin user
				user.AccessLevel = AccessLevel.Admin;
				user.ModulePermissions = false;
				f.Header["AccessLevel"].Type = "select";
				f.Header.Remove("ModulePermissions");
			}
			f.Data = new JObject().AddRange(
				"header", user,
				"detail", permissions(id)
				);
			module.Form = f;
		}

		public IEnumerable<Permission> permissions(int user) {
			foreach (ModuleInfo m in module.Server.NamespaceDef.Modules) {
				int lowest = m.LowestAccessLevel;
				if (m.ModuleAccessLevel == AccessLevel.Any && lowest == AccessLevel.Any)
					continue;
				Permission p = new Permission() {
					UserId = user,
					Module = m.Name,
					Method = "-",
					AccessLevel = AccessLevel.Unspecified
				};
				Permission r = module.Database.Get(p);
				if (r.UserId == user)
					p = r;
				p.MinAccessLevel = m.ModuleAccessLevel;
				if (lowest > AccessLevel.Any)
					p.MinAccessLevel = Math.Min(m.ModuleAccessLevel, lowest);
				yield return p;
				foreach (string method in m.AuthMethods.Keys) {
					p = new Permission() {
						UserId = user,
						Module = m.Name,
						Method = method,
						AccessLevel = AccessLevel.Unspecified
					};
					r = module.Database.Get(p);
					if (r.UserId == user)
						p = r;
					p.MinAccessLevel = m.AuthMethods[method];
					if(p.MinAccessLevel != AccessLevel.Any)
						yield return p;
				}
			}
		}

		public AjaxReturn EditUserPost(JObject json) {
			Table t = module.Database.TableFor("User");
			User user = (User)((JObject)json["header"]).To(t.Type);
			bool passwordChanged = false;
			if (user.idUser > 0) {
				// Existing record
				User u = module.Database.Get<User>((int)user.idUser);
				Utils.Check(user.idUser == u.idUser, "Invalid EditUser post");
				if (string.IsNullOrEmpty(user.Password)) {
					user.Password = u.Password;
				} else {
					passwordChanged = true;
				}
			} else {
				passwordChanged = true;
				if (module.Database.QueryOne("SELECT idUser FROM User") == null) {
					user.idUser = 1;        // Admin user
					user.AccessLevel = AccessLevel.Admin;
					user.ModulePermissions = false;
				}
			}
			if (passwordChanged) {
				// New record
				string error = user.PasswordValid();
				if (error != null)
					throw new CheckException(error);
				user.Password = user.HashPassword(user.Password);
			}
			module.Database.BeginTransaction();
			AjaxReturn r = module.PostRecord(user);
			if (!string.IsNullOrEmpty(r.error))
				return r;
			module.Database.Execute("DELETE FROM Permission WHERE UserId = " + user.idUser);
			t = module.Database.TableFor("Permission");
			foreach (JObject p in ((JArray)json["detail"])) {
				p["UserId"] = user.idUser;
				if (user.idUser == 1)
					p["AccessLevel"] = AccessLevel.Admin;
				module.Database.Insert("Permission", p);
			}
			module.Database.Commit();
			return r;
		}

		public AjaxReturn DeleteUserPost(int id) {
			module.Database.BeginTransaction();
			User user = module.Database.Get<User>(id);
			Utils.Check(user.idUser > 1, "Cannot delete this user");
			module.Database.Execute("DELETE FROM Permissions WHERE UserId = " + id);
			module.Database.Execute("DELETE FROM User WHERE iduser = " + id);
			module.Database.Commit();
			return new AjaxReturn() { message = "User deleted" };
		}

		public void Login() {
			module.Session.User = null;
			if (module.Request.HttpMethod == "POST") {
				string login = module.Parameters.AsString("login");
				string password = module.Parameters.AsString("password");
				User user = module.Database.QueryOne<User>("SELECT * FROM User WHERE Login = "
					+ module.Database.Quote(login) + " OR Email = " + module.Database.Quote(login));
				if (user.idUser > 0) {
					if (user.HashPassword(password) == user.Password) {
						module.Session.User = user;
						module.Message = "Logged in successfully";
						string redirect = module.SessionData.redirect;
						module.Session.Object.Remove("redirect");
						if (string.IsNullOrEmpty(redirect))
							redirect = "/home";
						module.Redirect(redirect);
						return;
					}
				}
				module.Message = "Login name or not found or password invalid";
			}
		}
	}

	/// <summary>
	/// Admin module - provides BatchStatus, Backup and Restore. Uses AdminHelper for the implementation.
	/// </summary>
	[Auth(AccessLevel.Admin)]
	public class AdminModule : AppModule {

		protected override void Init() {
			insertMenuOptions(
				new MenuOption("Settings", "/admin/editsettings"),
				new MenuOption("Users", "/admin/users"),
				new MenuOption("Backup", "/admin/backup"),
				new MenuOption("Restore", "/admin/restore"),
				new MenuOption(Session.User == null ? "Login" : "Logout", "/admin/login")
			);
		}

		[Auth(AccessLevel.Any)]
		public override void Default() {
		}

		public void EditSettings() {
			new AdminHelper(this).EditSettings();
		}

		public AjaxReturn EditSettingsPost(JObject json) {
			return new AdminHelper(this).EditSettingsPost(json);
		}

		/// <summary>
		/// Return the status of the batch
		/// </summary>
		[Auth(AccessLevel.Any)]
		public AjaxReturn BatchStatus(int id) {
			return new AdminHelper(this).BatchStatus(id);
		}

		/// <summary>
		/// Backup the database
		/// </summary>
		public void Backup() {
			new AdminHelper(this).Backup();
		}

		/// <summary>
		/// Restore the database.
		/// </summary>
		public void Restore() {
			new AdminHelper(this).Restore();
		}

		public void Users() {
			insertMenuOption(new MenuOption("New User", "/admin/EditUser?id=0"));
			new AdminHelper(this).Users();
		}

		public JObjectEnumerable UsersListing() {
			return new AdminHelper(this).UsersListing();
		}

		public void EditUser(int id) {
			new AdminHelper(this).EditUser(id);
		}

		public AjaxReturn EditUserPost(JObject json) {
			return new AdminHelper(this).EditUserPost(json);
		}

		public AjaxReturn DeleteUserPost(int id) {
			return new AdminHelper(this).DeleteUserPost(id);
		}

		[Auth(AccessLevel.Any)]
		public void Login() {
			new AdminHelper(this).Login();
		}

		[Auth(AccessLevel.Any)]
		public void Logout() {
			new AdminHelper(this).Login();
		}
	}
}
