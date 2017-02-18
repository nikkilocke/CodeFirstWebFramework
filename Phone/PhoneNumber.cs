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

		public override void Default() {
			Menu = new MenuOption[] {
				new MenuOption("New number", "/default/PhoneNumber?id=0")
			};
			Form = new DataTableForm(this, typeof(PhoneNumber), false);
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
			Form.Options["data"] = n.ToJToken();
			Form.Show();
		}

		public AjaxReturn PhoneNumberPost(PhoneNumber json) {
			json.PhoneKey = Regex.Replace(json.Number, "^[0-9]", "");
			return PostRecord(json);
		}

		public AjaxReturn PhoneNumberDelete(int id) {
			return DeleteRecord("PhoneNumber", id);
		}
	}
}
