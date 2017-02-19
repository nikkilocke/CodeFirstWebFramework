using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	public class FieldAttribute : Attribute {
		public FieldAttribute() {
		}

		public FieldAttribute(params object [] args) {
			Utils.Check(args.Length % 2 == 0, "Field arguments must be in pairs");
			for (int i = 0; i < args.Length; i += 2) {
				string name = args[i] as string;
				Utils.Check(!string.IsNullOrWhiteSpace(name), "Field argument {0} is not a string", i);
				Options[name] = args[i + 1].ToJToken();
			}
		}

		public JObject Options = new JObject();

		public string Data {
			get { return Options.AsString("data"); }
			set { Options["data"] = value; }
		}

		public string Type {
			get { return Options.AsString("type"); }
			set { Options["type"] = value; }
		}

		public string Heading {
			get { return Options.AsString("heading"); }
			set { Options["heading"] = value; }
		}

		public int Colspan {
			get { return Options.AsInt("colspan"); }
			set { Options["colspan"] = value; }
		}

		public bool SameRow {
			get { return Options.AsBool("sameRow"); }
			set { Options["sameRow"] = value; }
		}

		public string Attributes {
			get { return Options.AsString("attributes"); }
			set { Options["attributes"] = value; }
		}

		public string Name {
			get { return Options.AsString("name"); }
			set { Options["name"] = value; }
		}

		public int Size {
			get { return Options.AsInt("size"); }
			set { Options["size"] = value; }
		}

		public bool Visible {
			get { return Options["visible"] == null ? true : Options.AsBool("visible"); }
			set { Options["visible"] = value; }
		}

		public string FieldName {
			get { return Name ?? Data; }
		}

		public Field Field;

		public static FieldAttribute FieldFor(Database db, FieldInfo field, bool readwrite) {
			PrimaryAttribute pk;
			Field fld = Field.FieldFor(field, out pk);
			if (fld == null)
				return null;
			FieldAttribute f = field.GetCustomAttribute<FieldAttribute>();
			if (f == null) {
				ForeignKeyAttribute fk = field.GetCustomAttribute<ForeignKeyAttribute>();
				if (fk == null) {
					f = new FieldAttribute();
				} else {
					Table t = db.TableFor(fk.Table);
					string valueName = t.Indexes.Length < 2 ? t.Fields[1].Name :
						t.Indexes[1].Fields.Length < 2 ? t.Indexes[1].Fields[0].Name :
						"CONCAT(" + String.Join(",' ',", t.Indexes[1].Fields.Select(fi => fi.Name).ToArray()) + ")";
					f = new SelectAttribute(db.Query("SELECT " + t.PrimaryKey.Name + " AS id, "
						+ valueName + " AS value FROM " + t.Name
						+ " ORDER BY " + valueName), readwrite);
				}
			}
			f.Field = fld;
			if (f.Data == null)
				f.Data = fld.Name;
			if (f.Type == null) {
				switch (fld.Type.Name) {
					case "Int32":
						f.Type = readwrite ? "intInput" : "int";
						break;
					case "Decimal":
						f.Type = readwrite ? "decimalInput" : "decimal";
						break;
					case "Double":
						f.Type = readwrite ? "doubleInput" : "double";
						break;
					case "Boolean":
						f.Type = readwrite ? "checkboxInput" : "checkbox";
						break;
					case "DateTime":
						f.Type = readwrite ? "dateInput" : "date";
						break;
					default:
						f.Type = fld.Length == 0 ?
							readwrite ? "textAreaInput" : "textArea" :
							readwrite ? "textInput" : "string";
						break;
				}
			}
			if (f.Type == "textInput" && f.Size == 0 && fld.Length > 0)
				f.Size = (int)Math.Floor(fld.Length);
			return f;
		}
	}

	public class SelectAttribute : FieldAttribute {

		public SelectAttribute(JObjectEnumerable options, bool readwrite) {
			Type = readwrite ? "selectInput" : "select";
			SelectOptions = options;
		}

		public SelectAttribute(JObjectEnumerable options)
			: this(options, true) {
		}

		public SelectAttribute(IEnumerable<JObject> options, bool readwrite)
			: this(new JObjectEnumerable(options), readwrite) {
		}

		JObjectEnumerable SelectOptions {
			set { Options["selectOptions"] = (JArray)value; }
		}
	}

	public abstract class BaseForm {

		public BaseForm(AppModule module) {
			Module = module;
			Options = new JObject();
		}

		public JToken Data {
			get { return Options["data"]; }
			set { Options["data"] = value; }
		}

		public AppModule Module;

		public JObject Options;

		public abstract void Show();

		protected void Show(string formType) {
			Module.Form = this;
			Module.WriteResponse(Module.Template(formType.ToLower(), Module), "text/html", System.Net.HttpStatusCode.OK);
		}

	}

	public class Form : BaseForm {

		private JArray columns;

		public Form(AppModule module, Type t)
			: this(module, t, true) {
		}

		public Form(AppModule module, Type t, bool readWrite) 
		: base(module) {
			ReadWrite = readWrite;
			columns = new JArray();
			Options["columns"] = columns;
			Build(t);
		}

		public bool ReadWrite;

		public FieldAttribute Add(FieldInfo field) {
			return Add(field, ReadWrite && field.DeclaringType.IsDefined(typeof(TableAttribute)));
		}

		public FieldAttribute Add(FieldInfo field, bool readwrite) {
			FieldAttribute f = FieldAttribute.FieldFor(Module.Database, field, readwrite);
			if (f != null)
				columns.Add(f.Options);
			return f;
		}

		public void Build(Type t) {
			Type table = t;
			while (table != typeof(JsonObject)) {
				if (table.IsDefined(typeof(TableAttribute))) {
					Options["table"] = table.Name;
					Options["id"] = Module.Database.TableFor(table.Name).PrimaryKey.Name;
					break;
				}
				table = table.BaseType;
			}
			processFields(t);
		}

		public void Remove(string name) {
			int i = 0;
			bool found = false;
			foreach (FieldAttribute f in Fields) {
				if (f.FieldName == name) {
					found = true;
					break;
				}
				i++;
			}
			if (found)
				columns.RemoveAt(i);
		}

		public override void Show() {
			Show("Form");
		}

		public IEnumerable<FieldAttribute> Fields {
			get {
				return columns.Select(f => new FieldAttribute() { Options = (JObject)f });
			}
		}

		public FieldAttribute this[string name] {
			get {
				return Fields.FirstOrDefault(f => f.FieldName == name);
			}
		}

		protected virtual bool RequireField(FieldAttribute field) {
			return !field.Field.AutoIncrement && field.Visible;
		}

		void processFields(Type tbl) {
			if (tbl.BaseType != typeof(JsonObject))	// Process base types first
				processFields(tbl.BaseType);
			bool readwrite = ReadWrite && tbl.IsDefined(typeof(TableAttribute));
			foreach (FieldInfo field in tbl.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) {
				FieldAttribute f = FieldAttribute.FieldFor(Module.Database, field, readwrite);
				if (f != null && RequireField(f))
					columns.Add(f.Options);
			}
		}

	}

	public class DataTableForm : Form {
		public DataTableForm(AppModule module, Type t) 
			: base(module, t, true) {
		}

		public DataTableForm(AppModule module, Type t, bool readWrite)
			: base(module, t, readWrite) {
		}

		public override void Show() {
			base.Show("DataTable");
		}

		protected override bool RequireField(FieldAttribute field) {
			return !field.Field.AutoIncrement;
		}

	}

	public class ListForm : Form {
		public ListForm(AppModule module, Type t)
			: base(module, t, true) {
		}

		public ListForm(AppModule module, Type t, bool readWrite)
			: base(module, t, readWrite) {
		}

		public override void Show() {
			base.Show("ListForm");
		}

	}

	public class HeaderDetailForm : BaseForm {

		public HeaderDetailForm(AppModule module, Type header, Type detail) 
		: base(module) {
			Header = new Form(module, header);
			Detail = new ListForm(module, detail);
			Options["header"] = Header.Options;
			Options["detail"] = Detail.Options;
		}

		public Form Header;

		public ListForm Detail;

		public override void Show() {
			Show("HeaderDetailForm");
		}

	}

}
