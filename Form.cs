using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace CodeFirstWebFramework {
	/// <summary>
	/// Attribute to define field display in forms
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class FieldAttribute : Attribute {

		/// <summary>
		/// Constructor
		/// </summary>
		public FieldAttribute() {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="args">Pairs of name, value, passed direct into Options (so must be javascript style starting with lower case letter)</param>
		public FieldAttribute(params object[] args) {
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
		/// Pair of types, separated by , - first is read only, second is read-write
		/// </summary>
		public string Types;

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
		/// Popup hint text (text only)
		/// </summary>
		public string Hint {
			get { return Options.AsString("hint"); }
			set { Options["hint"] = value; }
		}

		/// <summary>
		/// Page number for multi-page forms - leave at 0 for the field to appear on every page
		/// </summary>
		public int Page {
			get { return Options.AsInt("page"); }
			set { Options["page"] = value; }
		}

		/// <summary>
		/// Preamble appears above field in plain Forms (may have html)
		/// </summary>
		public string Preamble {
			get { return Options.AsString("preamble"); }
			set { Options["preamble"] = value; }
		}

		/// <summary>
		/// Postamble appears immediately after field, in the same cell
		/// </summary>
		public string Postamble {
			get { return Options.AsString("postamble"); }
			set { Options["postamble"] = value; }
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
		/// Set to true to call MethodNameNotify(string fieldName, JObject json) when the field is changed by the user
		/// </summary>
		public bool Notify {
			get { return Options["notify"] == null ? false : Options.AsBool("notify"); }
			set { Options["notify"] = value; }
		}

		/// <summary>
		/// Number of characters to allow in input
		/// </summary>
		public int MaxLength {
			get { return Options.AsInt("maxlength"); }
			set { Options["maxlength"] = value; }
		}

		/// <summary>
		/// Set to false to hide the field (in DataTables) or omit it (in other forms)
		/// </summary>
		public bool Visible {
			get { return Options["visible"] == null ? true : Options.AsBool("visible"); }
			set { Options["visible"] = value; }
		}

		/// <summary>
		/// Class name to apply to field row in plain Forms
		/// </summary>
		public string Class {
			get { return Options.AsString("@class"); }
			set { Options["@class"] = value; }
		}

		/// <summary>
		/// Name of field (allowing for default to Data)
		/// </summary>
		public string FieldName {
			get { return Name ?? Data; }
		}

		/// <summary>
		/// Turn a field into a select or selectInput
		/// </summary>
		/// <param name="values">IEnumerable JObject containing id and value to make the select options</param>
		/// <param name="defaultLabel">Label to use if there is to be an option for "none of the above" (null if there isn't to be such an option)</param>
		/// <param name="defaultValue">Value to use for for "none of the above"</param>
		public FieldAttribute MakeSelectable(IEnumerable<JObject> values, string defaultLabel = null, int defaultValue = 0) {
			if (defaultLabel != null)
				values = (new JObject[] { new JObject().AddRange("id", defaultValue, "value", defaultLabel) }).Concat(values);
			JArray j = new JArray();
			foreach (JObject jo in values)
				j.Add(jo);
			Options["selectOptions"] = j;
			Type = Options.AsBool("readonly") ? "select" : "selectInput";
			return this;
		}

		/// <summary>
		/// Turn an Enum field into a select or selectInput
		/// </summary>
		static public IEnumerable<JObject> SelectValues(Type enumType) {
			foreach (var v in Enum.GetValues(enumType)) {
				JObject j = new JObject {
					["id"] = (int)v,
					["value"] = Enum.GetName(enumType, v).UnCamel()
				};
				yield return j;
			}
		}

		/// <summary>
		/// Turn a field into a select or selectInput
		/// </summary>
		/// <param name="enumType">Enum type for field</param>
		/// <param name="defaultLabel">Label to use if there is to be an option for "none of the above" (null if there isn't to be such an option)</param>
		/// <param name="defaultValue">Value to use for for "none of the above"</param>
		public FieldAttribute MakeSelectable(Type enumType, string defaultLabel = null, int defaultValue = 0) {
			return MakeSelectable(SelectValues(enumType), defaultLabel, defaultValue);
		}

		/// <summary>
		/// SQL Field definition
		/// </summary>
		public Field Field;

		/// <summary>
		/// Create a FieldAttribute for the given field in a class.
		/// </summary>
		/// <param name="db">Database (needed to retrieve default select options)</param>
		/// <param name="field">FieldInfo definition</param>
		/// <param name="readwrite">True if the user can edit the field</param>
		public static FieldAttribute FieldFor(Database db, FieldInfo field, bool readwrite) {
			Field fld = Field.FieldFor(field);
			if (fld == null)
				return null;
			if (readwrite && (field.IsDefined(typeof(ReadOnlyAttribute)) || field.IsDefined(typeof(DoNotStoreAttribute))))
				readwrite = false;
			FieldAttribute f = field.GetCustomAttribute<FieldAttribute>();
			if (f == null) {
				f = new FieldAttribute();
				f.SetField(fld, readwrite);
				ForeignKeyAttribute fk = field.GetCustomAttribute<ForeignKeyAttribute>();
				if (fk != null && fk.FieldName != ForeignKeyAttribute.NoAutoSelect && (ForeignKeyAttribute.AutoSelect || fk.FieldName != null)) {
					Table t = db.TableFor(fk.Table);
					string valueName = fk.FieldName == null ? t.Indexes.Length < 2 ? t.Fields[1].Name :
						t.Indexes[1].Fields.Length < 2 ? t.Indexes[1].Fields[0].Name :
						"CONCAT(" + String.Join(",' ',", t.Indexes[1].Fields.Select(fi => fi.Name)) + ")" :
						fk.FieldName;
					f.MakeSelectable(db.Query("SELECT " + t.PrimaryKey.Name + " AS id, "
						+ valueName + " AS value FROM " + t.Name
						+ " ORDER BY " + valueName));
				}
			} else {
				f.SetField(fld, readwrite);
			}
			return f;
		}

		/// <summary>
		/// Create a FieldAttribute for the given property in a class.
		/// </summary>
		/// <param name="field">Property definition</param>
		/// <param name="readwrite">True if the user can edit the field</param>
		public static FieldAttribute FieldFor(PropertyInfo field, bool readwrite) {
			Field fld = Field.FieldFor(field);
			if (fld == null)
				return null;
			if (readwrite && (field.IsDefined(typeof(ReadOnlyAttribute)) || field.IsDefined(typeof(DoNotStoreAttribute))))
				readwrite = false;
			FieldAttribute f = field.GetCustomAttribute<FieldAttribute>() ?? new FieldAttribute();
			f.SetField(fld, readwrite);
			return f;
		}

		void SetField(Field fld, bool readwrite) {
			Field = fld;
			if (Data == null)
				Data = fld.Name;
			Options["readonly"] = !readwrite;
			if (Type == null) {
				if (Types != null) {
					string[] t = Types.Split(',');
					if (readwrite) {
						if (t.Length > 1)
							Type = t[1];
					} else
						Type = t[0];
				}
				if (string.IsNullOrEmpty(Type)) {
					switch (fld.Type.Name) {
						case "Int64":
						case "Int32":
							Type = readwrite ? "intInput" : "int";
							break;
						case "Decimal":
							Type = readwrite ? "decimalInput" : "decimal";
							break;
						case "Double":
							Type = readwrite ? "doubleInput" : "double";
							break;
						case "Boolean":
							Type = readwrite ? "checkboxInput" : "checkbox";
							break;
						case "DateTime":
							Type = readwrite ? "dateInput" : "date";
							break;
						default:
							if (fld.Type.IsEnum) {
								MakeSelectable(fld.Type);
								break;
							}
							Type = fld.Length == 0 ?
								readwrite ? "textAreaInput" : "textArea" :
								readwrite ? "textInput" : "string";
							break;
					}
				}
			}
			if (Type == "textInput" && MaxLength == 0 && fld.Length > 0)
				MaxLength = (int)Math.Floor(fld.Length);
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

		/// <summary>
		/// Constructor
		/// </summary>
		public BaseForm(AppModule module) {
			Module = module;
			Options = new JObject();
		}

		/// <summary>
		/// Form data passed to javascript.
		/// </summary>
		public object Data {
			get { return Options["data"]; }
			set { Options["data"] = value.ToJToken(); }
		}

		/// <summary>
		/// Set to true to call MethodNameAutoSave(JObject json) when any field is changed by the user
		/// </summary>
		public bool AutoSave {
			get { return Options["autosave"] == null ? false : Options.AsBool("autosave"); }
			set { Options["autosave"] = value; }
		}

		/// <summary>
		/// Set to true to cache all changes when any field is changed by the user, and offer to restore them
		/// if they revisit the form with the same url and query string
		/// </summary>
		public bool CacheChanges {
			get { return Options["cacheChanges"] == null ? false : Options.AsBool("cacheChanges"); }
			set { Options["cacheChanges"] = value; }
		}

		/// <summary>
		/// Module creating the form
		/// </summary>
		public AppModule Module;

		/// <summary>
		/// Form options passed to javascript.
		/// </summary>
		public JObject Options;

		static Regex _sanitise = new Regex("< */script", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// Options as safe json (for direct inclusion in javascript)
		/// </summary>
		public string SafeOptions {
			get { return _sanitise.Replace(Options.ToString(), @"<\/script"); }
		}

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
			if (!Module.FileInfo(filename + ".tmpl").Exists)
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
		private readonly JArray columns = new JArray();

		/// <summary>
		/// Empty form
		/// </summary>
		/// <param name="module">Owning module</param>
		/// <param name="readwrite">Whether the user can input to some of the fields</param>
		public Form(AppModule module, bool readwrite)
			: base(module) {
			if (!module.HasAccess(module.Info, module.Method + "save", out _))
				readwrite = false;
			ReadWrite = readwrite;
			if (!readwrite)
				Options["readonly"] = true;
			Options["columns"] = columns;
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
			Build(t);
		}

		/// <summary>
		/// Form for C# type t with specific fields in specific order
		/// </summary>
		public Form(AppModule module, Type t, bool readwrite, params string[] fieldNames)
			: this(module, readwrite) {
			setTableName(t);
			foreach (string name in fieldNames) {
				Add(t, name);
			}
		}

		/// <summary>
		/// Whether the user can input
		/// </summary>
		public bool ReadWrite;

		/// <summary>
		/// Find or create the "multipage" options JObject
		/// </summary>
		public JObject MultiPageOptions => Options.FindOrCreatePath("multipage");

		/// <summary>
		/// List of page titles for multi-page forms. If not set, the form will be single page.
		/// </summary>
		public string[] Pages {
			get {
				JToken p = Options.SelectToken("multipage.pages");
				return p is JArray && ((JArray)p).Count > 0 ? p.To<string[]>() : null;
			}
			set { MultiPageOptions["pages"] = value.ToJToken(); }
		}

		/// <summary>
		/// Helper function to set list of page titles for multi-page forms
		/// </summary>
		public void SetPages(params string[] pageTitles) {
			MultiPageOptions["pages"] = pageTitles.ToJToken();
		}

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
		/// Add a field to the form
		/// </summary>
		public void Add(FieldAttribute f) {
			columns.Add(f.Options);
		}

		bool readWriteFlagForTable(Type t, bool writeable) {
			return ReadWrite && (writeable || t.IsDefined(typeof(TableAttribute), false) || t.IsDefined(typeof(WriteableAttribute), false));
		}

		/// <summary>
		/// Add all the fields from a type
		/// </summary>
		public void Add(Type t) {
			processFields(t, false);
		}

		/// <summary>
		/// Add a field to the form by name
		/// </summary>
		public FieldAttribute Add(Type t, string name) {
			// Name may be "fieldname/heading"
			string[] parts = name.Split('/');
			FieldInfo fld = t.GetField(parts[0]);
			FieldAttribute f;
			if (fld == null) {
				PropertyInfo p = t.GetProperty(parts[0]);
				Utils.Check(p != null, "Field {0} not found in type {1}", parts[0], t.Name);
				bool readwrite = p.SetMethod != null && readWriteFlagForTable(p.DeclaringType, p.IsDefined(typeof(WriteableAttribute)));
				f = FieldAttribute.FieldFor(p, readwrite);
			} else {
				f = FieldAttribute.FieldFor(Module.Database, fld, readWriteFlagForTable(fld.DeclaringType, fld.IsDefined(typeof(WriteableAttribute))));
			}
			if (f != null) {
				if (parts.Length > 1)
					f.Heading = parts[1];
				else if (string.IsNullOrEmpty(f.Heading) && Options["table"] != null && f.FieldName.StartsWith(Options.AsString("table")))  // If name starts with table name, remove table name from heading
					f.Heading = f.FieldName.Substring(Options.AsString("table").Length);
				columns.Add(f.Options);
			}
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
			foreach (FieldAttribute f in Fields) {
				if (f.FieldName == name)
					return i;
				i++;
			}
			return -1;
		}

		void setTableName(Type t) {
			Type table = t;
			Options["table"] = t.Name;
			while (table != typeof(JsonObject)) {
				if (table.IsDefined(typeof(TableAttribute), false)) {
					Options["table"] = table.Name;
					Options["id"] = Module.Database.TableFor(table.Name).PrimaryKey.Name;
					break;
				}
				table = table.BaseType;
			}
		}

		/// <summary>
		/// Add all the suitable fields from a C# type to the form
		/// </summary>
		public void Build(Type t) {
			setTableName(t);
			processFields(t, false);
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

		/// <summary>
		/// Remove the named fields
		/// </summary>
		public void Remove(params string[] names) {
			foreach (string name in names)
				Remove(name);
		}

		/// <summary>
		/// Render the form to the web page, using the appropriate template
		/// </summary>
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
		/// Whether the user can delete record.
		/// </summary>
		public bool CanDelete {
			get { return Options.AsBool("canDelete"); }
			set {
				if (!Module.HasAccess(Module.Info, Module.Method + "delete", out _))
					value = false;
				Options["canDelete"] = value;
			}
		}

		/// <summary>
		/// Process all the fields from a type (do any base classes first)
		/// </summary>
		/// <param name="tbl">Type being analysed</param>
		/// <param name="inTable">True if this is a base class of a Table or Writeable object</param>
		void processFields(Type tbl, bool inTable) {
			inTable |= tbl.IsDefined(typeof(TableAttribute), false) || tbl.IsDefined(typeof(WriteableAttribute), false);
			if (tbl.BaseType != typeof(JsonObject)) // Process base types first
				processFields(tbl.BaseType, inTable);
			bool readwrite = ReadWrite && inTable;
			foreach (FieldInfo field in tbl.GetFieldsInOrder(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) {
				FieldAttribute f = FieldAttribute.FieldFor(Module.Database, field, readwrite || (ReadWrite && field.IsDefined(typeof(WriteableAttribute))));
				if (f != null && RequireField(f))
					columns.Add(f.Options);
			}
		}

		/// <summary>
		/// Check a condition is true on a field. Throw a FormException with the field page number if not.
		/// </summary>
		public void Check(string fieldName, bool condition, string error) {
			if (!condition) {
				FieldAttribute f = this[fieldName];
				throw new FormException(error, f == null ? 0 : f.Page);
			}
		}
	}

	/// <summary>
	/// Exception thrown by Form.Check assertion function
	/// </summary>
	public class FormException : CheckException {
		/// <summary>
		/// Field page number (0 for none)
		/// </summary>
		public int Page { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public FormException(string message, int page = 0)
			: base(null, message) {
			Page = page;
		}

	}


	/// <summary>
	/// DataTable (jquery) form - always readonly.
	/// </summary>
	public class DataTableForm : Form {

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="module">Creating module</param>
		/// <param name="t">Type to display in the form</param>
		public DataTableForm(AppModule module, Type t)
			: base(module, t, false) {
		}


		/// <summary>
		/// DataTable for C# type t with specific fields in specific order
		/// </summary>
		public DataTableForm(AppModule module, Type t, bool readwrite, params string[] fieldNames)
			: base(module, t, readwrite, fieldNames) {
		}

		/// <summary>
		/// Url to call when the user selects a record.
		/// </summary>
		public string Select {
			get { return Options.AsString("select"); }
			set {
				if (Module.HasAccess(value))
					Options["select"] = value;
			}
		}

		/// <summary>
		/// Render the form to a web page using the appropriate template
		/// </summary>
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

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="module">Creating module</param>
		/// <param name="t">Type to display in the list</param>
		public ListForm(AppModule module, Type t)
			: base(module, t, true) {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="module">Creating module</param>
		/// <param name="t">Type to display in the list</param>
		/// <param name="readWrite">Whether the user can update the data</param>
		public ListForm(AppModule module, Type t, bool readWrite)
			: base(module, t, readWrite) {
		}

		/// <summary>
		/// Constructor for C# type t with specific fields in specific order
		/// </summary>
		public ListForm(AppModule module, Type t, bool readwrite, params string[] fieldNames)
			: base(module, t, readwrite, fieldNames) {
		}

		/// <summary>
		/// Url to call when the user selects a record.
		/// </summary>
		public string Select {
			get { return Options.AsString("select"); }
			set {
				if (Module.HasAccess(value))
					Options["select"] = value;
			}
		}

		/// <summary>
		/// Page to display this form on, if multi-page
		/// </summary>
		public int Page {
			get { return Options.AsInt("page"); }
			set { Options["page"] = value; }
		}

		/// <summary>
		/// Render the form to a web page using the appropriate template
		/// </summary>
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

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="module">Owning module</param>
		/// <param name="header">Header form</param>
		/// <param name="detail">Detail form</param>
		public HeaderDetailForm(AppModule module, Form header, ListForm detail)
		: base(module) {
			Header = header;
			Detail = detail;
			Options["header"] = Header.Options;
			Options["detail"] = Detail.Options;
		}

		/// <summary>
		/// The header Form
		/// </summary>
		public Form Header;

		/// <summary>
		/// The Detail ListForm
		/// </summary>
		public ListForm Detail;

		/// <summary>
		/// Render the form to a web page using the appropriate template
		/// </summary>
		public override void Show() {
			Show("HeaderDetailForm");
		}

		/// <summary>
		/// Whether the user can delete record.
		/// </summary>
		public bool CanDelete {
			get { return Header.CanDelete; }
			set { Header.CanDelete = value; }
		}


	}

	/// <summary>
	/// Header + multi detail form
	/// </summary>
	public class MultiDetailForm : BaseForm {

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="module">Owning module</param>
		/// <param name="header">C# type in the header</param>
		/// <param name="details">C# type in the detail</param>
		public MultiDetailForm(AppModule module, Type header, params Type[] details)
		: base(module) {
			Header = new Form(module, header);
			Details = details.Select(d => new ListForm(module, d)).ToArray();
			Options["header"] = Header.Options;
			Options["detail"] = new JArray(Details.Select(d => d.Options));
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="module">Owning module</param>
		/// <param name="header">Header form</param>
		/// <param name="details">Detail form</param>
		public MultiDetailForm(AppModule module, Form header, params ListForm[] details)
		: base(module) {
			Header = header;
			Details = details;
			Options["header"] = Header.Options;
			Options["detail"] = new JArray(Details.Select(d => d.Options));
		}

		/// <summary>
		/// The header Form
		/// </summary>
		public Form Header;

		/// <summary>
		/// The Detail ListForm
		/// </summary>
		public ListForm[] Details;

		/// <summary>
		/// Render the form to a web page using the appropriate template
		/// </summary>
		public override void Show() {
			Show("MultiDetailForm");
		}

		/// <summary>
		/// Whether the user can delete record.
		/// </summary>
		public bool CanDelete {
			get { return Header.CanDelete; }
			set { Header.CanDelete = value; }
		}

	}

	/// <summary>
	/// Old-style FORM/SUBMIT form
	/// </summary>
	public class DumbForm : Form {

		/// <summary>
		/// Empty form
		/// </summary>
		/// <param name="module">Owning module</param>
		/// <param name="readwrite">Whether the user can input to some of the fields</param>
		public DumbForm(AppModule module, bool readwrite) : base(module, readwrite) {
		}

		/// <summary>
		/// Readwrite form for C# type t
		/// </summary>
		public DumbForm(AppModule module, Type t)
			: base(module, t) {
		}

		/// <summary>
		/// Form for C# type t
		/// </summary>
		public DumbForm(AppModule module, Type t, bool readwrite)
			: base(module, t, readwrite) {
		}

		/// <summary>
		/// Form for C# type t with specific fields in specific order
		/// </summary>
		public DumbForm(AppModule module, Type t, bool readwrite, params string[] fieldNames)
			: base(module, t, readwrite, fieldNames) {
		}

		/// <summary>
		/// Build the form html from a template. By default it uses /modulename/methodname.tmpl, but, if that doesn't
		/// exist, it uses the default template for the form (/dumbform.tmpl).
		/// </summary>
		public override void Show() {
			Show("DumbForm");
		}

	}

	/// <summary>
	/// Class to check form fields
	/// </summary>
	public class FormChecker {
		Type Type;
		int Page;
		Dictionary<string, int> Pages;
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="type">Table JsonObject being checked</param>
		/// <param name="page">Page number to use if he whole form is one 1 page (so we don't need to check the fields)</param>
		public FormChecker(Type type, int page = -1) {
			Type = type;
			Page = page;
			if (page < 0)
				Pages = new Dictionary<string, int>();
		}


		void check<T>(T value, Func<T, bool> validate, Func<string> error, string expression) {
			if (!validate(value)) {
				int p = Page;
				if (p < 0) {
					p = 0;
					if (!string.IsNullOrEmpty(expression)) {
						string member = expression.Split('.').Last();
						if (!Pages.TryGetValue(member, out p)) {
							// Retrieve page from type and add to dictionary
							FieldAttribute f = null;
							FieldInfo fld = Type.GetField(member);
							if (fld == null) {
								PropertyInfo prop = Type.GetProperty(member);
								if (prop != null)
									f = prop.GetCustomAttribute<FieldAttribute>();
							} else {
								f = fld.GetCustomAttribute<FieldAttribute>();
							}
							if (f != null)
								p = f.Page;
							Pages[member] = p;
						}
					}
				}
				throw new FormException(error(), p);
			}
		}

		/// <summary>
		/// Check a condition is true on a field. Throw a FormException with the field page number if not.
		/// </summary>
		/// <param name="value">The member item to check</param>
		/// <param name="validate">A validation fnction to call, passing the value</param>
		/// <param name="error">A method to call which returns the error message</param>
		/// <param name="expression">Filled in automatically by the compiler</param>
		public void Check<T>(T value, Func<T, bool> validate, Func<string> error, [CallerArgumentExpression(nameof(value))] string expression = null) {
			check(value, validate, error, expression);
		}

		/// <summary>
		/// Check a condition is true on a field. Throw a FormException with the field page number if not.
		/// </summary>
		/// <param name="value">The member item to check</param>
		/// <param name="validate">A validation fnction to call, passing the value</param>
		/// <param name="error">The error message string</param>
		/// <param name="expression">Filled in automatically by the compiler</param>
		public void Check<T>(T value, Func<T, bool> validate, string error, [CallerArgumentExpression(nameof(value))] string expression = null) {
			check(value, validate, () => error, expression);
		}

		/// <summary>
		/// Check a condition is true on a field. Throw a FormException with the field page number if not.
		/// </summary>
		/// <param name="value">The member item to check</param>
		/// <param name="condition">Whether the item is valis</param>
		/// <param name="error">The error message string</param>
		/// <param name="expression">Filled in automatically by the compiler</param>
		public void Check<T>(T value, bool condition, string error, [CallerArgumentExpression(nameof(value))] string expression = null) {
			check(value, v => condition, () => error, expression);
		}
	}
}

namespace System.Runtime.CompilerServices {
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	internal sealed class CallerArgumentExpressionAttribute : Attribute {
		public CallerArgumentExpressionAttribute(string parameterName) {
			ParameterName = parameterName;
		}

		public string ParameterName { get; }
	}
}

