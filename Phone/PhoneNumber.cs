using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeFirstWebFramework;

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

	public class DefaultModule : AppModule {

		public DefaultModule() {
			Menu = new MenuOption[] {
				new MenuOption("Phone Numbers", "/default/Default"),
				new MenuOption("Analysis", "/default/AnalysisList"),
				new MenuOption("Cost centres", "/default/CostCentreList")
			};
		}

		public override void Default() {
			insertMenuOption(new MenuOption("New number", "/default/PhoneNumber?id=0"));
			Form = new DataTableForm(this, typeof(PhoneNumber));
			Form.Options["select"] = "/default/PhoneNumber";
			Form.Show();
		}

		public object DefaultListing() {
			return Database.Query("SELECT * FROM PhoneNumber ORDER BY PhoneKey");
		}

		public void PhoneNumber(int id) {
			PhoneNumber n = Database.Get<PhoneNumber>(id);
			Form = new Form(this, typeof(PhoneNumber));
			Form.Options["canDelete"] = n.PhoneNumberId != null;
			Form.Data = n.ToJToken();
			Form.Show();
		}

		public AjaxReturn PhoneNumberPost(PhoneNumber json) {
			json.PhoneKey = Regex.Replace(json.Number, "^[0-9]", "");
			return PostRecord(json);
		}

		public AjaxReturn PhoneNumberDelete(int id) {
			return DeleteRecord("PhoneNumber", id);
		}

		public void AnalysisList() {
			insertMenuOption(new MenuOption("New analysis", "/default/Analysis?id=0"));
			Form = new DataTableForm(this, typeof(Analysis));
			Form.Options["select"] = "/default/Analysis";
			Form.Show();
		}

		public object AnalysisListListing() {
			return Database.Query("SELECT * FROM Analysis ORDER BY AnalysisName");
		}

		public void Analysis(int id) {
			Analysis a = Database.Get<Analysis>(id);
			Form = new Form(this, typeof(Analysis));
			Form.Options["canDelete"] = a.AnalysisId != null && Database.QueryOne("SELECT Analysis FROM PhoneNumber WHERE Analysis = " + id) == null;
			Form.Data = a.ToJToken();
			Form.Show();
		}

		public AjaxReturn AnalysisPost(Analysis json) {
			Utils.Check(json.CostCentre > 0, "Cost Centre required");
			return PostRecord(json);
		}

		public AjaxReturn AnalysisDelete(int id) {
			Utils.Check(Database.QueryOne("SELECT Analysis FROM PhoneNumber WHERE Analysis = " + id) == null, "Cannot delete analysis code - in use");
			return DeleteRecord("Analysis", id);
		}

		public void CostCentreList() {
			insertMenuOption(new MenuOption("New Cost Centre", "/default/CostCentre?id=0"));
			Form = new DataTableForm(this, typeof(CostCentre));
			Form.Options["select"] = "/default/CostCentre";
			Form.Show();
		}

		public object CostCentreListListing() {
			return Database.Query("SELECT * FROM CostCentre ORDER BY CostCentreName");
		}

		public void CostCentre(int id) {
			CostCentre a = Database.Get<CostCentre>(id);
			Form = new Form(this, typeof(CostCentre));
			Form.Options["canDelete"] = a.CostCentreId != null && Database.QueryOne("SELECT CostCentre FROM Analysis WHERE CostCentre = " + id) == null;
			Form.Data = a.ToJToken();
			Form.Show();
		}

		public AjaxReturn CostCentrePost(CostCentre json) {
			return PostRecord(json);
		}

		public AjaxReturn CostCentreDelete(int id) {
			Utils.Check(Database.QueryOne("SELECT CostCentre FROM Analysis WHERE CostCentre = " + id) == null, "Cannot delete Cost Centre code - in use");
			return DeleteRecord("CostCentre", id);
		}
	}
}
