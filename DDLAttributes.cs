using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CodeFirstWebFramework {
	/// <summary>
	/// Mark a C# class as a database table
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TableAttribute : Attribute {
	}

	/// <summary>
	/// Mark a C# class as a database view
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ViewAttribute : Attribute {
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sql">SQL to generate the view data</param>
		public ViewAttribute(string sql) {
			Sql = sql;
		}

		/// <summary>
		/// SWL to generate the view data
		/// </summary>
		public string Sql;
	}

	/// <summary>
	/// Mark a field as part of a unique index
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class IndexAttribute : Attribute {

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Index name</param>
		public IndexAttribute(string name)
			: this(name, 0) {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Index name</param>
		/// <param name="sequence">Sequence when multiple fields make up the index</param>
		public IndexAttribute(string name, int sequence) {
			Name = name;
			Sequence = sequence;
		}

		/// <summary>
		/// Index name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Sequence when multiple fields make up the index
		/// </summary>
		public int Sequence { get; private set; }

		/// <summary>
		/// Whether this is a Unique index
		/// </summary>
		public bool Unique { get; protected set; }
	}

	/// <summary>
	/// Mark a field as part of a unique index
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class UniqueAttribute : IndexAttribute {
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Index name</param>
		public UniqueAttribute(string name)
			: this(name, 0) {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Index name</param>
		/// <param name="sequence">Sequence when multiple fields make up the index</param>
		public UniqueAttribute(string name, int sequence) : base(name, sequence) {
			Unique = true;
		}

	}

	/// <summary>
	/// Mark a field as the primary key
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class PrimaryAttribute : Attribute {

		/// <summary>
		/// Constructor - sets AutoIncrement to true by default, and Name to "PRIMARY"
		/// </summary>
		public PrimaryAttribute()
			: this(0) {
		}

		/// <summary>
		/// Constructor for when multiple fields make up the key
		/// </summary>
		/// <param name="sequence">Which order the fields are in</param>
		public PrimaryAttribute(int sequence) {
			Name = "PRIMARY";
			Sequence = sequence;
		}

		/// <summary>
		/// Whether the key is AutoIncrement (should only be applied to integer keys, usually the record id)
		/// </summary>
		public bool AutoIncrement = true;

		/// <summary>
		/// Key Name ("PRIMARY" by default)
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Sequence when multiple fields make up the index
		/// </summary>
		public int Sequence { get; private set; }
	}

	/// <summary>
	/// Attribute marking a field as a foreign key
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ForeignKeyAttribute : Attribute {
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="table">The foreign table</param>
		/// <param name="fieldName">The name of the field to use as the text value in select options, or NoAutoSelect</param>
		public ForeignKeyAttribute(string table, string fieldName = null) {
			Table = table;
			FieldName = fieldName;
		}

		/// <summary>
		/// The foreign table
		/// </summary>
		public string Table { get; private set; }
		/// <summary>
		/// The name of the field to use as the text value in select options
		/// </summary>
		public string FieldName { get; private set; }

		/// <summary>
		/// Set Fieldname to this to turn off AutoSelect
		/// </summary>
		public const string NoAutoSelect = "-";

		/// <summary>
		/// Whether ForeignKey fields automatically create an AutoSelect on the first key if no FieldName field is specified
		/// </summary>
		public static bool AutoSelect = true;

	}

	/// <summary>
	/// Mark a field as allowed to be null
	/// </summary>
	public class NullableAttribute : Attribute {
	}

	/// <summary>
	/// Set the length (and optionally precision) of a field in the database
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class LengthAttribute : Attribute {
		/// <summary>
		/// Constructor
		/// </summary>
		public LengthAttribute(int length)
			: this(length, 0) {
		}

		/// <summary>
		/// Constructor (with precision)
		/// </summary>
		public LengthAttribute(int length, int precision) {
			Length = length;
			Precision = precision;
		}

		/// <summary>
		/// Field length
		/// </summary>
		public int Length;

		/// <summary>
		/// Field precision (if required)
		/// </summary>
		public int Precision;
	}

	/// <summary>
	/// Set the default value for a field (on the database)
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class DefaultValueAttribute : Attribute {
		/// <summary>
		/// Constructor
		/// </summary>
		public DefaultValueAttribute(string value) {
			Value = value;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public DefaultValueAttribute(int value) {
			Value = value.ToString();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public DefaultValueAttribute(bool value) {
			Value = value ? "1" : "0";
		}

		/// <summary>
		/// The default value
		/// </summary>
		public string Value;
	}

	/// <summary>
	/// Mark a C# field which is not to be stored in the database
	/// </summary>
	[AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
	public class DoNotStoreAttribute : Attribute {
	}

	/// <summary>
	/// Attribute which indicates an AppModule class uses a helper class 
	/// to implement some or all of its methods.
	/// The helper class must have a constructor which takes a single AppModule parameter.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ImplementationAttribute : Attribute {
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="helperClass">Type of helper class. 
		/// It must have a constructor which takes a single AppModule parameter.</param>
		public ImplementationAttribute(Type helperClass) {
			Helper = helperClass;
		}

		/// <summary>
		/// The helper class type.
		/// </summary>
		public Type Helper { get; private set; }
	}

}
