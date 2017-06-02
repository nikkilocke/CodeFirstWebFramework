using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	/// <summary>
	/// Attribute to define field display in forms
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class FieldAttribute : Attribute {
		public FieldAttribute() {
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args">Pairs of name, value, passed direct into Options (so must be javascript style starting with lower case letter)</param>
		public FieldAttribute(params object [] args) {
			Utils.Check(args.Length % 2 == 0, "Field arguments must be in pairs");
			for (int i = 0; i < args.Length; i += 2) {
				string name = args[i] as string;
				Utils.Check(!string.IsNullOrWhiteSpace(name), "Field argument {0} is not a string", i);
				Options[name] = args[i + 1].ToJToken();
			}
		}

		/// <summary>
		/// The javascript options for the field
		/// </summary>
		public JObject Options = new JObject();

		/// <summary>
		/// Name of variable containing field value
		/// </summary>
		public string Data {
			get { return Options.AsString("data"); }
			set { Options["data"] = value; }
		}

		/// <summary>
		/// Type of field - see list in default.js
		/// </summary>
		public string Type {
			get { return Options.AsString("type"); }
			set { Options["type"] = value; }
		}

		/// <summary>
		/// Heading/prompt for field (defaults to Data, un camel cased)
		/// </summary>
		public string Heading {
			get { return Options.AsString("heading"); }
			set { Options["heading"] = value; }
		}

		/// <summary>
		/// How many columns for field
		/// </summary>
		public int Colspan {
			get { return Options.AsInt("colspan"); }
			set { Options["colspan"] = value; }
		}

		/// <summary>
		/// True if field is to be in the same row as the previous field
		/// </summary>
		public bool SameRow {
			get { return Options.AsBool("sameRow"); }
			set { Options["sameRow"] = value; }
		}

		/// <summary>
		/// Html attributes to add to field
		/// </summary>
		public string Attributes {
			get { return Options.AsString("attributes"); }
			set { Options["attributes"] = value; }
		}

		/// <summary>
		/// Name of field - should be unique within a form. Defaults to same as Data.
		/// </summary>
		public string Name {
			get { return Options.AsString("name"); }
			set { Options["name"] = value; }
		}

		/// <summary>
		/// Number of characters to allow in input
		/// </summary>
		public int Size {
			get { return Options.AsInt("size"); }
			set { Options["size"] = value; }
		}

		/// <summary>
		/// Set to false to hide the field (in DataTables) or omit it (in other forms)
		/// </summary>
		public bool Visible {
			get { return Options["visible"] == null ? true : Options.AsBool("visible"); }
			set { Options["visible"] = value; }
		}

		/// <summary>
		/// Name of field (allowing for default to Data)
		/// </summary>
		public string FieldName {
			get { return Name ?? Data; }
		}

		/// <summary>
		/// SQL Field definition
		/// </summary>
		public Field Field;

		/// <summary>
		/// Create a FieldAttribute for the given property in a class.
		/// </summary>
		/// <param name="db">Database (needed to retrieve default select options)</param>
		/// <param name="field">Property definition</param>
		/// <param name="readwrite">True if the user can edit the field</param>
		public static FieldAttribute FieldFor(Database db, FieldInfo field, bool readwrite) {
			PrimaryAttribute pk;
			Field fld = Field.FieldFor(field, out pk);
			if (fld == null)
				return null;
			if (readwrite && field.IsDefined(typeof(ReadOnlyAttribute)))
				readwrite = false;
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

	/// <summary>
	/// Special FieldAttribute for select fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class SelectAttribute : FieldAttribute {

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="options">The options - each JObject shold have an id and a value (at least). 
		/// If they have a category, a categorised select is created.</param>
		/// <param name="readwrite">True if the user can input to the field.</param>
		public SelectAttribute(JObjectEnumerable options, bool readwrite) {
			Type = readwrite ? "selectInput" : "select";
			SelectOptions = options;
		}

		/// <summary>
		/// Constructor for an input select.
		/// </summary>
		/// <param name="options">The options - each JObject shold have an id and a value (at least). 
		/// If they have a category, a categorised select is created.</param>
		public SelectAttribute(JObjectEnumerable options)
			: this(options, true) {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="options">The options - each JObject shold have an id and a value (at least). 
		/// If they have a category, a categorised select is created.</param>
		/// <param name="readwrite">True if the user can input to the field.</param>
		public SelectAttribute(IEnumerable<JObject> options, bool readwrite)
			: this(new JObjectEnumerable(options), readwrite) {
		}

		/// <summary>
		/// Set the select options.
		/// Each JObject shold have an id and a value (at least). 
		/// If they have a category, a categorised select is created.
		/// </summary>
		public JObjectEnumerable SelectOptions {
			set { Options["selectOptions"] = (JArray)value; }
		}
	}

	/// <summary>
	/// Indicate a field or class is writeable by default, even if it is not part of a Table
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
	public class WriteableAttribute : Attribute {
	}

	/// <summary>
	/// Indicate a field or class is readonly by default, even if it is part of a Table
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ReadOnlyAttribute : Attribute {
	}

	/// <summary>
	/// Base class for all supported forms
	/// </summary>
	public abstract class BaseForm {

		public BaseForm(AppModule module) {
			Module = module;
			Options = new JObject();
		}

		/// <summary>
		/// Form data passed to javascript.
		/// </summary>
		public JToken Data {
			get { return Options["data"]; }
			set { Options["data"] = value; }
		}

		public AppModule Module;

		/// <summary>
		/// Form options passed to javascript.
		/// </summary>
		public JObject Options;

		/// <summary>
		/// Build the form html from a template. By default it uses /modulename/methodname.tmpl, but, if that doesn't
		/// exist, it uses the default template for the form (e.g. /datatable.tmpl).
		/// </summary>
		public abstract void Show();

		/// <summary>
		/// Build the form html from a template. By default it uses /modulename/methodname.tmpl, but, if that doesn't
		/// exist, it uses the default template for the form /formType.tmpl.
		/// </summary>
		protected void Show(string formType) {
			string filename = System.IO.Path.Combine(Module.Module, Module.Method).ToLower();
			if (!Module.Server.FileInfo(filename + ".tmpl").Exists)
				filename = formType.ToLower();
			Module.Form = this;
			Module.WriteResponse(Module.Template(filename, Module), "text/html", System.Net.HttpStatusCode.OK);
		}

	}

	/// <summary>
	/// Normal input form.
	/// </summary>
	public class Form : BaseForm {
		/// <summary>
		/// The fields
		/// </summary>
		private JArray columns;

		/// <summary>
		/// Empty form
		/// </summary>
		/// <param name="module">Owning module</param>
		/// <param name="readwrite">Whether the user can input to some of the fields</param>
		public Form(AppModule module, bool readwrite)
			: base(module) {
			ReadWrite = readwrite;
		}

		/// <summary>
		/// Readwrite form for C# type t
		/// </summary>
		public Form(AppModule module, Type t)
			: this(module, t, true) {
		}

		/// <summary>
		/// Form for C# type t
		/// </summary>
		public Form(AppModule module, Type t, bool readwrite)
			: this(module, readwrite) {
			columns = new JArray();
			Options["columns"] = columns;
			Build(t);
		}

		/// <summary>
		/// Whether the user can input
		/// </summary>
		public bool ReadWrite;

		/// <summary>
		/// Add a field from a C# class to the form
		/// </summary>
		public FieldAttribute Add(FieldInfo field) {
			return Add(field, ReadWrite && field.DeclaringType.IsDefined(typeof(TableAttribute), false));
		}

		/// <summary>
		/// Add a field from a C# class to the form
		/// </summary>
		public FieldAttribute Add(FieldInfo field, bool readwrite) {
			FieldAttribute f = FieldAttribute.FieldFor(Module.Database, field, readwrite);
			if (f != null)
				columns.Add(f.Options);
			return f;
		}

		/// <summary>
		/// Insert a field from a C# class to the form
		/// </summary>
		public void Insert(int position, FieldAttribute f) {
			columns.Insert(position, f.Options);
		}

		/// <summary>
		/// Replace a field from a C# class to the form
		/// </summary>
		public void Replace(int position, FieldAttribute f) {
			columns[position] = f.Options;
		}

		/// <summary>
		/// Return the index of the named field in the form
		/// </summary>
		public int IndexOf(string name) {
			int i = 0;
			foreach(FieldAttribute f in Fields) {
				if (f.FieldName == name)
					return i;
				i++;
			}
			return -1;
		}

		/// <summary>
		/// Add all the suitable fields from a C# type to the form
		/// </summary>
		public void Build(Type t) {
			Type table = t;
			while (table != typeof(JsonObject)) {
				if (table.IsDefined(typeof(TableAttribute), false)) {
					Options["table"] = table.Name;
					Options["id"] = Module.Database.TableFor(table.Name).PrimaryKey.Name;
					break;
				}
				table = table.BaseType;
			}
			processFields(t);
		}

		/// <summary>
		/// Remove the named field
		/// </summary>
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

		/// <summary>
		/// All the fields in this form
		/// </summary>
		public IEnumerable<FieldAttribute> Fields {
			get {
				return columns.Select(f => new FieldAttribute() { Options = (JObject)f });
			}
		}

		/// <summary>
		/// Find a field by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public FieldAttribute this[string name] {
			get {
				return Fields.FirstOrDefault(f => f.FieldName == name);
			}
		}

		/// <summary>
		/// Decide whether a field should be included - e.g. autoincrement and non-visible fields are excluded by default.
		/// </summary>
		protected virtual bool RequireField(FieldAttribute field) {
			return !field.Field.AutoIncrement && field.Visible;
		}

		/// <summary>
		/// Process all the fields from a type (do any base classes first)
		/// </summary>
		/// <param name="tbl"></param>
		void processFields(Type tbl) {
			if (tbl.BaseType != typeof(JsonObject))	// Process base types first
				processFields(tbl.BaseType);
			bool readwrite = ReadWrite && (tbl.IsDefined(typeof(TableAttribute), false) || tbl.IsDefined(typeof(WriteableAttribute), false));
			foreach (FieldInfo field in tbl.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) {
				FieldAttribute f = FieldAttribute.FieldFor(Module.Database, field, readwrite || (ReadWrite && field.IsDefined(typeof(WriteableAttribute))));
				if (f != null && RequireField(f))
					columns.Add(f.Options);
			}
		}

	}

	/// <summary>
	/// DataTable (jquery) form - always readonly.
	/// </summary>
	public class DataTableForm : Form {
		public DataTableForm(AppModule module, Type t) 
			: base(module, t, false) {
		}

		public override void Show() {
			base.Show("DataTable");
		}

		/// <summary>
		/// Non-visible fields are included (so you can search on them)
		/// </summary>
		protected override bool RequireField(FieldAttribute field) {
			return !field.Field.AutoIncrement;
		}

	}

	/// <summary>
	/// List form
	/// </summary>
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

	/// <summary>
	/// Header detailt form
	/// </summary>
	public class HeaderDetailForm : BaseForm {

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="module">Owning module</param>
		/// <param name="header">C# type in the header</param>
		/// <param name="detail">C# type in the detail</param>
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
