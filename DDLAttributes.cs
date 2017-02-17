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

	public class CodeFirst {
		Dictionary<string, Table> _tables;
		Dictionary<Field, ForeignKeyAttribute> _foreignKeys;

		public CodeFirst(ModuleDef module) {
			_tables = new Dictionary<string, Table>();
			var baseType = typeof(JsonObject);
			var assembly = module.Assembly;
			_foreignKeys = new Dictionary<Field, ForeignKeyAttribute>();
			// Process all subclasses of JsonObject with Table attribute in module assembly
			foreach (Type tbl in assembly.GetTypes().Where(t => t.IsSubclassOf(baseType))) {
				if (!tbl.IsDefined(typeof(TableAttribute)))
					continue;
				processTable(tbl, null);
			}
			// Add any tables defined in the framework module, but not in the given module
			if (assembly.Name() != baseType.Assembly.Name()) {
				foreach (Type tbl in baseType.Assembly.GetTypes().Where(t => t.IsSubclassOf(baseType))) {
					if (!tbl.IsDefined(typeof(TableAttribute)) || _tables.ContainsKey(tbl.Name))
						continue;
					processTable(tbl, null);
				}
			}
			// Populate the foreign key attributes
			foreach (Field fld in _foreignKeys.Keys) {
				ForeignKeyAttribute fk = _foreignKeys[fld];
				Table tbl = TableFor(fk.Table);
				fld.ForeignKey = new ForeignKey(tbl, tbl.PrimaryKey);
			}
			// Now do the Views (we assume no views in the framework module)
			foreach (Type tbl in assembly.GetTypes().Where(t => t.IsSubclassOf(baseType))) {
				ViewAttribute view = tbl.GetCustomAttribute<ViewAttribute>();
				if (view == null)
					continue;
				processTable(tbl, view);
			}
			_foreignKeys = null;
		}

		public Dictionary<string, Table> Tables {
			get { return new Dictionary<string, Table>(_tables); }
		}

		public IEnumerable<string> TableNames {
			get { return _tables.Where(t => !t.Value.IsView).Select(t => t.Key); }
		}

		public IEnumerable<string> ViewNames {
			get { return _tables.Where(t => t.Value.IsView).Select(t => t.Key); }
		}

		public Table TableFor(string name) {
			Table table;
			Utils.Check(_tables.TryGetValue(name, out table), "Table '{0}' does not exist", name);
			return table;
		}

		void processTable(Type tbl, ViewAttribute view) {
			List<Field> fields = new List<Field>();
			Dictionary<string, List<Tuple<int, Field>>> indexes = new Dictionary<string, List<Tuple<int, Field>>>();
			List<Tuple<int, Field>> primary = new List<Tuple<int, Field>>();
			string primaryName = null;
			processFields(tbl, ref fields, ref indexes, ref primary, ref primaryName);
			if (primary.Count == 0) {
				primary.Add(new Tuple<int, Field>(0, fields[0]));
				primaryName = "PRIMARY";
			}
			List<Index> inds = new List<Index>(indexes.Keys
				.OrderBy(k => k)
				.Select(k => new Index(k, indexes[k]
					.OrderBy(i => i.Item1)
					.Select(i => i.Item2)
					.ToArray())));
			inds.Insert(0, new Index(primaryName, primary
					.OrderBy(i => i.Item1)
					.Select(i => i.Item2)
					.ToArray()));
			if (view != null) {
				Table updateTable = null;
				_tables.TryGetValue(Regex.Replace(tbl.Name, "^.*_", ""), out updateTable);
				_tables[tbl.Name] = new View(tbl.Name, fields.ToArray(), inds.ToArray(), view.Sql, updateTable);
			} else {
				_tables[tbl.Name] = new Table(tbl.Name, fields.ToArray(), inds.ToArray());
			}
		}

		void processFields(Type tbl, ref List<Field> fields, ref Dictionary<string, List<Tuple<int, Field>>> indexes, ref List<Tuple<int, Field>> primary, ref string primaryName) {
			if (tbl.BaseType != typeof(JsonObject))	// Process base types first
				processFields(tbl.BaseType, ref fields, ref indexes, ref primary, ref primaryName);
			foreach (FieldInfo field in tbl.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) {
				PrimaryAttribute pk;
				Field fld = Field.FieldFor(field, out pk);
				if (fld == null)
					continue;
				if (pk != null) {
					primary.Add(new Tuple<int, Field>(pk.Sequence, fld));
					Utils.Check(primaryName == null || primaryName == pk.Name, "2 Primary keys defined on {0}", tbl.Name);
					primaryName = pk.Name;
				}
				foreach (UniqueAttribute a in field.GetCustomAttributes<UniqueAttribute>()) {
					List<Tuple<int, Field>> index;
					if (!indexes.TryGetValue(a.Name, out index)) {
						index = new List<Tuple<int, Field>>();
						indexes[a.Name] = index;
					}
					index.Add(new Tuple<int, Field>(a.Sequence, fld));
				}
				ForeignKeyAttribute fk = field.GetCustomAttribute<ForeignKeyAttribute>();
				if (fk != null)
					_foreignKeys[fld] = fk;
				fields.Add(fld);
			}
		}

	}

}
