using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Security.Principal;
using MySqlX.XDevAPI;

namespace CodeFirstWebFramework {

	/// <summary>
	/// Class to hold a module name, for use in templates
	/// </summary>
	public class ModuleInfo {
		Namespace _namespace;

		/// <summary>
		/// Constructor
		/// </summary>
		public ModuleInfo(string name, Type t, Namespace space) {
			Name = name;
			Type = t;
			_namespace = space;
			Auth = t.GetCustomAttribute<AuthAttribute>(true);
			if (Auth == null)
				Auth = new AuthAttribute(AccessLevel.Any);
			if (Auth.Name == null)
				Auth.Name = name;
			Auth.Groups = _namespace.AddAuthGroups(t.GetCustomAttributes<AuthGroupAttribute>());
			AuthMethods = new Dictionary<string, AuthAttribute>(StringComparer.OrdinalIgnoreCase);
			addMethods(t);
			foreach (ImplementationAttribute implementation in t.GetCustomAttributes<ImplementationAttribute>()) {
				addMethods(implementation.Helper);
			}
		}

		/// <summary>
		/// Look for Auth attributes on all the methods of a type, and add them to the dictionary
		/// </summary>
		/// <param name="t"></param>
		void addMethods(Type t) {
			foreach (MethodInfo m in t.GetMethods()) {
				if (!AuthMethods.ContainsKey(m.Name)) {
					int[] groups = _namespace.AddAuthGroups(m.GetCustomAttributes<AuthGroupAttribute>());
					AuthAttribute a = m.GetCustomAttribute<AuthAttribute>(true);
					if (groups.Length > 0 && a == null)
						a = new AuthAttribute(Auth.AccessLevel);
					if (a != null) {
						a.Groups = groups;
						if (a.Name == null)
							a.Name = m.Name;
						AuthMethods[m.Name] = a;
					}
				}
			}
		}

		/// <summary>
		/// Name of module
		/// </summary>
		public string Name;

		/// <summary>
		/// AppModule type
		/// </summary>
		public Type Type;

		/// <summary>
		/// Uncamelled name for display
		/// </summary>
		public string UnCamelName {
			get { return Name.UnCamel(); }
		}

		/// <summary>
		/// The AuthAttribute associated with this module (or one with Any access if there is none)
		/// </summary>
		public AuthAttribute Auth;

		/// <summary>
		/// Auth access level (or AccessLevel.Any)
		/// </summary>
		public int ModuleAccessLevel;

		/// <summary>
		/// Dictionary of method names that have an Auth attribute
		/// </summary>
		public Dictionary<string, AuthAttribute> AuthMethods;

		/// <summary>
		/// Lowest Access level for any method (for determining whether module should be displayed in the menu for a user).
		/// </summary>
		int _lowestAccessLevel;

		/// <summary>
		/// Lowest access levels for each group (for determining whether module should be displayed in the menu for a user)
		/// </summary>
		int[] _lowestAccessLevels;

		/// <summary>
		/// Compute the LowestAccessLevel for users without groups
		/// </summary>
		public int GetLowestAccessLevel() {
			_lowestAccessLevel = ModuleAccessLevel;
			foreach (AuthAttribute lvl in AuthMethods.Values) {
				if (lvl.AccessLevel < _lowestAccessLevel)
					_lowestAccessLevel = lvl.AccessLevel;
			}
			return _lowestAccessLevel;
		}

		/// <summary>
		/// Compute the lowest access levels for each group
		/// </summary>
		public void GetLowestAccessLevels() {
			_lowestAccessLevels = Enumerable.Repeat(int.MaxValue, _namespace.AuthGroups.Count).ToArray();
			foreach (AuthAttribute lvl in AuthMethods.Values) {
				foreach (int i in lvl.Groups) {
					if (lvl.AccessLevel < _lowestAccessLevels[i])
						_lowestAccessLevels[i] = lvl.AccessLevel;
				}
			}
		}

		/// <summary>
		/// Whether user can access at least one method in this module
		/// </summary>
		public bool UserHasAccess(AppModule m) {
			if (_lowestAccessLevel == AccessLevel.Any)
				return true;				// Anyone can access module
			if (m.Session.User == null)
				return false;				// User not logged in
			if (m.Session.User.AccessLevel >= _lowestAccessLevel)
				return true;				// User has access level >= module's lowest
			int[] userLevels = m.Session.User.AccessLevels(m);
			if (userLevels == null)			// User does not have separate group permissions
				return false;
			for (int i = 0; i < _lowestAccessLevels.Length; i++)
				if (userLevels[i] >= _lowestAccessLevels[i])
					return true;			// One of the user's group access levels is >= the lowest access level in this module for that group
			return false;
		}

		/// <summary>
		/// Lowest access level for group g
		/// </summary>
		public int LowestAccessLevelForGroup(int g) {
			return _lowestAccessLevels[g];
		}
	}

	/// <summary>
	/// Desribes a namespace with AppModules which makes up a WebApp
	/// </summary>
	public class Namespace {
		Dictionary<string, ModuleInfo> appModules;	// List of all AppModule types by name ("Module" stripped off end)
		Dictionary<string, Table> tables;
		Dictionary<Field, ForeignKeyAttribute> foreignKeys;
		List<Assembly> assemblies;
		Database _db;
		/// <summary>
		/// The dictionary of group names
		/// </summary>
		public Dictionary<string, int> AuthGroups;
		/// <summary>
		/// Backward compatible authorisation grouups by module and method
		/// </summary>
		public bool OldAuth { get; private set; }
		/// <summary>
		/// The Server using this Namespace
		/// </summary>
		public ServerConfig Server;

		/// <summary>
		/// The Database for this Namespace
		/// </summary>
		public virtual Database Database {
			get {
				lock (this) {
					if (_db == null) {
						_db = GetDatabase(Server);
					}
				}
				return _db;
			}
		}

		/// <summary>
		/// List of module names for templates (e.g. to auto-generate a module menu)
		/// </summary>
		public IEnumerable<ModuleInfo> Modules {
			get { return appModules.Values; }
		}

		/// <summary>
		/// Namespace name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Create a Namespace object for the server.
		/// Looks for a class called "Namespace" in the server's Namespace which is a subclass of CodeFirstWebFramework.Namespace, 
		/// and has a constructor accepting a single ServerConfig argument.
		/// If not found, creates a base Namespace object
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public static Namespace Create(ServerConfig server) {
			Type ns = null;
			foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
				// Try and catch is here because LinqPad throws in GetTypes sometimes
				try {
					ns = a.GetTypes().Where(t => t.Namespace == server.Namespace && !t.IsAbstract && t.IsSubclassOf(typeof(Namespace))).FirstOrDefault();
					if (ns != null)
						break;
				} catch {
				}
			}
			if(ns != null) { 
				ConstructorInfo ctor = ns.GetConstructor(new[] { typeof(ServerConfig) });
				if(ctor != null)
					return (Namespace)ctor.Invoke(new object[] { server });
			}
			return new Namespace(server);
		}

		/// <summary>
		/// Constructor - uses reflection to get the information
		/// </summary>
		public Namespace(ServerConfig server) {
			Server = server;
			Name = server.Namespace;
			appModules = new Dictionary<string, ModuleInfo>();
			assemblies = new List<Assembly>();
			tables = new Dictionary<string, Table>();
			foreignKeys = new Dictionary<Field, ForeignKeyAttribute>();
			AuthGroups = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			List<Type> views = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				bool relevant = false;
				// Try and catch is here because LinqPad throws in GetTypes sometimes
				try {
					foreach (Type t in assembly.GetTypes().Where(t => t.Namespace == Name && !t.IsAbstract)) {
						relevant = true;
						if (t.IsSubclassOf(typeof(AppModule))) {
							string n = t.Name;
							if (n.EndsWith("Module"))
								n = n.Substring(0, n.Length - 6);
							appModules[n.ToLower()] = new ModuleInfo(n, t, this);
						}
						// Process all subclasses of JsonObject with Table attribute in module assembly
						if (t.IsSubclassOf(typeof(JsonObject))) {
							if (t.IsDefined(typeof(TableAttribute), false)) {
								processTable(t, null);
							} else if (t.IsDefined(typeof(ViewAttribute), false)) {
								views.Add(t);
							}
						}
					}
					if (relevant)
						assemblies.Add(assembly);
				} catch {
				}
			}
			// Add any tables defined in the framework module, but not in the given module
			// Try and catch is here because LinqPad throws in GetTypes sometimes
			try {
				foreach (Type tbl in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(JsonObject)))) {
					if (!tbl.IsDefined(typeof(TableAttribute), false) || tables.ContainsKey(tbl.Name))
						continue;
					processTable(tbl, null);
				}
			} catch {
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
			FileSystem = GetInstanceOf<FileSystem>();
			// Backwards compatibility - if no AuthGroups, then create groups equivalent to the modules and methods with Auth attributes
			if (AuthGroups.Count == 0)
				OldAuth = true;
			foreach(string key in appModules.Keys) {
				ModuleInfo m = appModules[key];
				int lowest = m.GetLowestAccessLevel();
				if (!OldAuth || m.Auth.Hide)
					continue;
				if (m.Auth.AccessLevel == AccessLevel.Any && lowest == AccessLevel.Any)
					continue;
				m.Auth.Groups = AddAuthGroup(m.Auth.Name + ":-");
				foreach(AuthAttribute a in m.AuthMethods.Values) {
					if (a.Hide)
						continue;
					a.Groups = AddAuthGroup(m.Auth.Name + ":" + a.Name);
				}
			}
			// Now compute the minimum required access level for each group in each module
			foreach(ModuleInfo m in appModules.Values) {
				m.GetLowestAccessLevels();
			}
		}

		/// <summary>
		/// Add the AuthGroups from the list
		/// </summary>
		/// <returns>HashSet of added group numbers</returns>
		public int[] AddAuthGroups(IEnumerable<AuthGroupAttribute> groups) {
			return AddAuthGroups(groups.Select(a => a.Name));
		}

		/// <summary>
		/// Add the given AuthGroup
		/// </summary>
		/// <returns>HashSet containing the added group number</returns>
		public int[] AddAuthGroup(string group) {
			return AddAuthGroups(Enumerable.Repeat(group, 1));
		}

		/// <summary>
		/// Add the AuthGroups from the list
		/// </summary>
		/// <returns>HashSet of added group numbers</returns>
		public int[] AddAuthGroups(IEnumerable<string> groups) {
			HashSet<int> result = new HashSet<int>();
			foreach (string grp in groups) {
				if (!AuthGroups.TryGetValue(grp, out int r))
					AuthGroups[grp] = r = AuthGroups.Count;
				result.Add(r);
			}
			return result.ToArray();
		}

		WebServer.Session _empty;

		/// <summary>
		/// Create an empty Session object of the appropriate type for this Namespace
		/// </summary>
		public WebServer.Session EmptySession {
			get {
				lock (this) {
					if (_empty == null)
						_empty = GetInstanceOf<WebServer.Session>();
					return _empty;
				}
			}
		}

		/// <summary>
		/// The FileSystem for this Namespace
		/// </summary>
		public FileSystem FileSystem;

		/// <summary>
		/// If there is a subclass of baseType in the namespace, returns that, otherwise baseType
		/// </summary>
		public Type GetNamespaceType(Type baseClass) {
			foreach (Assembly assembly in assemblies) {
				foreach (Type t in assembly.GetTypes().Where(t => t.Namespace == Name && !t.IsAbstract && t.IsSubclassOf(baseClass))) {
					return t;
				}
			}
			return baseClass;
		}

		/// <summary>
		/// If there is a subclass of T in the namespace, create an instance of it.
		/// Otherwise create an instance of T
		/// </summary>
		/// <typeparam name="T">The class to create</typeparam>
		/// <param name="args">The constructor arguments</param>
		public T GetInstanceOf<T>(params object [] args) {
			return (T)Activator.CreateInstance(GetNamespaceType(typeof(T)), args);
		}

		/// <summary>
		/// Returns the Database object to use.
		/// If there is one in the namespace, returns an instance of that, otherwise an instance of CodeFirstWebFramework.Database
		/// </summary>
		/// <param name="server">ConfigServer to pass to the database constructor</param>
		public Database GetDatabase(ServerConfig server) {
			return GetInstanceOf<Database>(server);
		}

		/// <summary>
		/// Returns the AccessLevel object to use.
		/// If there is one in the namespace, returns an instance of that, otherwise an instance of CodeFirstWebFramework.AccessLevel
		/// </summary>
		public AccessLevel GetAccessLevel() {
			return GetInstanceOf<AccessLevel>();
		}

		/// <summary>
		/// Get the AppModule for a module name from the url
		/// </summary>
		public ModuleInfo GetModuleInfo(string name) {
			return appModules.TryGetValue(name.ToLower(), out ModuleInfo m) ? m : null;
		}

		/// <summary>
		/// Parse a uri and return the ModuleInfo associated with it (or null if none).
		/// Sets filename to the proper relative filename (modulename/methodname.extension), stripping off VersionSuffix, 
		/// and adding any defaults (home/default if uri is "/", for instance).
		/// </summary>
		virtual public ModuleInfo ParseUri(string uri, out string filename) {
			filename = uri.Split('?', '&')[0];
			filename = filename.Replace(WebServer.VersionSuffix, "");	// Remove VersionSuffix, which is just there to stop caching between versions
			if (filename.StartsWith("/"))
				filename = filename.Substring(1);
			if (filename == "") filename = "home/default";
			string baseName = Regex.Replace(filename, @"\.[^/]*$", ""); // Ignore extension - treat as a program request
																		// Urls of the form /ModuleName[/MethodName][.html] call a C# AppModule
			string[] parts = baseName.Split('/');
			// Urls of the form /ModuleName[/MethodName][.html] call a C# AppModule
			ModuleInfo info = parts.Length <= 2 ? GetModuleInfo(parts[0]) : null;
			if(info != null && parts.Length == 1)
				filename += "/default";
			return info;
		}

		/// <summary>
		/// Dictionary of tables defined in this namespace
		/// </summary>
		public Dictionary<string, Table> Tables {
			get { return new Dictionary<string, Table>(tables); }
		}

		/// <summary>
		/// List of the table names
		/// </summary>
		public IEnumerable<string> TableNames {
			get { return tables.Where(t => !t.Value.IsView).Select(t => t.Key); }
		}

		/// <summary>
		/// List of the view names
		/// </summary>
		public IEnumerable<string> ViewNames {
			get { return tables.Where(t => t.Value.IsView).Select(t => t.Key); }
		}

		/// <summary>
		/// Find a Table object by name - throw if not found
		/// </summary>
		public Table TableFor(string name) {
			Table table;
			Utils.Check(tables.TryGetValue(name, out table), "Table '{0}' does not exist", name);
			return table;
		}

		class IndexDef {
			public IndexDef(string name, bool unique) {
				Name = name;
				Unique = unique;
				Fields = new List<Tuple<int, Field>>();
			}
			public string Name;
			public bool Unique;
			public List<Tuple<int, Field>> Fields;
		}

		/// <summary>
		/// Generate Table or View object for a C# class
		/// </summary>
		void processTable(Type tbl, ViewAttribute view) {
			List<Field> fields = new List<Field>();
			Dictionary<string, IndexDef> indexes = new Dictionary<string, IndexDef>();
			List<Tuple<int, Field>> primary = new List<Tuple<int, Field>>();
			string primaryName = null;
			processFields(tbl, ref fields, ref indexes, ref primary, ref primaryName);
			if (primary.Count == 0) {
				primary.Add(new Tuple<int, Field>(0, fields[0]));
				primaryName = "PRIMARY";
			}
			List<Index> inds = new List<Index>(indexes.Values
				.Select(v => new Index(v.Name, v.Unique, v.Fields
					.OrderBy(i => i.Item1)
					.Select(i => i.Item2)
					.ToArray())));
			inds.Insert(0, new Index(primaryName, true, primary
					.OrderBy(i => i.Item1)
					.Select(i => i.Item2)
					.ToArray()));
			if (view != null) {
				Table updateTable = null;
				// If View is based on a Table class, use that as the update table
				for (Type t = tbl.BaseType; t != typeof(Object); t = t.BaseType) {
					if(t.IsDefined(typeof(TableAttribute), false)) {
						if(tables.TryGetValue(t.Name, out updateTable))
							break;
					}
				}
				if(updateTable == null)
					tables.TryGetValue(Regex.Replace(tbl.Name, "^.*_", ""), out updateTable);
				tables[tbl.Name] = new View(tbl.Name, fields.ToArray(), inds.ToArray(), view.Sql, updateTable) { Type = tbl };
			} else {
				tables[tbl.Name] = new Table(tbl.Name, fields.ToArray(), inds.ToArray()) { Type = tbl };
			}
		}

		/// <summary>
		/// Update the field, index, etc. information for a C# type.
		/// Process base classes first.
		/// </summary>
		void processFields(Type tbl, ref List<Field> fields, ref Dictionary<string, IndexDef> indexes, ref List<Tuple<int, Field>> primary, ref string primaryName) {
			if (tbl.BaseType != typeof(JsonObject))	// Process base types first
				processFields(tbl.BaseType, ref fields, ref indexes, ref primary, ref primaryName);
			foreach (FieldInfo field in tbl.GetFieldsInOrder(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) {
				PrimaryAttribute pk;
				Field fld = Field.FieldFor(field, out pk);
				if (fld == null)
					continue;
				if (pk != null) {
					primary.Add(new Tuple<int, Field>(pk.Sequence, fld));
					Utils.Check(primaryName == null || primaryName == pk.Name, "2 Primary keys defined on {0}", tbl.Name);
					primaryName = pk.Name;
				}
				foreach (IndexAttribute a in field.GetCustomAttributes<IndexAttribute>()) {
					IndexDef index;
					if (!indexes.TryGetValue(a.Name, out index)) {
						index = new IndexDef(a.Name, a.Unique);
						indexes[a.Name] = index;
					}
					index.Fields.Add(new Tuple<int, Field>(a.Sequence, fld));
				}
				ForeignKeyAttribute fk = field.GetCustomAttribute<ForeignKeyAttribute>();
				if (fk != null)
					foreignKeys[fld] = fk;
				fields.Add(fld);
			}
		}
	}
}
