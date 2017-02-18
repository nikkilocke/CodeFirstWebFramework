using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace CodeFirstWebFramework {
	public class Namespace {
		Dictionary<string, Type> appModules;	// List of all AppModule types by name ("Module" stripped off end)
		Dictionary<string, Table> tables;
		Dictionary<Field, ForeignKeyAttribute> foreignKeys;
		List<Assembly> assemblies;

		public string Name { get; private set; }

		public Namespace(string name) {
			Name = name;
			appModules = new Dictionary<string, Type>();
			assemblies = new List<Assembly>();
			tables = new Dictionary<string, Table>();
			foreignKeys = new Dictionary<Field, ForeignKeyAttribute>();
			List<Type> views = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				bool relevant = false;
				foreach (Type t in assembly.GetTypes().Where(t => t.Namespace == name && !t.IsAbstract)) {
					relevant = true;
					if (t.IsSubclassOf(typeof(AppModule))) {
						string n = t.Name.ToLower();
						if (n.EndsWith("module"))
							n = n.Substring(0, n.Length - 6);
						appModules[n] = t;
					}
					// Process all subclasses of JsonObject with Table attribute in module assembly
					if (t.IsSubclassOf(typeof(JsonObject))) {
						if (t.IsDefined(typeof(TableAttribute))) {
							processTable(t, null);
						} else if (t.IsDefined(typeof(ViewAttribute))) {
							views.Add(t);
						}
					}
				}
				if (relevant)
					assemblies.Add(assembly);
			}
			// Add any tables defined in the framework module, but not in the given module
			foreach (Type tbl in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(JsonObject)))) {
				if (!tbl.IsDefined(typeof(TableAttribute)) || tables.ContainsKey(tbl.Name))
					continue;
				processTable(tbl, null);
			}
			// Populate the foreign key attributes
			foreach (Field fld in foreignKeys.Keys) {
				ForeignKeyAttribute fk = foreignKeys[fld];
				Table tbl = TableFor(fk.Table);
				fld.ForeignKey = new ForeignKey(tbl, tbl.Fields[0]);
			}
			// Now do the Views (we assume no views in the framework module)
			foreach (Type tbl in views) {
				processTable(tbl, tbl.GetCustomAttribute<ViewAttribute>());
			}
			foreignKeys = null;
		}

		public Type GetDatabase() {
			foreach (Assembly assembly in assemblies) {
				foreach (Type t in assembly.GetTypes().Where(t => t.Namespace == Name && !t.IsAbstract && t.IsSubclassOf(typeof(Database)))) {
					return t;
				}
			}
			return typeof(Database);
		}

		/// <summary>
		/// Get the AppModule for a module name from the url
		/// </summary>
		public Type GetModule(string name) {
			name = name.ToLower();
			return appModules.ContainsKey(name) ? appModules[name] : null;
		}

		public Dictionary<string, Table> Tables {
			get { return new Dictionary<string, Table>(tables); }
		}

		public IEnumerable<string> TableNames {
			get { return tables.Where(t => !t.Value.IsView).Select(t => t.Key); }
		}

		public IEnumerable<string> ViewNames {
			get { return tables.Where(t => t.Value.IsView).Select(t => t.Key); }
		}

		public Table TableFor(string name) {
			Table table;
			Utils.Check(tables.TryGetValue(name, out table), "Table '{0}' does not exist", name);
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
				// If View is based on a Table class, use that as the update table
				for (Type t = tbl.BaseType; t != typeof(Object); t = t.BaseType) {
					if(t.IsDefined(typeof(TableAttribute))) {
						if(tables.TryGetValue(t.Name, out updateTable))
							break;
					}
				}
				if(updateTable == null)
					tables.TryGetValue(Regex.Replace(tbl.Name, "^.*_", ""), out updateTable);
				tables[tbl.Name] = new View(tbl.Name, fields.ToArray(), inds.ToArray(), view.Sql, updateTable);
			} else {
				tables[tbl.Name] = new Table(tbl.Name, fields.ToArray(), inds.ToArray());
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
					foreignKeys[fld] = fk;
				fields.Add(fld);
			}
		}
	}
}
