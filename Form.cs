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

	public class Form {

		public Form(AppModule module, Type t) 
			: this(module, t, true) {
		}

		public Form(AppModule module, Type t, bool readWrite) {
			Module = module;
			ReadWrite = readWrite;
			Type table = t;
			Type view = null;
			while (table != typeof(JsonObject) && !table.IsDefined(typeof(TableAttribute))) {
				if (view == null && table.IsDefined(typeof(ViewAttribute)))
					view = table;
				table = table.BaseType;
			}
			if (table != typeof(JsonObject)) {
				Options["table"] = table.Name;
				Options["id"] = Module.Database.TableFor(table.Name).PrimaryKey.Name;
			}
			processFields(t);
		}

		public AppModule Module;

		public JObject Options = new JObject().AddRange("columns", new JArray());

		public JArray Columns {
			get { return (JArray)Options["columns"]; }
		}

		public bool ReadWrite;

		public void Show(string formType) {
			Module.Form = this;
			Module.WriteResponse(Module.Template(formType.ToLower(), Module), "text/html", System.Net.HttpStatusCode.OK);
		}

		void processFields(Type tbl) {
			if (tbl.BaseType != typeof(JsonObject))	// Process base types first
				processFields(tbl.BaseType);
			bool readwrite = ReadWrite && tbl.IsDefined(typeof(TableAttribute));
			foreach (FieldInfo field in tbl.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) {
				PrimaryAttribute pk;
				Field fld = Field.FieldFor(field, out pk);
				if (fld == null)
					continue;
				FieldAttribute f = field.GetCustomAttribute<FieldAttribute>();
				if(f == null) {
					ForeignKeyAttribute fk = field.GetCustomAttribute<ForeignKeyAttribute>();
					if (fk == null) {
						f = new FieldAttribute();
					} else {
						Table t = Module.Database.TableFor(fk.Table);
						string valueName = t.Indexes.Length > 1 ? t.Indexes[1].Fields[0].Name : t.Fields[1].Name;
						f = new SelectAttribute(Module.Database.Query("SELECT " + t.PrimaryKey.Name + " AS id, "
							+ valueName + " AS value FROM " + t.Name
							+ " ORDER BY " + valueName), readwrite);
					}
				}
				if (f.Data == null)
					f.Data = fld.Name;
				if(f.Type == null) {
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
				if(f.Type == "textInput" && f.Size == 0 && fld.Length > 0)
					f.Size = (int)Math.Floor(fld.Length);
				Columns.Add(f.Options);
			}
		}

		public IEnumerable<FieldAttribute> Fields {
			get {
				return Columns.Select(f => new FieldAttribute() { Options = (JObject)f });
			}
		}

		public FieldAttribute this[string name] {
			get {
				return Fields.FirstOrDefault(f => f.Name == name) ?? Fields.FirstOrDefault(f => f.Data == name);
			}
		}
	}

}
