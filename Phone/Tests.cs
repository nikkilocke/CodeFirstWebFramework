using System;
using System.Collections.Generic;
using System.Text;
using CodeFirstWebFramework;
using Newtonsoft.Json.Linq;

namespace Phone {
	public enum TestValues {
		Value0,
		Value1,
		Value2,
		Value3,
		Value4
	}
	[Table]
	public class TestData : JsonObject {
		[Primary]
		public int? idTestData;
		public DateTime Date;
		public string Text;
		public decimal Decimal;
		public double Double;
		public int Integer;
		[Field("hint", "This is a test hint on a checkbox", 
			"preamble", "This is a preamble with a <a href=\"/\" target=\"_blank\">link</a>",
			"postamble", "This is a postamble with a <a href=\"/\" target=\"_blank\">link</a>")]
		public bool Boolean;
		[Field("hint", "This is a test hint on an Enum")]
		public TestValues Option;
        [Length(0)]
		[Field("hint", "This is a test hint")]
		public string TextArea;
		[Length(0)]
		[Field(Type = "inlineImageInput")]
		public string Image;
	}
	[Table]
	public class TestDetail : JsonObject {
		[Primary]
		public int? idTestDetail;
		public string Text;
	}
	public class Tests : AppModule {
		protected override void Init() {
			base.Init();
			InsertMenuOptions(
				new MenuOption("Test Form", "/Tests/TestForm"),
				new MenuOption("Credit/Radio Form", "/Tests/TestForm?credit=y"),
				new MenuOption("Header/Detail Form", "/Tests/HeaderDetailForm")
				);
		}

		public Form TestForm() {
			bool edit = GetParameters["edit"] == "y";
			if (!edit)
				InsertMenuOption(new MenuOption("Edit", Request.Url + (string.IsNullOrEmpty(Request.Url.Query) ?"?" : "&") + "edit=y"));
			Form form = new Form(this, typeof(TestData), edit);
			if(GetParameters["credit"] == "y") {
				form["Decimal"].Type = "creditInput";
				form["Decimal"].Heading = "Debit";
				form.Insert(form.IndexOf("Decimal") + 1, new FieldAttribute("name", "Debit", "data", "Decimal", "heading", "Debit", "type", "debitInput"));
				if (edit)
					form["Option"].Type = "radioInput";
            }
			form.Data = Database.QueryOne("SELECT * FROM TestData") ?? new JObject();
			return form;
		}

		public AjaxReturn TestFormSave(TestData json) {
			return SaveRecord(json);
		}

		public HeaderDetailForm HeaderDetailForm() {
			bool edit = GetParameters["edit"] == "y";
			if (!edit)
				InsertMenuOption(new MenuOption("Edit", Request.Url + (string.IsNullOrEmpty(Request.Url.Query) ? "?" : "&") + "edit=y"));
			Form header = new Form(this, typeof(TestData), edit);
			ListForm detail = new ListForm(this, typeof(TestDetail), edit);
			detail.Options["search"] = true;
			detail.Options["addRows"] = true;
			detail.Options["deleteRows"] = true;
			detail.Options["sortable"] = true;
			HeaderDetailForm form = new HeaderDetailForm(this, header, detail);
			form.Data = new JObject().AddRange(
				"header", Database.QueryOne("SELECT * FROM TestData") ?? new JObject(),
				"detail", Database.Query("SELECT * FROM TestDetail ORDER BY idTestDetail")
				);
			return form;
		}

		public AjaxReturn HeaderDetailFormSave(JObject json) {
			TestData header = json["header"].To<TestData>();
			List<TestDetail> detail = json["detail"].To<List<TestDetail>>();
			AjaxReturn r = SaveRecord(header);
			if(r.error == null) {
				Database.Execute("DELETE FROM TestDetail");
				foreach (TestDetail d in detail) {
					if (!string.IsNullOrEmpty(d.Text)) {
						d.idTestDetail = null;
						Database.Insert(d);
					}
				}
			}
			return r;
		}

	}
}
