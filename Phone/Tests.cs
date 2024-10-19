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
		Value4,
		Invalid
	}
	[Table]
	public class TestData : JsonObject {
		[Primary]
		public int? idTestData;
		public string Text;
		[Field(Page = 2)]
		public DateTime Date;
		[Field(Page = 2)]
		public decimal Decimal;
		[Field(Page = 2)]
		public double Double;
		[Field(Page = 2)]
		public int Integer;
		[Field(Hint = "This is a test hint on a checkbox", 
			Preamble =  "This is a preamble with a <a href=\"/\" target=\"_blank\">link</a>",
			Postamble = "This is a postamble with a <a href=\"/\" target=\"_blank\">link</a>",
			Page = 3)]
		public bool Boolean;
		[Field(Hint =  "This is a test hint on an Enum",
			Page = 3)]
		public TestValues Option;
        [Length(0)]
		[Field(Hint = "This is a test hint",
			Page = 3)]
		public string TextArea;
		[Length(0)]
		[Field(Type = "inlineImageInput",
			Page = 3)]
		public string Image;
	}
	[Table]
	public class TestDetail : JsonObject {
		[Primary]
		public int? idTestDetail;
		public string Text;
		public TestValues Option;
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

		public Form TestForm(string edit = null, string pages = null) {
			if (edit != "y")
				InsertMenuOption(new MenuOption("Edit", Request.Url + (string.IsNullOrEmpty(Request.Url.Query) ?"?" : "&") + "edit=y"));
			Form form = new Form(this, typeof(TestData), edit == "y");
			if(GetParameters["credit"] == "y") {
				form["Decimal"].Type = "creditInput";
				form["Decimal"].Heading = "Debit";
				form.Insert(form.IndexOf("Decimal") + 1, new FieldAttribute("name", "Debit", "data", "Decimal", "heading", "Debit", "type", "debitInput"));
				if (edit == "y")
					form["Option"].Type = "radioInput";
            }
			form.Data = Database.QueryOne("SELECT * FROM TestData") ?? new JObject();
			if (pages == "y")
				form.SetPages("Text", "Numbers", "Others");
			else
				InsertMenuOption(new MenuOption("Pages", Request.Url + (string.IsNullOrEmpty(Request.Url.Query) ? "?" : "&") + "pages=y"));
			return form;
		}

		public AjaxReturn TestFormSave(TestData json) {
			Form form = new Form(this, typeof(TestData));
			form.Check("Text", !string.IsNullOrEmpty(json.Text), "Text may not be empty");
			form.Check("Integer", json.Integer != 0, "Integer may not be zero");
			form.Check("Option", json.Option != TestValues.Invalid, "You may not choose the Invalid option");
			return SaveRecord(json);
		}

		public HeaderDetailForm HeaderDetailForm(string edit = null, string pages = null) {
			if (edit != "y")
				InsertMenuOption(new MenuOption("Edit", Request.Url + (string.IsNullOrEmpty(Request.Url.Query) ? "?" : "&") + "edit=y"));
			Form header = new Form(this, typeof(TestData), edit == "y");
			ListForm detail = new ListForm(this, typeof(TestDetail), edit == "y");
			detail.Options["search"] = true;
			detail.Options["addRows"] = true;
			detail.Options["deleteRows"] = true;
			detail.Options["sortable"] = true;
			HeaderDetailForm form = new HeaderDetailForm(this, header, detail);
			form.Data = new JObject().AddRange(
				"header", Database.QueryOne("SELECT * FROM TestData") ?? new JObject(),
				"detail", Database.Query("SELECT * FROM TestDetail ORDER BY idTestDetail")
				);
			if (pages == "y") {
				header.SetPages("Text", "Numbers", "Others", "Details");
				detail.Page = 4;
				header.MultiPageOptions["saveOnAllPages"] = true;
				header.MultiPageOptions["cancel"] = true;
			} else
				InsertMenuOption(new MenuOption("Pages", Request.Url + (string.IsNullOrEmpty(Request.Url.Query) ? "?" : "&") + "pages=y"));
			return form;
		}

		public AjaxReturn HeaderDetailFormSave(JObject json) {
			TestData header = json["header"].To<TestData>();
			List<TestDetail> detail = json["detail"].To<List<TestDetail>>();
			Database.BeginTransaction();
			Form form = new Form(this, typeof(TestData));
			form.Check("Text", !string.IsNullOrEmpty(header.Text), "Text may not be empty");
			form.Check("Integer", header.Integer != 0, "Integer may not be zero");
			form.Check("Option", header.Option != TestValues.Invalid, "You may not choose the Invalid option");
			AjaxReturn r = SaveRecord(header);
			if(r.error == null) {
				Database.Execute("DELETE FROM TestDetail");
				foreach (TestDetail d in detail) {
					if (!string.IsNullOrEmpty(d.Text)) {
						Utils.Check(4, d.Option != TestValues.Invalid, "You may not choose the Invalid option");
						d.idTestDetail = null;
						Database.Insert(d);
					}
				}
				Database.Commit();
			}
			return r;
		}

	}
}
