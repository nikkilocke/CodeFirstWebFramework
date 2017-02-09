using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace CodeFirstWebFramework {
	public class ModuleDef {
		Dictionary<string, Type> appModules;	// List of all AppModule types by name ("Module" stripped off end)
		Dictionary<string, Table> _tables;
		Dictionary<Field, ForeignKeyAttribute> _foreignKeys;

		public Assembly Assembly { get; private set; }

		public string Name;

		public ModuleDef(string name) {
			Name = name;
			Assembly = Assembly.Load(name);
			appModules = new Dictionary<string, Type>();
			var baseType = typeof(AppModule);
			foreach (Type t in Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(baseType))) {
				string n = t.Name.ToLower();
				if (n.EndsWith("module"))
					n = n.Substring(0, n.Length - 6);
				appModules[n] = t;
			}
			_tables = new Dictionary<string, Table>();
			baseType = typeof(JsonObject);
			_foreignKeys = new Dictionary<Field, ForeignKeyAttribute>();
			// Process all subclasses of JsonObject with Table attribute in module assembly
			foreach (Type tbl in Assembly.GetTypes().Where(t => t.IsSubclassOf(baseType))) {
				if (!tbl.IsDefined(typeof(TableAttribute)))
					continue;
				processTable(tbl, null);
			}
			// Add any tables defined in the framework module, but not in the given module
			if (Assembly.Name() != baseType.Assembly.Name()) {
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
				fld.ForeignKey = new ForeignKey(tbl, tbl.Fields[0]);
			}
			// Now do the Views (we assume no views in the framework module)
			foreach (Type tbl in Assembly.GetTypes().Where(t => t.IsSubclassOf(baseType))) {
				ViewAttribute view = tbl.GetCustomAttribute<ViewAttribute>();
				if (view == null)
					continue;
				processTable(tbl, view);
			}
			_foreignKeys = null;
		}

		public Type GetDatabase() {
			return Assembly.GetType(Assembly.Name() + ".Database");
		}

		/// <summary>
		/// Get the AppModule for a module name from the url
		/// </summary>
		public Type GetModule(string name) {
			name = name.ToLower();
			return appModules.ContainsKey(name) ? appModules[name] : null;
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
				if (field.IsDefined(typeof(DoNotStoreAttribute)))
					continue;
				bool nullable = field.IsDefined(typeof(NullableAttribute));
				Type pt = field.FieldType;
				decimal length = 0;
				string defaultValue = null;
				if (pt == typeof(bool?)) {
					pt = typeof(bool);
					nullable = true;
				} else if (pt == typeof(int?)) {
					pt = typeof(int);
					nullable = true;
				} else if (pt == typeof(decimal?)) {
					pt = typeof(decimal);
					nullable = true;
				} else if (pt == typeof(double?)) {
					pt = typeof(double);
					nullable = true;
				} else if (pt == typeof(DateTime?)) {
					pt = typeof(DateTime);
					nullable = true;
				}
				PrimaryAttribute pk = field.GetCustomAttribute<PrimaryAttribute>();
				if (pk != null)
					nullable = false;
				if (pt == typeof(bool)) {
					length = 1;
					defaultValue = "0";
				} else if (pt == typeof(int)) {
					length = 11;
					defaultValue = "0";
				} else if (pt == typeof(decimal)) {
					length = 10.2M;
					defaultValue = "0.00";
				} else if (pt == typeof(double)) {
					length = 10.4M;
					defaultValue = "0";
				} else if (pt == typeof(string)) {
					length = 45;
					defaultValue = "";
				}
				if (nullable)
					defaultValue = null;
				LengthAttribute la = field.GetCustomAttribute<LengthAttribute>();
				if (la != null)
					length = la.Length + la.Precision / 10M;
				DefaultValueAttribute da = field.GetCustomAttribute<DefaultValueAttribute>();
				if (da != null)
					defaultValue = da.Value;
				Field fld = new Field(field.Name, pt, length, nullable, pk != null && pk.AutoIncrement, defaultValue);
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
