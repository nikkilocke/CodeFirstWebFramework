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
	public class UniqueAttribute : Attribute {

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
		public UniqueAttribute(string name, int sequence) {
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
		public ForeignKeyAttribute(string table) {
			Table = table;
		}

		/// <summary>
		/// The foreign table
		/// </summary>
		public string Table { get; private set; }
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

}
