using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	public class AdminHelper {
		AppModule module;

		public AdminHelper(AppModule module) {
			this.module = module;
		}

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
					// TODO					_settings = Database.QueryOne<Settings>("SELECT * FROM Settings");
					module.Batch.Status = module.Message = "Database restored successfully";
				});
			}
		}

	}

	public class AdminModule : AppModule {

		public AjaxReturn BatchStatus(int id) {
			return new AdminHelper(this).BatchStatus(id);
		}

		public void Backup() {
			new AdminHelper(this).Backup();
		}

		public void Restore() {
			new AdminHelper(this).Restore();
		}
	}
}
