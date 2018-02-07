using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using CodeFirstWebFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Phone {
	[Table]
	public class CostCentre : JsonObject {
		[Primary]
		public int? CostCentreId;
		public string CostCentreName;
	}

	[Table]
	public class Analysis : JsonObject {
		[Primary]
		public int? AnalysisId;
		public string AnalysisName;
		[ForeignKey("CostCentre")]
		public int CostCentre;
	}

	[Table]
	public class PhoneNumber : JsonObject {
		[Primary]
		public int? PhoneNumberId;
		[Unique("PhoneKey")]
		[Field(Visible = false)]
		public string PhoneKey;
		public string Number;
		[Length(0)]
		public string Name;
		[ForeignKey("Analysis")]
		public int Analysis;
	}

	[Auth(Name = "Phone Numbers")]
	public class HomeModule : AppModule {

		protected override void Init() {
			InsertMenuOptions(
				new MenuOption("Phone Numbers", "/home/Default"),
				new MenuOption("Analysis", "/home/AnalysisList"),
				new MenuOption("Cost centres", "/home/CostCentreList"),
				new MenuOption("Import", "/home/Import")
			);
		}

		public override void Default() {
			InsertMenuOption(new MenuOption("New number", "/home/PhoneNumber?id=0"));
			DataTableForm form = new DataTableForm(this, typeof(PhoneNumber)) {
				Select = "/home/PhoneNumber"
			};
			form.Show();
		}

		public object DefaultListing() {
			return Database.Query("SELECT * FROM PhoneNumber ORDER BY PhoneKey");
		}

		[Auth(AccessLevel.ReadOnly)]
		public void PhoneNumber(int id) {
			PhoneNumber n = Database.Get<PhoneNumber>(id);
			Form form = new Form(this, typeof(PhoneNumber)) {
				CanDelete = n.PhoneNumberId != null,
				Data = n.ToJToken()
			};
			form.Show();
		}

		[Auth(AccessLevel.ReadWrite)]
		public AjaxReturn PhoneNumberSave(PhoneNumber json) {
			json.PhoneKey = Regex.Replace(json.Number, "^[0-9]", "");
			return SaveRecord(json);
		}

		[Auth(AccessLevel.ReadWrite)]
		public AjaxReturn PhoneNumberDelete(int id) {
			return DeleteRecord("PhoneNumber", id);
		}

		public void AnalysisList() {
			InsertMenuOption(new MenuOption("New analysis", "/home/Analysis?id=0&from=/home/analysislist"));
			DataTableForm form = new DataTableForm(this, typeof(Analysis)) {
				Select = "/home/Analysis"
			};
			form.Show();
		}

		public object AnalysisListListing() {
			return Database.Query("SELECT * FROM Analysis ORDER BY AnalysisName");
		}

		[Auth(AccessLevel.ReadWrite, Name = "Update Analysis Codes")]
		public void Analysis(int id) {
			Analysis a = Database.Get<Analysis>(id);
			Form form = new Form(this, typeof(Analysis)) {
				CanDelete = a.AnalysisId != null && Database.QueryOne("SELECT Analysis FROM PhoneNumber WHERE Analysis = " + id) == null,
				Data = a.ToJToken()
			};
			form.Show();
		}

		public AjaxReturn AnalysisSave(Analysis json) {
			Utils.Check(json.CostCentre > 0, "Cost Centre required");
			return SaveRecord(json);
		}

		public AjaxReturn AnalysisDelete(int id) {
			Utils.Check(Database.QueryOne("SELECT Analysis FROM PhoneNumber WHERE Analysis = " + id) == null, "Cannot delete analysis code - in use");
			return DeleteRecord("Analysis", id);
		}

		public void CostCentreList() {
			InsertMenuOption(new MenuOption("New Cost Centre", "/home/CostCentre?id=0&from=/home/costcentrelist"));
			DataTableForm form = new DataTableForm(this, typeof(CostCentre)) {
				Select = "/home/CostCentre"
			};
			form.Show();
		}

		public object CostCentreListListing() {
			return Database.Query("SELECT * FROM CostCentre ORDER BY CostCentreName");
		}

		[Auth(AccessLevel.ReadWrite, Name = "Update Cost Centres")]
		public void CostCentre(int id) {
			CostCentre a = Database.Get<CostCentre>(id);
			Form form = new Form(this, typeof(CostCentre)) {
				CanDelete = a.CostCentreId != null && Database.QueryOne("SELECT CostCentre FROM Analysis WHERE CostCentre = " + id) == null,
				Data = a.ToJToken()
			};
			form.Show();
		}

		public AjaxReturn CostCentreSave(CostCentre json) {
			return SaveRecord(json);
		}

		public AjaxReturn CostCentreDelete(int id) {
			Utils.Check(Database.QueryOne("SELECT CostCentre FROM Analysis WHERE CostCentre = " + id) == null, "Cannot delete Cost Centre code - in use");
			return DeleteRecord("CostCentre", id);
		}

		/// <summary>
		/// Import vcard file. Submit button calls ImportFile.
		/// </summary>
		public void Import() {
			DumbForm f = new DumbForm(this, true) {
				Data = new JObject().AddRange("analysis", 1, "prefix", "+44")
			};
			f.Add(new FieldAttribute() {
				Data = "file",
				Heading = "File to import",
				Type = "file"
			});
			FieldAttribute fld = new FieldAttribute() {
				Data = "analysis",
				Heading = "Analysis"
			};
			fld.MakeSelectable(Database.Query("SELECT AnalysisId as id, AnalysisName AS value FROM Analysis ORDER BY AnalysisName"));
			f.Add(fld);
			f.Add(new FieldAttribute() {
				Data = "prefix",
				Heading = "International prefix to remove",
				Type = "textInput"
			});
			f.Show();
		}

		/// <summary>
		/// User has submitted a vcard file
		/// </summary>
		public void ImportSave(UploadedFile file, int analysis, string prefix) {
			Utils.Check(Database.Get("Analysis", analysis) != null, "You must choose an analysis code");
			Method = "import";		// Show import.tmpl again
			new BatchJob(this, delegate () {
				int lineNo = 0;
				try {
					string name = "";
					string[] lines = file.Content.Split('\n');
					Batch.Records = lines.Length;
					foreach (string l in lines) {
						Batch.Record = lineNo++;
						string line = l;
						string tag = Utils.NextToken(ref line, ":");
						line = line.Replace("\r", "").Replace("\\n", "\r\n");
						switch (tag) {
							case "BEGIN":
							case "END":
								name = "";
								break;
							case "FN":
								name = line;
								break;
							default:
								if (tag.StartsWith("TEL")) {
									Utils.NextToken(ref tag, ";");
									line = line.Trim();
									if (!string.IsNullOrEmpty(prefix) && line.StartsWith(prefix))
										line = "0" + line.Substring(prefix.Length).Trim();
									string key = Regex.Replace(line, "^[0-9]", "");
									if (key == "")
										break;
									PhoneNumber p = Database.Get(new PhoneNumber() {
										PhoneKey = key
									});
									if (p.PhoneNumberId > 0) {
										Batch.Status = string.Format("Existing number:{0} {1}", name, line);
									} else {
										Batch.Status = string.Format("New number:{0} {1}", name, line);
										p.Name = name + " " + tag;
										p.Number = line;
										p.PhoneKey = key;
										p.Analysis = analysis;
										Database.Insert(p);
									}
								}
								break;
						}
					}
				} catch (Exception ex) {
					throw new CheckException(ex, "{0}:Error:{1}\r\n", lineNo, ex.Message);
				}
			});
		}

	}

	public class AdminModule : CodeFirstWebFramework.AdminModule {

	}

}