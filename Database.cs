using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	public enum LogLevel {
		None = 0,
		Writes,
		Reads
	};
	public class Database : IDisposable {
		DbInterface db;
		ServerConfig server;
		Dictionary<string, Table> _tables;

		/// <summary>
		/// Make an individual table in the database correspond to the code Table class
		/// </summary>
		/// <param name="code"></param>
		/// <param name="database"></param>
		void upgrade(Table code, Table database) {
			if (database == null) {
				db.CreateTable(code);
				return;
			}
			bool view = code is View;
			if (view != database is View) {
				db.DropTable(database);
				db.CreateTable(code);
				return;
			}
			if (view) {
				bool? result = db.ViewsMatch(code as View, database as View);
				if (result == true)
					return;
				if (result == false) {
					db.DropTable(database);
					db.CreateTable(code);
					return;
				}
			}
			List<Field> insert = new List<Field>();
			List<Field> update = new List<Field>();
			List<Field> remove = new List<Field>();
			List<Field> insertFK = new List<Field>();
			List<Field> dropFK = new List<Field>();
			List<Index> insertIndex = new List<Index>();
			List<Index> dropIndex = new List<Index>();
			foreach (Field f1 in code.Fields) {
				Field f2 = database.FieldFor(f1.Name);
				if (f2 == null)
					insert.Add(f1);
				else {
					if (!db.FieldsMatch(code, f1, f2)) {
						update.Add(f1);
					}
					if (f1.ForeignKey == null) {
						if (f2.ForeignKey != null)
							dropFK.Add(f2);
					} else {
						if (f2.ForeignKey == null)
							insertFK.Add(f1);
						else if (f1.ForeignKey.Table.Name != f2.ForeignKey.Table.Name) {
							dropFK.Add(f2);
							insertFK.Add(f1);
						}
					}
				}
			}
			foreach (Field f2 in database.Fields) {
				if (code.FieldFor(f2.Name) == null) {
					remove.Add(f2);
					if (f2.ForeignKey != null)
						dropFK.Add(f2);
				}
			}
			foreach (Index i1 in code.Indexes) {
				Index i2 = database.Indexes.Where(i => i.FieldList == i1.FieldList).FirstOrDefault();
				if (i2 == null) {
					insertIndex.Add(i1);
				}
			}
			foreach (Index i2 in database.Indexes) {
				if (code.Indexes.Where(i => i.FieldList == i2.FieldList).FirstOrDefault() == null)
					dropIndex.Add(i2);
			}
			if (view) {
				if (insert.Count == 0 && update.Count == 0 && remove.Count == 0)
					return;
				db.DropTable(database);
				db.CreateTable(code);
				return;
			}
			if (insert.Count != 0 || update.Count != 0 || remove.Count != 0 || insertFK.Count != 0 || dropFK.Count != 0 || insertIndex.Count != 0 || dropIndex.Count != 0)
				db.UpgradeTable(code, database, insert, update, remove, insertFK, dropFK, insertIndex, dropIndex);
		}

		/// <summary>
		/// Check the version in the Settings table. If not the same as CurrentDbVersion, call PreUpgradeFromVersion
		/// In any case, modify all the tables so they match the code
		/// If the version was upgraded, call PostUpgradeFromVersion afterwards,
		/// </summary>
		public void Upgrade() {
			LogLevel originalLevel = Logging;
			Logging = LogLevel.Writes;
			try {
				Dictionary<string, Table> dbTables = db.Tables();
				int p = -1;
				try {
					p = QueryOne("SELECT DbVersion FROM Settings").AsInt("DbVersion");
					if (p < 0)
						p = 0;
				} catch {
				}
				if (p < CurrentDbVersion)
					PreUpgradeFromVersion(p);
				TableList orderedTables = new TableList(_tables.Values);
				foreach (Table t in orderedTables.Reverse<Table>()) {
					Table database;
					dbTables.TryGetValue(t.Name, out database);
					upgrade(t, database);
				}
				BeginTransaction();
				if (p < CurrentDbVersion) {
					PostUpgradeFromVersion(p);
					Execute(p < 0 ? "INSERT INTO Settings (DbVersion) VALUES(" + CurrentDbVersion + ")" : "UPDATE Settings SET DbVersion = " + CurrentDbVersion);
				}
				Commit();
			} catch (Exception ex) {
				WebServer.Log(ex.ToString());
				throw;
			}
			Logging = originalLevel;
		}

		/// <summary>
		/// A database version number stored in the Settings table. Used to check if any extra changes
		/// need to be made on version change.
		/// </summary>
		public virtual int CurrentDbVersion {
			get {
				return 0;
			}
		}

		/// <summary>
		/// Code that must be run before the database is reconfigured - e.g. renaming old fields
		/// </summary>
		/// <param name="version">Original version (-1 = new database)</param>
		public virtual void PreUpgradeFromVersion(int version) {
		}

		/// <summary>
		/// Code that must be run after the database is reconfigured - e.g. populating new fields
		/// </summary>
		/// <param name="version">Original version (-1 = new database)</param>
		public virtual void PostUpgradeFromVersion(int version) {
		}

		DbInterface getDatabase(string type, string connectionString) {
			switch (type.ToLower()) {
				case "sqlite":
					return new SQLiteDatabase(this, connectionString);
				case "mysql":
					return new MySqlDatabase(this, connectionString);
				case "sqlserver":
					return new SqlServerDatabase(this, connectionString);
				default:
					throw new CheckException("Unknown database type {0}", Config.Default.Database);
			}
		}

		public Database(ServerConfig server) {
			this.server = server;
			_tables = server.NamespaceDef.Tables;
			string type = string.IsNullOrWhiteSpace(server.Database) ? Config.Default.Database : server.Database;
			string connectionString = string.IsNullOrWhiteSpace(server.ConnectionString) ? Config.Default.ConnectionString : server.ConnectionString;
			UniqueIdentifier = type + "\t" + connectionString;
			db = getDatabase(type, connectionString);
		}

		public void BeginTransaction() {
			db.BeginTransaction();
		}

		public string Cast(string value, string type) {
			return db.Cast(value, type);
		}

		public void CheckValidFieldname(string f) {
			if (!IsValidFieldname(f))
				throw new CheckException("'{0}' is not a valid field name", f);
		}

		public void Clean() {
			db.CleanDatabase();
		}

		public void Commit() {
			db.Commit();
		}

		public void Delete(string tableName, JObject data) {
			delete(TableFor(tableName), data);
		}

		public void Delete(string tableName, int id) {
			Table t = TableFor(tableName);
			Execute("DELETE FROM " + tableName + " WHERE " + t.PrimaryKey.Name + '=' + id);
		}

		public void Delete(JsonObject data) {
			delete(TableFor(data.GetType()).UpdateTable, data.ToJObject());
		}

		public void delete(Table table, JObject data) {
			Index index = table.IndexFor(data);
			Utils.Check(index != null, "Deleting from {0}:data does not specify unique record", table.Name);
			Execute("DELETE FROM " + table.Name + " WHERE " + index.Where(data));
		}

		public void Dispose() {
			Rollback();
			db.Dispose();
			db = null;
		}

		public JObject EmptyRecord(string tableName) {
			return emptyRecord(TableFor(tableName));
		}

		public T EmptyRecord<T>() where T : JsonObject {
			JObject record = emptyRecord(TableFor(typeof(T)));
			return record.ToObject<T>();
		}

		JObject emptyRecord(Table table) {
			JObject record = new JObject();
			foreach (Field field in table.Fields.Where(f => f.ForeignKey == null && !f.Nullable)) {
				record[field.Name] = field.Type == typeof(string) ? "" : Activator.CreateInstance(field.Type).ToJToken();
			}
			record[table.PrimaryKey.Name] = null;
			return record;
		}

		public int Execute(string sql) {
			using (new Timer(sql)) {
				if (Logging >= LogLevel.Writes) Log(sql);
				try {
					return db.Execute(sql);
				} catch (Exception ex) {
					throw new DatabaseException(ex, sql);
				}
			}
		}

		public bool Exists(string tableName, int? id) {
			Table table = TableFor(tableName);
			string idName = table.PrimaryKey.Name;
			return id != null && QueryOne("SELECT " + idName + " FROM " + tableName + " WHERE "
				+ idName + " = " + id) != null;
		}

		public int? ForeignKey(string tableName, JObject data) {
			int? result = LookupKey(tableName, data);
			return result ?? insert(TableFor(tableName), data);
		}

		public int? ForeignKey(string tableName, params object[] data) {
			return ForeignKey(tableName, new JObject().AddRange(data));
		}

		public T Get<T>(int id) where T : JsonObject {
			Table table = TableFor(typeof(T));
			return QueryOne<T>("SELECT * FROM " + table.Name + " WHERE " + table.PrimaryKey.Name + " = " + id);
		}

		public T Get<T>(T criteria) where T : JsonObject {
			Table table = TableFor(typeof(T));
			JObject data = criteria.ToJObject();
			Index index = table.IndexFor(data);
			if (index != null) {
				data = QueryOne("SELECT * FROM " + table.Name + " WHERE " + index.Where(data));
			} else {
				data = null;
			}
			if (data == null || data.IsAllNull())
				data = emptyRecord(table);
			return data.ToObject<T>();
		}

		public JObject Get(string tableName, int id) {
			Table table = TableFor(tableName);
			JObject result = QueryOne("SELECT * FROM " + table.Name + " WHERE " + table.PrimaryKey.Name + " = " + id);
			return result == null ? emptyRecord(table) : result;
		}

		/// <summary>
		/// Produce an "IN(...)" SQL statement from a list of values
		/// </summary>
		public string In(params object[] args) {
			return "IN(" + string.Join(",", args.Select(o => Quote(o is Enum ? (int)o : o)).ToArray()) + ")";
		}

		/// <summary>
		/// Produce an "IN(...)" SQL statement from a list of values
		/// </summary>
		public string In<T>(IEnumerable<T> args) {
			return "IN(" + string.Join(",", args.Select(o => Quote(o)).ToArray()) + ")";
		}

		public void Insert(string tableName, List<JObject> data) {
			Table table = TableFor(tableName);
			foreach (JObject row in data)
				insert(table, row);
		}

		public void Insert(string tableName, JObject data) {
			insert(TableFor(tableName), data);
		}

		public void Insert(string tableName, JsonObject data) {
			Table table = TableFor(tableName);
			JObject d = data.ToJObject();
			insert(table, d);
			data.Id = (int)d[table.PrimaryKey.Name];
		}

		public void Insert(JsonObject data) {
			Table table = TableFor(data.GetType()).UpdateTable;
			JObject d = data.ToJObject();
			insert(table, d);
			data.Id = (int)d[table.PrimaryKey.Name];
		}

		int insert(Table table, JObject data) {
			Field idField = table.PrimaryKey;
			string idName = idField.Name;
			List<Field> fields = table.Fields.Where(f => !data.IsMissingOrNull(f.Name)).ToList();
			checkForMissingFields(table, data, true);
			try {
				string sql = "INSERT INTO \"" + table.Name + "\" ("
					+ string.Join(", ", fields.Select(f => f.Name).ToArray()) + ") VALUES ("
					+ string.Join(", ", fields.Select(f => f.Quote(data[f.Name])).ToArray()) + ")";
				using (new Timer(sql)) {
					if (Logging >= LogLevel.Writes) Log(sql);
					int lastInsertId = db.Insert(table, sql, idField.AutoIncrement && !data.IsMissingOrNull(idName));
					if (idField.AutoIncrement && data.IsMissingOrNull(idName))
						data[idName] = lastInsertId;
					return lastInsertId;
				}
			} catch (DatabaseException ex) {
				throw new DatabaseException(ex, table);
			}
		}

		void checkForMissingFields(Table table, JObject data, bool insert) {
			Field idField = table.PrimaryKey;
			string[] errors = table.Indexes.SelectMany(i => i.Fields)
				.Distinct()
				.Where(f => f != idField && !f.Nullable && string.IsNullOrWhiteSpace(data.AsString(f.Name)) && (insert || !data.IsMissingOrNull(f.Name)))
				.Select(f => f.Name)
				.ToArray();
			Utils.Check(errors.Length == 0, "Table {0} {1}:Missing key fields {2}",
				table.Name, insert ? "insert" : "update", string.Join(", ", errors));
		}

		public bool IsValidFieldname(string f) {
			return Regex.IsMatch(f, @"^[a-z]+$", RegexOptions.IgnoreCase);
		}

		public void Log(string sql) {
			WebServer.Log(sql);
		}

		public LogLevel Logging;

		public int? LookupKey(string tableName, JObject data) {
			Table table = TableFor(tableName);
			string idName = table.PrimaryKey.Name;
			Index index = table.IndexFor(data);
			if (index == null || index.Fields.FirstOrDefault(f => data[f.Name].ToString() != "") == null) return null;
			JObject result = QueryOne("SELECT " + idName + " FROM " + tableName + " WHERE "
				+ index.Where(data));
			return result == null ? null : result[idName].To<int?>();
		}

		public int? LookupKey(string tableName, params object[] data) {
			return LookupKey(tableName, new JObject().AddRange(data));
		}

		public JObjectEnumerable Query(string sql) {
			if (Logging >= LogLevel.Reads) Log(sql);
			try {
				using (new Timer(sql)) {
					return new JObjectEnumerable(db.Query(sql));
				}
			} catch (Exception ex) {
				throw new DatabaseException(ex, sql);
			}
		}

		public JObjectEnumerable Query(string fields, string conditions, params string[] tableNames) {
			return Query(buildQuery(fields, conditions, tableNames));
		}

		string buildQuery(string fields, string conditions, params string[] tableNames) {
			List<string> joins = new List<string>();
			List<Table> tables = tableNames.Select(n => TableFor(n)).ToList();
			List<Field> allFields = new List<Field>();
			List<Table> processed = new List<Table>();
			foreach (Table q in tables) {
				processed.Add(q);
				Field pk = q.PrimaryKey;
				if (joins.Count == 0) {
					joins.Add("FROM " + q.Name);
					allFields.AddRange(q.Fields);
				} else {
					Table detail = processed.FirstOrDefault(t => t.ForeignKeyFieldFor(q) != null);
					if (detail != null) {
						// q is master
						Field fk = detail.ForeignKeyFieldFor(q);
						joins.Add("LEFT JOIN " + q.Name + " ON " + q.Name + "." + fk.ForeignKey.Field.Name + " = " + detail.Name + "." + fk.Name);
						allFields.AddRange(q.Fields.Where(f => f != pk));
					} else {
						// q is detail
						Table master = processed.FirstOrDefault(t => q.ForeignKeyFieldFor(t) != null);
						if (master == null)
							throw new CheckException("No joins between {0} and any of {1}",
								q.Name, string.Join(",", tables.Select(t => t.Name).ToArray()));
						Field fk = q.ForeignKeyFieldFor(master);
						joins.Add("LEFT JOIN " + q.Name + " ON " + q.Name + "." + fk.Name + " = " + master.Name + "." + fk.ForeignKey.Field.Name);
						allFields.AddRange(q.Fields.Where(f => f != pk));
					}
				}
				foreach (Field fk in q.Fields.Where(f => f.ForeignKey != null && f.ForeignKey.Table.Indexes.Length > 1 && tables.IndexOf(f.ForeignKey.Table) < 0)) {
					Table master = fk.ForeignKey.Table;
					string joinName = q.Name + "_" + master.Name;
					joins.Add("LEFT JOIN " + master.Name + " AS " + joinName + " ON " + joinName + "." + fk.ForeignKey.Field.Name + " = " + q.Name + "." + fk.Name);
					int i = allFields.IndexOf(fk);
					if (i <= 0)		// Do not remove first field, which will be key of first file
						i = allFields.Count;
					else if (fields != "+")
						allFields.RemoveAt(i);
					allFields.InsertRange(i, master.Indexes[1].Fields);
				}
			}
			if (string.IsNullOrEmpty(fields) || fields == "+")
				fields = string.Join(",", allFields.Select(f => f.Name).ToArray());
			return "SELECT " + fields + "\r\n" + string.Join("\r\n", joins) + "\r\n" + conditions;
		}

		public IEnumerable<T> Query<T>(string sql) {
			return Query(sql).Select(r => r.ToObject<T>());
		}

		public IEnumerable<T> Query<T>(string fields, string conditions, params string[] tableNames) {
			return Query(fields, conditions, tableNames).Select(r => r.ToObject<T>());
		}

		public JObject QueryOne(string query) {
			return db.QueryOne(query);
		}

		public JObject QueryOne(string fields, string conditions, params string[] tableNames) {
			return QueryOne(buildQuery(fields, conditions, tableNames));
		}

		public T QueryOne<T>(string query) where T : JsonObject {
			JObject data = QueryOne(query);
			return data == null || data.IsAllNull() ? EmptyRecord<T>() : data.To<T>();
		}

		public T QueryOne<T>(string fields, string conditions, params string[] tableNames) where T : JsonObject {
			JObject data = QueryOne(fields, conditions, tableNames);
			return data == null || data.IsAllNull() ? EmptyRecord<T>() : data.To<T>();
		}

		public string Quote(object o) {
			if (o == null || o == DBNull.Value) return "NULL";
			if (o is int || o is long || o is double) return o.ToString();
			if (o is decimal) return ((decimal)o).ToString("0.00");
			if (o is double) return (Math.Round((decimal)o, 4)).ToString();
			if (o is double) return ((decimal)o).ToString("0");
			if (o is bool) return (bool)o ? "1" : "0";
			if (o is DateTime) return "'" + ((DateTime)o).ToString("yyyy-MM-dd") + "'";
			return "'" + o.ToString().Replace("'", "''") + "'";
		}

		public bool RecordExists(string table, int id) {
			return RecordExists(TableFor(table), id);
		}

		public bool RecordExists(Table table, int id) {
			Field idField = table.PrimaryKey;
			string idName = idField.Name;
			return QueryOne("SELECT " + idName + " FROM " + table.Name + " WHERE " + idName + " = " + id) != null;
		}

		public void Rollback() {
			db.Rollback();
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

		public Table TableFor(Type type) {
			Type t = type;
			while (!_tables.ContainsKey(t.Name)) {
				t = t.BaseType;
				Utils.Check(t != typeof(JsonObject), "Unable to find a table for type {0}", type.Name);
			}
			return TableFor(t.Name);
		}

		public string UniqueIdentifier { get; private set; }

		public void Update(string tableName, List<JObject> data) {
			Table table = TableFor(tableName);
			foreach (JObject row in data)
				update(table, row);
		}

		public void Update(string tableName, JObject data) {
			update(TableFor(tableName), data);
		}

		public void Update(JsonObject data) {
			Table table = TableFor(data.GetType()).UpdateTable;
			JObject d = data.ToJObject();
			update(table, d);
			data.Id = (int)d[table.PrimaryKey.Name];
		}

		protected void update(Table table, JObject data) {
			Field idField = table.PrimaryKey;
			string idName = idField.Name;
			JToken idValue = null;
			Index index = table.Indexes[0];
			JObject result = QueryOne(idName, "WHERE " + index.Where(data), table.Name);
			if (result != null) {
				data[idName] = idValue = result[idName];
			}
			List<Field> fields = table.Fields.Where(f => !data.IsMissingOrNull(f.Name)).ToList();
			checkForMissingFields(table, data, idValue == null);
			try {
				if (idValue != null) {
					Execute("UPDATE " + table.Name + " SET "
						+ string.Join(", ", fields.Where(f => f != idField).Select(f => f.Name + '=' + f.Quote(data[f.Name])).ToArray())
						+ " WHERE " + index.Where(data));
				} else {
					insert(table, data);
				}
			} catch (DatabaseException ex) {
				throw new DatabaseException(ex, table);
			}
		}

		protected void updateIfChanged(Table table, JObject data) {
			Field idField = table.PrimaryKey;
			string idName = idField.Name;
			JToken idValue = null;
			List<Field> fields = table.Fields.Where(f => data[f.Name] != null).ToList();
			Index index = table.IndexFor(data);
			JObject result = null;
			try {
				result = QueryOne("SELECT * FROM " + table.Name + " WHERE " + index.Where(data));
				if (result != null)
					data[idName] = idValue = result[idName];
				if (idValue != null) {
					fields = fields.Where(f => data.AsString(f.Name) != result.AsString(f.Name)).ToList();
					if (fields.Count == 0)
						return;
					Execute("UPDATE " + table.Name + " SET "
						+ string.Join(", ", fields.Where(f => f != idField).Select(f => f.Name + '=' + f.Quote(data[f.Name])).ToArray())
						+ " WHERE " + index.Where(data));
				} else {
					data[idName] = db.Insert(table, "INSERT INTO " + table.Name + " ("
						+ string.Join(", ", fields.Select(f => f.Name).ToArray()) + ") VALUES ("
						+ string.Join(", ", fields.Select(f => f.Quote(data[f.Name])).ToArray()) + ")", false);
				}
			} catch (DatabaseException ex) {
				throw new DatabaseException(ex, table);
			}
		}

		public class Timer : IDisposable {
			DateTime _start;
			string _message;

			public Timer(string message) {
				_start = Utils.Now;
				_message = message;
			}

			public void Dispose() {
				double elapsed = (Utils.Now - _start).TotalMilliseconds;
				if (elapsed > MaxTime)
					WebServer.Log("{0}:{1}", elapsed, _message);
			}

			public double MaxTime = Config.Default.SlowQuery;
		}

	}

	public class ForeignKey {
		public ForeignKey(Table table, Field field) {
			Table = table;
			Field = field;
		}

		public Table Table { get; private set; }

		public Field Field { get; private set; }
	}

	public class Field {

		public Field(string name) {
			Name = name;
			Type = typeof(string);
		}

		public Field(string name, Type type, decimal length, bool nullable, bool autoIncrement, string defaultValue) {
			Name = name;
			Type = type;
			Length = length;
			Nullable = nullable;
			AutoIncrement = autoIncrement;
			if (type == typeof(decimal) && defaultValue != null) {
				try {
					defaultValue = decimal.Parse(defaultValue).ToString("0.####");
				} catch {
				}
			}
			if (defaultValue == null && !nullable) {
				if (type == typeof(bool) || type == typeof(int) || type == typeof(decimal) || type == typeof(double)) {
					defaultValue = "0";
				} else if (type == typeof(string)) {
					defaultValue = "";
				}
			}
			DefaultValue = defaultValue;
		}

		public bool AutoIncrement { get; private set; }

		public string DefaultValue { get; private set; }

		public ForeignKey ForeignKey;

		public decimal Length { get; private set; }

		public string Name { get; private set; }

		public bool Nullable { get; private set; }

		public string Quote(object o) {
			if (o == null || o == DBNull.Value) return "NULL";
			if ((Type == typeof(int) || Type == typeof(decimal) || Type == typeof(double) || Type == typeof(DateTime)) && o.ToString() == "") return "NULL";
			try {
				o = Convert.ChangeType(o.ToString(), Type);
			} catch (Exception ex) {
				throw new CheckException(ex, "Invalid value for {0} field {1} '{2}'", Type.Name, Name, o);
			}
			if (o is int || o is long || o is double) return o.ToString();
			if (o is decimal) return ((decimal)o).ToString("0.00");
			if (o is double) return (Math.Round((decimal)o, 4)).ToString();
			if (o is bool) return (bool)o ? "1" : "0";
			if (o is DateTime) return "'" + ((DateTime)o).ToString("yyyy-MM-dd") + "'";
			return "'" + o.ToString().Replace("'", "''") + "'";
		}

		public Type Type { get; private set; }

		public string TypeName {
			get {
				string name = Type.Name;
				switch (name) {
					case "Int32":
						return Nullable ? "int?" : "int";
					case "Decimal":
						return Nullable ? "decimal?" : "decimal";
					case "Double":
						return Nullable ? "double?" : "double";
					case "Boolean":
						return Nullable ? "bool?" : "bool";
					case "DateTime":
						return Nullable ? "DateTime?" : "DateTime";
					case "String":
						return "string";
					default:
						return name;
				}
			}
		}

		public string Data(bool view) {
			string s = Name + "(" + TypeName;
			if (view)
				s += ")";
			else {
				s += ":" + Length + ")";
				if (DefaultValue != null)
					s += "='" + DefaultValue + "'";
				if (AutoIncrement)
					s += "[AutoIncrement]";
			}
			return s;
		}

		public static Field FieldFor(FieldInfo field, out PrimaryAttribute pk) {
			pk = null;
			if (field.IsDefined(typeof(DoNotStoreAttribute)))
				return null;
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
			pk = field.GetCustomAttribute<PrimaryAttribute>();
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
			return new Field(field.Name, pt, length, nullable, pk != null && pk.AutoIncrement, defaultValue);
		}

		public override string ToString() {
			return Name + "(" + TypeName + ")";
		}
	}

	public class Index {

		public Index(string name, params Field[] fields) {
			Name = name;
			Fields = fields;
		}

		public Index(string name, params string[] fields) {
			Name = name;
			Fields = fields.Select(f => new Field(f)).ToArray();
		}

		public bool CoversData(JObject data) {
			return (Fields.Where(f => data.IsMissingOrNull(f.Name)).FirstOrDefault() == null);
		}

		public string FieldList {
			get { return string.Join(",", Fields.Select(f => f.ToString()).ToArray()); }
		}

		public Field[] Fields { get; private set; }

		public string Name { get; private set; }

		public string Where(JObject data) {
			return string.Join(" AND ", Fields.Select(f => f.Name + "=" + f.Quote(data[f.Name])).ToArray());
		}

		public override string ToString() {
			return "I:" + Name + "=" + FieldList;
		}
	}

	public class Table {

		public Table(string name, Field[] fields, Index[] indexes) {
			Name = name;
			Fields = fields;
			Indexes = indexes;
		}

		public Field[] Fields;

		public Field FieldFor(string name) {
			return Fields.FirstOrDefault(f => f.Name == name);
		}

		public Field ForeignKeyFieldFor(Table table) {
			return Fields.FirstOrDefault(f => f.ForeignKey != null && f.ForeignKey.Table.Name == table.Name);
		}

		public Index[] Indexes { get; private set; }

		public Index IndexFor(JObject data) {
			return Indexes.Where(i => i.CoversData(data)).FirstOrDefault();
		}

		public string Name { get; private set; }

		public Field PrimaryKey {
			get { return Indexes[0].Fields[0]; }
		}

		public virtual Table UpdateTable { get { return this; } }

		public virtual bool IsView { get { return false; } }

		public override string ToString() {
			return "T:" + string.Join(",", Fields.Select(f => f.ToString()).ToArray()) + "\r\n"
				+ string.Join("\r\n", Indexes.Select(i => i.ToString()).ToArray());
		}
	}

	public class View : Table {
		Table _updateTable;

		public View(string name, Field[] fields, Index[] indexes, string sql, Table updateTable)
			: base(name, fields, indexes) {
			Sql = sql;
			_updateTable = updateTable;
		}

		public string Sql { get; private set; }

		public override Table UpdateTable { get { return _updateTable; } }

		public override bool IsView { get { return true; } }

	}

	public class JObjectEnumerable : IEnumerable<JObject> {
		IEnumerable<JObject> _e;

		public JObjectEnumerable(IEnumerable<JObject> e) {
			_e = e;
		}

		public List<JObject> ToList() {
			List<JObject> e = _e.ToList();
			_e = e;
			return e;
		}

		public override string ToString() {
			return this.ToJson();
		}

		public IEnumerator<JObject> GetEnumerator() {
			return _e.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return _e.GetEnumerator();
		}

		static public implicit operator JArray(JObjectEnumerable o) {
			JArray j = new JArray();
			foreach (JObject jo in o) {
				j.Add(jo);
			}
			return j;
		}
	}

	public class JsonObject {

		public JObject ToJObject() {
			return JObject.FromObject(this);
		}

		public T Clone<T>() {
			return this.ToJObject().To<T>();
		}

		public virtual int? Id {
			get { return null; }
			set { }
		}

		public override string ToString() {
			return this.ToJson();
		}

	}

	public class TableList : List<Table> {
		List<Table> _allTables;

		public TableList(IEnumerable<Table> allTables) {
			_allTables = allTables.ToList();
			foreach (Table t in _allTables.Where(t => t is View))
				add(t);
			foreach (Table t in _allTables.Where(t => !(t is View)))
				add(t);
		}

		void add(Table table) {
			if (IndexOf(table) >= 0) return;
			foreach (Table detail in _allTables.Where(t => t.ForeignKeyFieldFor(table) != null)) {
				add(detail);
			}
			if (table is View) {
				foreach (Table detail in _allTables.Where(t => t is View && (t as View).Sql.Contains(table.Name))) {
					add(detail);
				}

			}
			Add(table);
		}
	}

	public class DatabaseException : Exception {

		public DatabaseException(DatabaseException ex, Table table)
			: base(ex.InnerException.Message, ex.InnerException) {
			Sql = ex.Sql;
			Table = table.Name;
		}

		public DatabaseException(Exception ex, string sql)
			: base(ex.Message, ex) {
			Sql = sql;
		}

		public string Sql;

		public string Table;

		public override string Message {
			get {
				return Table == null ? base.Message : Table + ":" + base.Message;
			}
		}

		public override string ToString() {
			return base.ToString() + "\r\nSQL:" + Sql;

		}
	}

}
