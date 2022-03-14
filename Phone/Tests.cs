﻿using System;
using System.Collections.Generic;
using System.Text;
using CodeFirstWebFramework;

namespace Phone {
	enum TestValues {
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
		public bool Boolean;
		public int Option;
		[Length(0)]
		public string TextArea;
	}
	public class Tests : AppModule {
		protected override void Init() {
			base.Init();
			InsertMenuOptions(new MenuOption("Test Form", "/Tests/TestForm"),
				new MenuOption("Credit/Radio Form", "/Tests/TestForm?credit=y"));
		}

		public Form TestForm() {
			bool edit = GetParameters["edit"] == "y";
			if (!edit)
				InsertMenuOption(new MenuOption("Edit", Request.Url + (string.IsNullOrEmpty(Request.Url.Query) ?"?" : "&") + "edit=y"));
			Form form = new Form(this, typeof(TestData), edit);
			form["Option"].MakeSelectable(typeof(TestValues));
			if(GetParameters["credit"] == "y") {
				form["Decimal"].Type = "creditInput";
				form["Decimal"].Heading = "Debit";
				form.Insert(form.IndexOf("Decimal") + 1, new FieldAttribute("name", "Debit", "data", "Decimal", "heading", "Debit", "type", "debitInput"));
				form["Option"].Type = edit ? "radioInput" : "radio";
			}
			form.Data = Database.QueryOne("SELECT * FROM TestData") ?? new Newtonsoft.Json.Linq.JObject();
			return form;
		}

		public AjaxReturn TestFormSave(TestData json) {
			return SaveRecord(json);
		}

	}
}
