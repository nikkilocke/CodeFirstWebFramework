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

	public class HomeModule : AppModule {

		protected override void Init() {
			insertMenuOptions(
				new MenuOption("Phone Numbers", "/home/Default"),
				new MenuOption("Analysis", "/home/AnalysisList"),
				new MenuOption("Cost centres", "/home/CostCentreList")
			);
		}

		public override void Default() {
			insertMenuOption(new MenuOption("New number", "/home/PhoneNumber?id=0"));
			DataTableForm form = new DataTableForm(this, typeof(PhoneNumber));
			form.Select = "/home/PhoneNumber";
			form.Show();
		}

		public object DefaultListing() {
			return Database.Query("SELECT * FROM PhoneNumber ORDER BY PhoneKey");
		}

		[Auth(AccessLevel.ReadWrite)]
		public void PhoneNumber(int id) {
			PhoneNumber n = Database.Get<PhoneNumber>(id);
			Form form = new Form(this, typeof(PhoneNumber));
			form.Options["canDelete"] = n.PhoneNumberId != null;
			form.Data = n.ToJToken();
			form.Show();
		}

		public AjaxReturn PhoneNumberPost(PhoneNumber json) {
			json.PhoneKey = Regex.Replace(json.Number, "^[0-9]", "");
			return PostRecord(json);
		}

		public AjaxReturn PhoneNumberDelete(int id) {
			return DeleteRecord("PhoneNumber", id);
		}

		public void AnalysisList() {
			insertMenuOption(new MenuOption("New analysis", "/home/Analysis?id=0"));
			DataTableForm form = new DataTableForm(this, typeof(Analysis));
			form.Select = "/home/Analysis";
			form.Show();
		}

		public object AnalysisListListing() {
			return Database.Query("SELECT * FROM Analysis ORDER BY AnalysisName");
		}

		[Auth(AccessLevel.ReadWrite)]
		public void Analysis(int id) {
			Analysis a = Database.Get<Analysis>(id);
			Form form = new Form(this, typeof(Analysis));
			form.Options["canDelete"] = a.AnalysisId != null && Database.QueryOne("SELECT Analysis FROM PhoneNumber WHERE Analysis = " + id) == null;
			form.Data = a.ToJToken();
			form.Show();
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
			insertMenuOption(new MenuOption("New Cost Centre", "/home/CostCentre?id=0"));
			DataTableForm form = new DataTableForm(this, typeof(CostCentre));
			form.Select = "/home/CostCentre";
			form.Show();
		}

		public object CostCentreListListing() {
			return Database.Query("SELECT * FROM CostCentre ORDER BY CostCentreName");
		}

		[Auth(AccessLevel.ReadWrite)]
		public void CostCentre(int id) {
			CostCentre a = Database.Get<CostCentre>(id);
			Form form = new Form(this, typeof(CostCentre));
			form.Options["canDelete"] = a.CostCentreId != null && Database.QueryOne("SELECT CostCentre FROM Analysis WHERE CostCentre = " + id) == null;
			form.Data = a.ToJToken();
			form.Show();
		}

		public AjaxReturn CostCentrePost(CostCentre json) {
			return PostRecord(json);
		}

		public AjaxReturn CostCentreDelete(int id) {
			Utils.Check(Database.QueryOne("SELECT CostCentre FROM Analysis WHERE CostCentre = " + id) == null, "Cannot delete Cost Centre code - in use");
			return DeleteRecord("CostCentre", id);
		}
	}

	public class AdminModule : CodeFirstWebFramework.AdminModule {

	}

}