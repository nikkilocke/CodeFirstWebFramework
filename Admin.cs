using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	public class AdminModule : AppModule {

		public AdminModule() {
		}

		public AdminModule(AppModule module)
			: base(module) {
		}

		public AjaxReturn BatchStatus(int id) {
			AjaxReturn result = new AjaxReturn();
			BatchJob batch = AppModule.GetBatchJob(id);
			if (batch == null) {
				Log("Invalid batch id");
				result.error = "Invalid batch id";
			} else {
				if (batch == null) {
					Log("Invalid batch id");
					result.error = "Invalid batch id";
				} else {
					Log("Batch {0}:{1}%:{2}", batch.Id, batch.PercentComplete, batch.Status);
					result.data = batch;
					if (batch.Finished) {
						result.error = batch.Error;
						result.redirect = batch.Redirect;
						Log("Batch finished - redirecting to {0}", batch.Redirect);
					}
				}
			}
			return result;
		}

		public void Backup() {
			Database.Logging = LogLevel.None;
			Database.BeginTransaction();
			DateTime now = Utils.Now;
			JObject result = new JObject().AddRange("BackupDate", now.ToString("yyyy-MM-dd HH:mm:ss"));
			foreach (string name in Database.TableNames) {
				result.Add(name, Database.Query("SELECT * FROM " + name));
			}
			Response.AddHeader("Content-disposition", "attachment; filename=Backup-" + now.ToString("yyyy-MM-dd-HH-mm-ss") + ".json");
			WriteResponse(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented), "application/json", System.Net.HttpStatusCode.OK);
		}

		public void Restore() {
			if (PostParameters != null && PostParameters["file"] != null) {
				new BatchJob(this, delegate() {
					Batch.Status = "Loading new data";
					UploadedFile data = PostParameters.As<UploadedFile>("file");
					Database.Logging = LogLevel.None;
					Database.BeginTransaction();
					JObject d = data.Content.JsonTo<JObject>();
					List<Table> tables = Database.TableNames.Select(n => Database.TableFor(n)).ToList();
					Batch.Records = tables.Count * 4;
					foreach (Table t in tables) {
						if (d[t.Name] != null) {
							Batch.Records += ((JArray)d[t.Name]).Count;
						}
					}
					Batch.Status = "Deleting existing data";
					TableList orderedTables = new TableList(tables);
					foreach (Table t in orderedTables) {
						Database.Execute("DELETE FROM " + t.Name);
						Batch.Record += 4;
					}
					Database.Logging = LogLevel.None;
					foreach (Table t in orderedTables.Reverse<Table>()) {
						if (d[t.Name] != null) {
							Batch.Status = "Restoring " + t.Name + " data";
							foreach (JObject record in (JArray)d[t.Name]) {
								Database.Insert(t.Name, record);
								Batch.Record++;
							}
						}
					}
					Batch.Status = "Checking database version";
					Database.Upgrade();
					Database.Commit();
					Batch.Status = "Compacting database";
					Database.Clean();
					// TODO					_settings = Database.QueryOne<Settings>("SELECT * FROM Settings");
					Batch.Status = Message = "Database restored successfully";
				});
			}
		}

	}
}
