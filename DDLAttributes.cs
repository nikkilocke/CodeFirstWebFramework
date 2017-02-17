using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CodeFirstWebFramework {
	public class TableAttribute : Attribute {
	}

	public class ViewAttribute : Attribute {
		public ViewAttribute(string sql) {
			Sql = sql;
		}

		public string Sql;
	}

	public class UniqueAttribute : Attribute {

		public UniqueAttribute(string name)
			: this(name, 0) {
		}

		public UniqueAttribute(string name, int sequence) {
			Name = name;
			Sequence = sequence;
		}

		public string Name { get; private set; }

		public int Sequence { get; private set; }
	}

	public class PrimaryAttribute : Attribute {

		public PrimaryAttribute()
			: this(0) {
		}

		public PrimaryAttribute(int sequence) {
			Name = "PRIMARY";
			Sequence = sequence;
		}

		public bool AutoIncrement = true;

		public string Name { get; private set; }

		public int Sequence { get; private set; }
	}

	public class ForeignKeyAttribute : Attribute {
		public ForeignKeyAttribute(string table) {
			Table = table;
		}

		public string Table { get; private set; }
	}

	public class NullableAttribute : Attribute {
	}

	public class LengthAttribute : Attribute {
		public LengthAttribute(int length)
			: this(length, 0) {
		}

		public LengthAttribute(int length, int precision) {
			Length = length;
			Precision = precision;
		}

		public int Length;

		public int Precision;
	}

	public class DefaultValueAttribute : Attribute {
		public DefaultValueAttribute(string value) {
			Value = value;
		}

		public DefaultValueAttribute(int value) {
			Value = value.ToString();
		}

		public DefaultValueAttribute(bool value) {
			Value = value ? "1" : "0";
		}

		public string Value;
	}

	public class DoNotStoreAttribute : Attribute {
	}

}
