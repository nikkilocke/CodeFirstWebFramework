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
	/// <summary>
	/// Class used for accessing the database.
	/// Programs may subclass this to add more functionality.
	/// </summary>
	public class Database : IDisposable {
		DbInterface db;
		ServerConfig server;
		Dictionary<string, Table> _tables;
		bool _startup;

		/// <summary>
		/// Make an individual table in the database correspond to the code Table class
		/// </summary>
		/// <param name="code"></param>
		/// <param name="database"></param>
		void upgrade(Table code, Table database) {
			try {
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
						if (!view) {
							if (f1.ForeignKey == null) {
								if (f2.ForeignKey != null)
									dropFK.Add(f2);
							} else {
								if (f2.ForeignKey == null)
									insertFK.Add(f1);
								else if (!db.TableNamesMatch(f1.ForeignKey.Table, f2.ForeignKey.Table)) {
									dropFK.Add(f2);
									insertFK.Add(f1);
								}
							}
						}
					}
				}
				foreach (Field f2 in database.Fields) {
					if (code.FieldFor(f2.Name) == null) {
						remove.Add(f2);
						if (!view && f2.ForeignKey != null)
							dropFK.Add(f2);
					}
				}
				if (view) {
					if (insert.Count == 0 && update.Count == 0 && remove.Count == 0)
						return;
					db.DropTable(database);
					db.CreateTable(code);
					return;
				}
				foreach (Index i1 in code.Indexes) {
					Index i2 = database.Indexes.Where(i => i.Matches(i1)).FirstOrDefault();
					if (i2 == null) {
						insertIndex.Add(i1);
					}
				}
				foreach (Index i2 in database.Indexes) {
					if (code.Indexes.Where(i => i.Matches(i2)).FirstOrDefault() == null)
						dropIndex.Add(i2);
				}
				if (insert.Count != 0 || update.Count != 0 || remove.Count != 0 || insertFK.Count != 0 || dropFK.Count != 0 || insertIndex.Count != 0 || dropIndex.Count != 0)
					db.UpgradeTable(code, database, insert, update, remove, insertFK, dropFK, insertIndex, dropIndex);
			} catch(Exception ex) {
				Log.Error.WriteLine("Error upgrading table {0} in database {1}\n", code.Name, this.UniqueIdentifier, ex);
				throw;
			}
		}

		/// <summary>
		/// Check the version in the Settings table. If not the same as CurrentDbVersion, call PreUpgradeFromVersion
		/// In any case, modify all the tables so they match the code
		/// If the version was upgraded, call PostUpgradeFromVersion afterwards,
		/// </summary>
		public void Upgrade() {
			try {
				_startup = true;
				Dictionary<string, Table> dbTables = db.Tables();
				int p = -1;
				try {
					JObject q = QueryOne("SELECT DbVersion FROM Settings");
					if (q != null) {
						p = q.AsInt("DbVersion");
						if (p < 0)
							p = 0;
					}
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
				Log.Error.WriteLine(ex.ToString());
				throw;
			} finally {
				_startup = false;
			}
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
					throw new CheckException("Support for SQL Server has temporarily been dropped");
					// return new SqlServerDatabase(this, connectionString);
				default:
					throw new CheckException("Unknown database type {0}", Config.Default.Database);
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="server">ServerConfig optionally containing the database type and connection string 
		/// (Config.Default is used for items not supplied)</param>
		public Database(ServerConfig server) {
			this.server = server;
			_tables = server.NamespaceDef.Tables;
			string type = string.IsNullOrWhiteSpace(server.Database) ? Config.Default.Database : server.Database;
			string connectionString = string.IsNullOrWhiteSpace(server.ConnectionString) ? Config.Default.ConnectionString : server.ConnectionString;
			UniqueIdentifier = type + "\t" + connectionString;
			db = getDatabase(type, connectionString);
		}

		/// <summary>
		/// The module which created this database
		/// </summary>
		public AppModule Module;

		/// <summary>
		/// Start a transaction
		/// </summary>
		public void BeginTransaction() {
			db.BeginTransaction();
		}

		/// <summary>
		/// Return SQL to cast a value to a type
		/// </summary>
		public string Cast(string value, string type) {
			return db.Cast(value, type);
		}

		/// <summary>
		/// Check a field name is valid, throw an exception if not.
		/// </summary>
		/// <param name="f"></param>
		public void CheckValidFieldname(string f) {
			if (!IsValidFieldname(f))
				throw new CheckException("'{0}' is not a valid field name", f);
		}

		/// <summary>
		/// Clean and compact the database.
		/// </summary>
		public void Clean() {
			db.CleanDatabase();
		}

		/// <summary>
		/// Commit a transaction
		/// </summary>
		public void Commit() {
			db.Commit();
		}

		/// <summary>
		/// Delete a record by content.
		/// </summary>
		/// <param name="tableName">Table name</param>
		/// <param name="data">Content - if this matches a unique key, that is the record which will be deleted</param>
		public void Delete(string tableName, JObject data) {
			delete(TableFor(tableName), data);
		}

		/// <summary>
		/// Delete a record by id.
		/// </summary>
		public void Delete(string tableName, int id) {
			Table t = TableFor(tableName);
			Execute("DELETE FROM " + tableName + " WHERE " + t.PrimaryKey.Name + '=' + id);
		}

		/// <summary>
		/// Delete a record by content.
		/// </summary>
		/// <param name="data">Content - if this matches a unique key, that is the record which will be deleted</param>
		public void Delete(JsonObject data) {
			delete(TableFor(data.GetType()).UpdateTable, data.ToJObject());
		}

		/// <summary>
		/// Delete a record by content.
		/// </summary>
		/// <param name="table">Table</param>
		/// <param name="data">Content - if this matches a unique key, that is the record which will be deleted</param>
		public void delete(Table table, JObject data) {
			Index index = table.IndexFor(data);
			Utils.Check(index != null, "Deleting from {0}:data does not specify unique record", table.Name);
			Execute("DELETE FROM " + table.Name + " WHERE " + index.Where(data, Quote));
		}

		/// <summary>
		/// Dispose of the database.
		/// Any uncommitted transaction will be rolled back, and the connection will be closed.
		/// </summary>
		public void Dispose() {
			try {
				Rollback();
				db.Dispose();
				db = null;
			} catch {
			}
		}

		/// <summary>
		/// Create an empty record for the given table as a JObject
		/// </summary>
		public JObject EmptyRecord(string tableName) {
			return emptyRecord(TableFor(tableName));
		}

		/// <summary>
		/// Create an empty record as a C# object
		/// NB If called with T a base class of the class used to create the table, returns an object of the derived class
		/// </summary>
		public T EmptyRecord<T>() where T : JsonObject {
			Table t = TableFor(typeof(T));
			JObject record = emptyRecord(t);
			return t.FromJson<T>(record);
		}

		JObject emptyRecord(Table table) {
			JObject record = new JObject();
			foreach (Field field in table.Fields.Where(f => f.ForeignKey == null && !f.Nullable)) {
				record[field.Name] = field.Type == typeof(string) ? "" : Activator.CreateInstance(field.Type).ToJToken();
			}
			record[table.PrimaryKey.Name] = null;
			return record;
		}

		/// <summary>
		/// Execute arbitrary SQL
		/// </summary>
		public int Execute(string sql) {
			using (new Timer(sql)) {
				if (_startup)
					Log.Startup.WriteLine(sql);
				if(Logging)
					Log.DatabaseWrite.WriteLine(sql);
				try {
					return db.Execute(sql);
				} catch (DatabaseException) {
					throw;
				} catch (Exception ex) {
					throw new DatabaseException(ex, sql);
				}
			}
		}

		/// <summary>
		/// Find out if a record with the given id exists in the table
		/// </summary>
		public bool Exists(string tableName, int? id) {
			Table table = TableFor(tableName);
			string idName = table.PrimaryKey.Name;
			return id != null && QueryOne("SELECT " + idName + " FROM " + tableName + " WHERE "
				+ idName + " = " + id) != null;
		}

		/// <summary>
		/// Given data that represents a unique key in a table, if a record matching the key
		/// exists, return its record id, otherwise create a new record using the data and
		/// return its id.
		/// </summary>
		public int? ForeignKey(string tableName, JObject data) {
			int? result = LookupKey(tableName, data);
			return result ?? insert(TableFor(tableName), data);
		}

		/// <summary>
		/// Given name, value pairs that represents a unique key in a table, if a record matching the key
		/// exists, return its record id, otherwise create a new record using the data and
		/// return its id.
		/// </summary>
		public int? ForeignKey(string tableName, params object[] data) {
			return ForeignKey(tableName, new JObject().AddRange(data));
		}

		/// <summary>
		/// Get a record by id
		/// NB If called with T a base class of the class used to create the table, returns an object of the derived class
		/// </summary>
		public T Get<T>(int id) where T : JsonObject {
			Table table = TableFor(typeof(T));
			return QueryOne<T>("SELECT * FROM " + table.Name + " WHERE " + table.PrimaryKey.Name + " = " + id);
		}

		/// <summary>
		/// See if a record exists by id
		/// NB If called with T a base class of the class used to create the table, returns an object of the derived class
		/// </summary>
		/// <param name="id">The record to get</param>
		/// <param name="record">Returned record (will be empty if record doesn't exist)</param>
		public bool TryGet<T>(int id, out T record) where T : JsonObject {
			Table table = TableFor(typeof(T));
			record = QueryOne<T>("SELECT * FROM " + table.Name + " WHERE " + table.PrimaryKey.Name + " = " + id);
			return record.Id == id;
		}

		/// <summary>
		/// See if a record exists by primary key
		/// NB If called with T a base class of the class used to create the table, returns an object of the derived class
		/// </summary>
		/// <param name="record">Returned record (will be empty if record doesn't exist)</param>
		/// <param name="keys">Keys for that index</param>
		public bool TryGet<T>(out T record, params object[] keys) where T : JsonObject {
			Table table = TableFor(typeof(T));
			Utils.Check(table != null, $"No table for {typeof(T).Name}");
			Index index = table.Indexes[0];
			return TryGet(index, out record, keys);
		}

		/// <summary>
		/// See if a record exists by any key
		/// NB If called with T a base class of the class used to create the table, returns an object of the derived class
		/// </summary>
		/// <param name="indexName">The index to use</param>
		/// <param name="record">Returned record (will be empty if record doesn't exist)</param>
		/// <param name="keys">Keys for that index</param>
		public bool TryGet<T>(string indexName, out T record, params object [] keys) where T : JsonObject {
			Table table = TableFor(typeof(T));
			Utils.Check(table != null, $"No table for {typeof(T).Name}");
			Index index = table.Indexes.FirstOrDefault(i => i.Name == indexName);
			return TryGet(index, out record, keys);
		}

		/// <summary>
		/// See if a record exists by any key
		/// NB If called with T a base class of the class used to create the table, returns an object of the derived class
		/// </summary>
		/// <param name="index">The index to use</param>
		/// <param name="record">Returned record (will be empty if record doesn't exist)</param>
		/// <param name="keys">Keys for that index</param>
		public bool TryGet<T>(Index index, out T record, params object[] keys) where T : JsonObject {
			Utils.Check(keys.Length == index.Fields.Length, "Wrong number of paramaters in TryGet");
			Table table = TableFor(typeof(T));
			JObject criteria = new JObject();
			for (int i = 0; i < index.Fields.Length; i++) {
				Field f = index.Fields[i];
				criteria[f.Name] = keys[i].ToJToken();
			}
			JObject data = getByIndex(table, index, criteria);
			bool isNull = data == null || data.IsAllNull();
			if (isNull)
				data = criteria;
			record = table.FromJson<T>(data);
			return !isNull;
		}

		/// <summary>
		/// Get a record by unique key
		/// NB If called with T a base class of the class used to create the table, returns an object of the derived class
		/// </summary>
		public T Get<T>(T criteria) where T : JsonObject {
			Table table = TableFor(typeof(T));
			JObject data = criteria.ToJObject();
			Index index = table.IndexFor(data);
			if (index != null) {
				data = QueryOne("SELECT * FROM " + table.Name + " WHERE " + index.Where(data, Quote));
			} else {
				data = null;
			}
			if (data == null || data.IsAllNull())
				data = emptyRecord(table);
			return table.FromJson<T>(data);
		}

		/// <summary>
		/// See if a record exists by unique key. If it does, populate the supplied record from the one in the database.
		/// </summary>
		public bool TryGet<T>(T criteria) where T : JsonObject {
			Table table = TableFor(typeof(T));
			JObject data = criteria.ToJObject();
			Index index = table.IndexFor(data);
			data = getByIndex(table, index, data);
			if (data == null || data.IsAllNull())
				return false;
			criteria.CopyFrom(data);
			return true;
		}

		/// <summary>
		/// See if a record exists by unique key. If it does, populate the supplied record from the one in the database.
		/// </summary>
		JObject getByIndex(Table table, Index index, JObject data) {
			if (index != null) {
				data = QueryOne("SELECT * FROM " + table.Name + " WHERE " + index.Where(data, Quote));
			} else {
				data = null;
			}
			return data;
		}

		/// <summary>
		/// Get a record by id
		/// </summary>
		public JObject Get(string tableName, int id) {
			Table table = TableFor(tableName);
			JObject result = QueryOne("SELECT * FROM " + table.Name + " WHERE " + table.PrimaryKey.Name + " = " + id);
			return result ?? emptyRecord(table);
		}

		/// <summary>
		/// Return SQL to cast a value to a type
		/// </summary>
		public string GroupConcat(string expression, string separator = null) {
			return db.GroupConcat(expression, separator);
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

		/// <summary>
		/// Insert a series of records into the database
		/// </summary>
		public void Insert(string tableName, List<JObject> data) {
			Table table = TableFor(tableName);
			foreach (JObject row in data)
				insert(table, row);
		}

		/// <summary>
		/// Insert a record into the database
		/// </summary>
		public void Insert(string tableName, JObject data) {
			insert(TableFor(tableName), data);
		}

		/// <summary>
		/// Insert a record into the database
		/// </summary>
		public void Insert(string tableName, JsonObject data) {
			Table table = TableFor(tableName);
			JObject d = data.ToJObject();
			insert(table, d);
			data.Id = (int)d[table.PrimaryKey.Name];
		}

		/// <summary>
		/// Insert a record into the database
		/// </summary>
		public void Insert(JsonObject data) {
			Table table = TableFor(data.GetType()).UpdateTable;
			JObject d = data.ToJObject();
			insert(table, d);
			if (table.PrimaryKey.AutoIncrement && table.Indexes[0].Fields.Length == 1)
				data.Id = (int)d[table.PrimaryKey.Name];
		}

		int insert(Table table, JObject data) {
			Field idField = table.PrimaryKey;
			string idName = idField.Name;
			List<Field> fields = table.Fields.Where(f => !data.IsMissingOrNull(f.Name)).ToList();
			checkForMissingFields(table, data, true);
			string sql = "INSERT INTO " + table.Name + " ("
				+ string.Join(", ", fields.Select(f => f.Name).ToArray()) + ") VALUES ("
				+ string.Join(", ", fields.Select(f => f.Quote(data[f.Name], Quote)).ToArray()) + ")";
			try {
				using (new Timer(sql)) {
					if (_startup)
						Log.Startup.WriteLine(sql);
					if (Logging)
						Log.DatabaseWrite.WriteLine(sql);
					int lastInsertId = db.Insert(table, sql, idField.AutoIncrement && !data.IsMissingOrNull(idName));
					if (idField.AutoIncrement && data.IsMissingOrNull(idName))
						data[idName] = lastInsertId;
					return lastInsertId;
				}
			} catch (DatabaseException) {
				throw;
			} catch (Exception ex) {
				throw new DatabaseException(ex, sql);
			}
		}

		void checkForMissingFields(Table table, JObject data, bool insert) {
			Field idField = table.PrimaryKey;
			string[] errors = table.Indexes.Where(i => i.Unique).SelectMany(i => i.Fields)
				.Distinct()
				.Where(f => f != idField && !f.Nullable && string.IsNullOrWhiteSpace(data.AsString(f.Name)) && (insert || !data.IsMissingOrNull(f.Name)))
				.Select(f => f.Name)
				.ToArray();
			Utils.Check(errors.Length == 0, "Table {0} {1}:Missing key fields {2}",
				table.Name, insert ? "insert" : "update", string.Join(", ", errors));
		}

		/// <summary>
		/// Determine if a name is a valid database field name
		/// </summary>
		public bool IsValidFieldname(string f) {
			return Regex.IsMatch(f, @"^[a-z]+$", RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// Whether to log - set to false to suppress logging
		/// </summary>
		public bool Logging = true;

		/// <summary>
		/// Find the record id of a record given data containing a unique key
		/// </summary>
		public int? LookupKey(string tableName, JObject data) {
			Table table = TableFor(tableName);
			string idName = table.PrimaryKey.Name;
			Index index = table.IndexFor(data);
			if (index == null || index.Fields.FirstOrDefault(f => data[f.Name].ToString() != "") == null) return null;
			JObject result = QueryOne("SELECT " + idName + " FROM " + tableName + " WHERE "
				+ index.Where(data, Quote));
			return result?[idName].To<int?>();
		}

		/// <summary>
		/// Find the record id of a record given name, value pairs containing a unique key
		/// </summary>
		public int? LookupKey(string tableName, params object[] data) {
			return LookupKey(tableName, new JObject().AddRange(data));
		}

		/// <summary>
		/// Query the database and return the records as JObjects
		/// </summary>
		public JObjectEnumerable Query(string sql) {
			if (Logging)
				Log.DatabaseRead.WriteLine(sql);
			try {
				using (new Timer(sql)) {
					return new JObjectEnumerable(db.Query(sql));
				}
			} catch(DatabaseException) {
				throw;
			} catch (Exception ex) {
				throw new DatabaseException(ex, sql);
			}
		}

		/// <summary>
		/// Query the database and return the records as JObjects
		/// </summary>
		/// <param name="fields">Fields to return - leave empty or use "+" to return all relevant fields</param>
		/// <param name="conditions">To use in the WHERE clause (may also include SORT BY)</param>
		/// <param name="tableNames">List of table names to join for the query. 
		/// Joins are performed automatically on foreign keys.</param>
		public JObjectEnumerable Query(string fields, string conditions, params string[] tableNames) {
			return Query(buildQuery(fields, conditions, tableNames));
		}

		string buildQuery(string fields, string conditions, params string[] tableNames) {
			List<string> joins = new List<string>();
			List<Table> tables = tableNames.Select(n => TableFor(n)).ToList();
			List<Field> allFields = new List<Field>();
			List<Table> processed = new List<Table>();
			int joinCount = 0;
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
					string joinName = q.Name + "_" + master.Name + ++joinCount;
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

		/// <summary>
		/// Query the database and return the records as C# objects
		/// NB If called with T a base class of the class used to create the table, returns an object of the derived class
		/// </summary>
		public IEnumerable<T> Query<T>(string sql) {
			Table t = TableForOrDefault(typeof(T));
			return Query(sql).Select(r => t.FromJson<T>(r));
		}

		/// <summary>
		/// Query the database and return the records as C# objects
		/// NB If called with T a base class of the class used to create the table, returns an object of the derived class
		/// </summary>
		/// <param name="fields">Fields to return - leave empty or use "+" to return all relevant fields</param>
		/// <param name="conditions">To use in the WHERE clause (may also include SORT BY)</param>
		/// <param name="tableNames">List of table names to join for the query. 
		/// Joins are performed automatically on foreign keys.</param>
		public IEnumerable<T> Query<T>(string fields, string conditions, params string[] tableNames) {
			Table t = TableForOrDefault(typeof(T));
			return Query(fields, conditions, tableNames).Select(r => t.FromJson<T>(r));
		}

		/// <summary>
		/// Retrieve a single value from a select
		/// </summary>
		public IEnumerable<T> QuerySingleValues<T>(string sql) {
			return Query(sql).Select(r => r.First.First.To<T>());
		}

		/// <summary>
		/// Query the database and return the first matching record as a JObject (or null if none)
		/// </summary>
		public JObject QueryOne(string query) {
			return db.QueryOne(query);
		}

		/// <summary>
		/// Query the database and return the first matching record as a JObject (or null if none)
		/// <param name="fields">Fields to return - leave empty or use "+" to return all relevant fields</param>
		/// <param name="conditions">To use in the WHERE clause (may also include SORT BY)</param>
		/// <param name="tableNames">List of table names to join for the query. 
		/// Joins are performed automatically on foreign keys.</param>
		/// </summary>
		public JObject QueryOne(string fields, string conditions, params string[] tableNames) {
			return QueryOne(buildQuery(fields, conditions, tableNames));
		}

		/// <summary>
		/// Query the database and return the first matching record as a C# object (or an empty record if none)
		/// NB If called with T a base class of the class used to create the table, returns an object of the derived class
		/// </summary>
		public T QueryOne<T>(string query) where T : JsonObject {
			Table t = TableForOrDefault(typeof(T));
			JObject data = QueryOne(query);
			return data == null || data.IsAllNull() ? EmptyRecord<T>() : t.FromJson<T>(data);
		}

		/// <summary>
		/// Query the database and return the first matching record as a C# object (or an empty record if none)
		/// NB If called with T a base class of the class used to create the table, returns an object of the derived class
		/// <param name="fields">Fields to return - leave empty or use "+" to return all relevant fields</param>
		/// <param name="conditions">To use in the WHERE clause (may also include SORT BY)</param>
		/// <param name="tableNames">List of table names to join for the query. 
		/// Joins are performed automatically on foreign keys.</param>
		/// </summary>
		public T QueryOne<T>(string fields, string conditions, params string[] tableNames) where T : JsonObject {
			Table t = TableForOrDefault(typeof(T));
			JObject data = QueryOne(fields, conditions, tableNames);
			return data == null || data.IsAllNull() ? EmptyRecord<T>() : t.FromJson<T>(data);
		}

		/// <summary>
		/// Quote any kind of data for inclusion in a SQL query
		/// </summary>
		public string Quote(object o) {
			return db.Quote(o);
		}

		/// <summary>
		/// Determine if a record with the given id exists
		/// </summary>
		public bool RecordExists(string table, int id) {
			return RecordExists(TableFor(table), id);
		}

		/// <summary>
		/// Determine if a record with the given id exists
		/// </summary>
		public bool RecordExists(Table table, int id) {
			Field idField = table.PrimaryKey;
			string idName = idField.Name;
			return QueryOne("SELECT " + idName + " FROM " + table.Name + " WHERE " + idName + " = " + id) != null;
		}

		/// <summary>
		/// Rollback transaction
		/// </summary>
		public void Rollback() {
			db.Rollback();
		}

		/// <summary>
		/// Return the names of all the tables
		/// </summary>
		public IEnumerable<string> TableNames {
			get { return _tables.Where(t => !t.Value.IsView).Select(t => t.Key); }
		}

		/// <summary>
		/// Return the names of all the views
		/// </summary>
		public IEnumerable<string> ViewNames {
			get { return _tables.Where(t => t.Value.IsView).Select(t => t.Key); }
		}

		/// <summary>
		/// Find the Table descriptor for a table name
		/// </summary>
		public Table TableFor(string name) {
			Table table;
			Utils.Check(_tables.TryGetValue(name, out table), "Table '{0}' does not exist", name);
			return table;
		}

		/// <summary>
		/// Find the Table descriptor for a C# type
		/// </summary>
		public Table TableFor(Type type) {
			Type t = type;
			while (!_tables.ContainsKey(t.Name)) {
				t = t.BaseType;
				Utils.Check(t != typeof(JsonObject), "Unable to find a table for type {0}", type.Name);
			}
			return TableFor(t.Name);
		}

		/// <summary>
		/// Try to find the Table descriptor for a C# type
		/// </summary>
		public Table TableForOrDefault(Type type) {
			Type t = type;
			while (!_tables.ContainsKey(t.Name)) {
				t = t.BaseType;
				if (t == typeof(JsonObject)) {
					return new Table(t);
				}
			}
			return TableFor(t.Name);
		}

		/// <summary>
		/// Return a unique identifier based on the connection string so different Database objects
		/// accessing the same database can be recognised as the same.
		/// </summary>
		public string UniqueIdentifier { get; private set; }

		/// <summary>
		/// Update a series of records
		/// If each record doesn't already exist, it will be created.
		/// </summary>
		public void Update(string tableName, List<JObject> data) {
			Table table = TableFor(tableName);
			foreach (JObject row in data)
				update(table, row);
		}

		/// <summary>
		/// Update a record.
		/// If the record doesn't already exist, it will be created.
		/// </summary>
		public void Update(string tableName, JObject data) {
			update(TableFor(tableName), data);
		}

		/// <summary>
		/// Update a record.
		/// If the record doesn't already exist, it will be created.
		/// </summary>
		public void Update(JsonObject data) {
			Table table = TableFor(data.GetType()).UpdateTable;
			JObject d = data.ToJObject();
			update(table, d);
			if(table.PrimaryKey.Type == typeof(int) || table.PrimaryKey.Type == typeof(int?))
				data.Id = (int)d[table.PrimaryKey.Name];
		}

		/// <summary>
		/// Update a record.
		/// If the record doesn't already exist, it will be created.
		/// </summary>
		protected void update(Table table, JObject data) {
			Field idField = table.PrimaryKey;
			string idName = idField.Name;
			JToken idValue = null;
			Index index = table.Indexes[0];
			JObject result = QueryOne("SELECT " + idName + " FROM " + table.Name + " WHERE " + index.Where(data, Quote));
			if (result != null)
				data[idName] = idValue = result[idName];
			List<Field> fields = table.Fields.Where(f => data[f.Name] != null && (f.Nullable || data[f.Name].Type != JTokenType.Null)).ToList();
			if (fields.Any(f => data[f.Name].Type == JTokenType.Null))
				Log.Debug.WriteLine("Fields set to null " + string.Join(",", fields.Where(f => data[f.Name].Type == JTokenType.Null).Select(f => f.Name).ToArray()));
			checkForMissingFields(table, data, idValue == null);
			if (idValue != null) {
				Execute("UPDATE " + table.Name + " SET "
					+ string.Join(", ", fields.Where(f => f != idField).Select(f => f.Name + '=' + f.Quote(data[f.Name], Quote)).ToArray())
					+ " WHERE " + index.Where(data, Quote));
			} else {
				insert(table, data);
			}
		}

		/// <summary>
		/// Update a record only if it has changed.
		/// If the record doesn't already exist, it will be created.
		/// </summary>
		protected void updateIfChanged(Table table, JObject data) {
			Field idField = table.PrimaryKey;
			string idName = idField.Name;
			JToken idValue = null;
			List<Field> fields = table.Fields.Where(f => data[f.Name] != null).ToList();
			Index index = table.IndexFor(data);
			JObject result = null;
			result = QueryOne("SELECT * FROM " + table.Name + " WHERE " + index.Where(data, Quote));
			if (result != null)
				data[idName] = idValue = result[idName];
			if (idValue != null) {
				fields = fields.Where(f => data.AsString(f.Name) != result.AsString(f.Name)).ToList();
				if (fields.Count == 0)
					return;
				Execute("UPDATE " + table.Name + " SET "
					+ string.Join(", ", fields.Where(f => f != idField).Select(f => f.Name + '=' + f.Quote(data[f.Name], Quote)).ToArray())
					+ " WHERE " + index.Where(data, Quote));
			} else {
				string sql = "INSERT INTO " + table.Name + " ("
					+ string.Join(", ", fields.Select(f => f.Name).ToArray()) + ") VALUES ("
					+ string.Join(", ", fields.Select(f => f.Quote(data[f.Name], Quote)).ToArray()) + ")";
				try {
					data[idName] = db.Insert(table, sql, false);
				} catch (DatabaseException) {
					throw;
				} catch (Exception ex) {
					throw new DatabaseException(ex, sql);
				}
			}
		}

		/// <summary>
		/// Class to time queries, and log if they exceed MaxTime (default Config.Default.SlowQuery)
		/// </summary>
		public class Timer : IDisposable {
			DateTime _start;
			string _message;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="message"></param>
			public Timer(string message) {
				_start = Utils.Now;
				_message = message;
			}

			/// <summary>
			/// Check elapsed time and log if it exceeds MaxTime
			/// </summary>
			public void Dispose() {
				double elapsed = (Utils.Now - _start).TotalMilliseconds;
				if (elapsed > MaxTime)
					Log.Trace.WriteLine("{0}:{1}", elapsed, _message);
			}

			/// <summary>
			/// Max time (default Config.Default.SlowQuery)
			/// </summary>
			public double MaxTime = Config.Default.SlowQuery;
		}

	}

	/// <summary>
	/// Store details about a Foreign Key field
	/// </summary>
	public class ForeignKey {
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="table">Table the key refers to</param>
		/// <param name="field">Field in that table</param>
		public ForeignKey(Table table, Field field) {
			Table = table;
			Field = field;
		}

		/// <summary>
		/// Table the key refers to
		/// </summary>
		public Table Table { get; private set; }

		/// <summary>
		/// Field in that table
		/// </summary>
		public Field Field { get; private set; }
	}

	/// <summary>
	/// Store details about a database field
	/// </summary>
	public class Field {

		/// <summary>
		/// Constructor
		/// </summary>
		public Field(string name) {
			Name = name;
			Type = typeof(string);
		}

		/// <summary>
		/// Full constructor
		/// </summary>
		/// <param name="name">Field name</param>
		/// <param name="type">C# type</param>
		/// <param name="length">Length</param>
		/// <param name="nullable">Whether it may be null</param>
		/// <param name="autoIncrement">Whether it is auto increment</param>
		/// <param name="defaultValue">Default value (or null)</param>
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
				if (type == typeof(bool) || type == typeof(int) || type.IsEnum || type == typeof(long) || type == typeof(decimal) || type == typeof(double)) {
					defaultValue = "0";
				} else if (type == typeof(string)) {
					defaultValue = "";
				}
			}
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Whether the field is auto increment
		/// </summary>
		public bool AutoIncrement { get; private set; }

		/// <summary>
		/// Default value
		/// </summary>
		public string DefaultValue { get; private set; }

		/// <summary>
		/// Foreign key details (or null)
		/// </summary>
		public ForeignKey ForeignKey;

		/// <summary>
		/// Length
		/// </summary>
		public decimal Length { get; private set; }

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Whether the field may be null
		/// </summary>
		public bool Nullable { get; private set; }

		/// <summary>
		/// Quote value of object o to use in a SQL statement, assuming value is to be placed in this field.
		/// </summary>
		public string Quote(object o, Func<object, string> quote) {
			if (o == null || o == DBNull.Value || (o is JToken && ((JToken)o).Type == JTokenType.Null)) return quote(null);
			if (Type.IsEnum)
				return quote(Convert.ChangeType(o, typeof(int)));
			string s = o.ToString();
			if ((Type.IsEnum || Type == typeof(int) || Type == typeof(long) || Type == typeof(decimal) || Type == typeof(double) || Type == typeof(DateTime) || Type == typeof(bool)) && s == "") return quote(null);
			if (Type == typeof(bool)) {
				// Accept numeric value as bool (non-zero = true)
				if (Regex.IsMatch(s, @"^\d+$"))
					return quote(int.Parse(s) > 0 ? "1" : "0");
			}
			try {
				o = Convert.ChangeType(o.ToString(), Type);
			} catch (Exception ex) {
				throw new CheckException(ex, "Invalid value for {0} field {1} '{2}'", Type.Name, Name, o);
			}
			return quote(o);
		}

		/// <summary>
		/// C# type
		/// </summary>
		public Type Type { get; private set; }

		/// <summary>
		/// String representation of C# type, allowing for Nullable.
		/// E.g. a Nullable Int32 will retuen "int?"
		/// </summary>
		public string DatabaseTypeName {
			get {
				string name = Type.Name;
				switch (name) {
					case "Int64":
						return Nullable ? "long?" : "long";
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
						if(Type.IsEnum)
                            return "int";
                        return name;
				}
			}
		}

		/// <summary>
		/// String description of Field
		/// </summary>
		/// <param name="view">Whether this field is in a view</param>
		public string Data(bool view) {
			string s = Name + "(" + DatabaseTypeName;
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

		/// <summary>
		/// Create a Field object from the Attributes on a C# class field (unless it has a DoNotStore attribute)
		/// </summary>
		/// <param name="field">The C# FieldInfo for the field</param>
		/// <param name="pk">Set to PrimaryAttribute if the field has one</param>
		public static Field FieldFor(FieldInfo field, out PrimaryAttribute pk) {
			pk = field.GetCustomAttribute<PrimaryAttribute>();
			if (field.IsDefined(typeof(DoNotStoreAttribute)))
				return null;
			return FieldFor(field.Name, field.FieldType, field.IsDefined(typeof(NullableAttribute)),
				pk, field.GetCustomAttribute<LengthAttribute>(),
				field.GetCustomAttribute<DefaultValueAttribute>());
		}

		/// <summary>
		/// Create a Field object from the Attributes on a C# class field
		/// </summary>
		/// <param name="field">The C# FieldInfo for the field</param>
		public static Field FieldFor(FieldInfo field) {
			return FieldFor(field.Name, field.FieldType, field.IsDefined(typeof(NullableAttribute)),
				field.GetCustomAttribute<PrimaryAttribute>(), field.GetCustomAttribute<LengthAttribute>(),
				field.GetCustomAttribute<DefaultValueAttribute>());
		}

		/// <summary>
		/// Create a Field object from the Attributes on a C# class property
		/// </summary>
		/// <param name="field">The C# PropertyInfo for the field</param>
		public static Field FieldFor(PropertyInfo field) {
			return FieldFor(field.Name, field.PropertyType, field.IsDefined(typeof(NullableAttribute)),
				field.GetCustomAttribute<PrimaryAttribute>(), field.GetCustomAttribute<LengthAttribute>(),
				field.GetCustomAttribute<DefaultValueAttribute>());
		}

		/// <summary>
		/// Create a Field object from the Attributes on a C# class field
		/// </summary>
		public static Field FieldFor(string name, Type pt, bool nullable, PrimaryAttribute pk, LengthAttribute la, 
			DefaultValueAttribute da) {
			decimal length = 0;
			string defaultValue = null;
			if (pt == typeof(bool?)) {
				pt = typeof(bool);
				nullable = true;
			} else if (pt == typeof(int?)) {
				pt = typeof(int);
				nullable = true;
			} else if (pt == typeof(long?)) {
				pt = typeof(long);
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
            if (pk != null)
				nullable = false;
			if (pt == typeof(bool)) {
				length = 1;
				defaultValue = "0";
			} else if (pt == typeof(int) || pt.IsEnum) {
				length = 11;
				defaultValue = "0";
			} else if (pt == typeof(long)) {
				length = 20;
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
			if (la != null)
				length = la.Length + la.Precision / 10M;
			if (da != null)
				defaultValue = da.Value;
			return new Field(name, pt, length, nullable, pk != null && pk.AutoIncrement, defaultValue);
		}


		/// <summary>
		/// String representation (for debugging)
		/// </summary>
		public override string ToString() {
			return Name + "(" + DatabaseTypeName + ")";
		}
	}

	/// <summary>
	/// Index descriptor
	/// </summary>
	public class Index {

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Index name</param>
		/// <param name="fields">Fields making up the index</param>
		/// <param name="unique">If the index is unique</param>
		public Index(string name, bool unique, params Field[] fields) {
			Name = name;
			Fields = fields;
			Unique = unique;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Index name</param>
		/// <param name="fields">Field names making up the index</param>
		/// <param name="unique">If the index is unique</param>
		public Index(string name, bool unique, params string[] fields) {
			Name = name;
			Fields = fields.Select(f => new Field(f)).ToArray();
			Unique = unique;
		}

		/// <summary>
		/// Whether this index has values in the data for all of its fields
		/// </summary>
		public bool CoversData(JObject data) {
			return (Unique && Fields.Where(f => data.IsMissingOrNull(f.Name)).FirstOrDefault() == null);
		}

		/// <summary>
		/// List of fields (with types) in the index, separate by commas - for matching two indexes to see if they are the same
		/// </summary>
		public string FieldList {
			get { return string.Join(",", Fields.Select(f => f.ToString()).ToArray()); }
		}

		/// <summary>
		/// The fields that go to make up the index
		/// </summary>
		public Field[] Fields { get; private set; }

		/// <summary>
		/// Whether this index is the same as the other one
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Matches(Index other) {
			if (Unique != other.Unique || Fields.Length != other.Fields.Length)
				return false;
			for (int i = 0; i < Fields.Length; i++) {
				Field mine = Fields[i], theirs = other.Fields[i];
				if (mine.Name != theirs.Name || mine.Type != theirs.Type)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Index name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Index is unique
		/// </summary>
		public bool Unique { get; private set; }

		/// <summary>
		/// Generate a WHERE clause (without the "WHERE") to select the record matching data for this index
		/// </summary>
		public string Where(JObject data, Func<object, string> quote) {
			return string.Join(" AND ", Fields.Select(f => f.Name + "=" + f.Quote(data[f.Name], quote)).ToArray());
		}

		/// <summary>
		/// For debugging
		/// </summary>
		public override string ToString() {
			return "I:" + Name + "=" + FieldList;
		}
	}

	/// <summary>
	/// Table definition
	/// </summary>
	public class Table {

		/// <summary>
		/// Constructor
		/// </summary>
		public Table(string name, Field[] fields, Index[] indexes) {
			Name = name;
			Fields = fields;
			Indexes = indexes;
		}

		/// <summary>
		/// For TableForOrDefault - just provides JObject type conversion
		/// </summary>
		public Table(Type t) {
			Name = t.Name;
			Type = t;
		}

		/// <summary>
		/// The fields in the table
		/// </summary>
		public Field[] Fields;

		/// <summary>
		/// Find a field by name (null if not found)
		/// </summary>
		public Field FieldFor(string name) {
			return Fields.FirstOrDefault(f => f.Name == name);
		}

		/// <summary>
		/// Find foreign key in this table that refers to target table (null if not found)
		/// </summary>
		public Field ForeignKeyFieldFor(Table table) {
			return Fields.FirstOrDefault(f => f.ForeignKey != null && f.ForeignKey.Table.Name == table.Name);
		}

		/// <summary>
		/// The indexes
		/// </summary>
		public Index[] Indexes { get; private set; }

		/// <summary>
		/// Find the first index for which there are values for all fields in data (null if none)
		/// </summary>
		public Index IndexFor(JObject data) {
			return Indexes.Where(i => i.CoversData(data)).FirstOrDefault();
		}

		/// <summary>
		/// Table name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Primary key (first index)
		/// </summary>
		public Field PrimaryKey {
			get { return Indexes[0].Fields[0]; }
		}

		/// <summary>
		/// The C# type to which this table relates
		/// </summary>
		public Type Type;

		/// <summary>
		/// Table to update if update is called on a View
		/// </summary>
		public virtual Table UpdateTable { get { return this; } }

		/// <summary>
		/// Whether this is a View rather than a native table
		/// </summary>
		public virtual bool IsView { get { return false; } }

		/// <summary>
		/// Convert JObject to type T
		/// If the table type is a subclass of T, return the table type cast to T
		/// </summary>
		public T FromJson<T>(JObject o) {
			Type t = typeof(T);
			if (Type.IsSubclassOf(t))
				t = Type;
			return (T)o.ToObject(t);
		}

		/// <summary>
		/// For debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return "T:" + string.Join(",", Fields.Select(f => f.ToString()).ToArray()) + "\r\n"
				+ string.Join("\r\n", Indexes.Select(i => i.ToString()).ToArray());
		}
	}

	/// <summary>
	/// View definition
	/// </summary>
	public class View : Table {
		Table _updateTable;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">View name</param>
		/// <param name="fields">Fields</param>
		/// <param name="indexes">Indexes</param>
		/// <param name="sql">SQL to generate data</param>
		/// <param name="updateTable">Table to update if update called on a view record</param>
		public View(string name, Field[] fields, Index[] indexes, string sql, Table updateTable)
			: base(name, fields, indexes) {
			Sql = sql;
			_updateTable = updateTable;
		}

		/// <summary>
		/// SQL to generate data
		/// </summary>
		public string Sql { get; private set; }

		/// <summary>
		/// Table to update if update called on a record from the view
		/// </summary>
		public override Table UpdateTable { get { return _updateTable; } }

		/// <summary>
		/// Whether this is a view (always true)
		/// </summary>
		public override bool IsView { get { return true; } }

	}

	/// <summary>
	/// An enumerable of JObjects with a JArray converter, to efficiently handle output of a query
	/// </summary>
	public class JObjectEnumerable : IEnumerable<JObject> {
		IEnumerable<JObject> _e;

		/// <summary>
		/// Constructor
		/// </summary>
		public JObjectEnumerable(IEnumerable<JObject> e) {
			_e = e;
		}

		/// <summary>
		/// ToList converter. If called, the enumerable itself is converted to a list, so that it won't be enumerated again.
		/// </summary>
		/// <returns></returns>
		public List<JObject> ToList() {
			List<JObject> e = _e.ToList();
			_e = e;
			return e;
		}

		/// <summary>
		/// For debugging
		/// </summary>
		public override string ToString() {
			return this.ToJson();
		}

		/// <summary>
		/// Standard GetEnumerator
		/// </summary>
		public IEnumerator<JObject> GetEnumerator() {
			return _e.GetEnumerator();
		}

		/// <summary>
		/// Standard GetEnumerator
		/// </summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return _e.GetEnumerator();
		}

		/// <summary>
		/// Convert to a JArray
		/// </summary>
		static public implicit operator JArray(JObjectEnumerable o) {
			JArray j = new JArray();
			foreach (JObject jo in o) {
				j.Add(jo);
			}
			return j;
		}
	}

	/// <summary>
	/// Base class for all [Table] C# objects
	/// </summary>
	public class JsonObject {

		/// <summary>
		/// Convert to a JObject
		/// </summary>
		public JObject ToJObject() {
			return JObject.FromObject(this);
		}

		/// <summary>
		/// Make a copy
		/// </summary>
		public T Clone<T>() {
			return this.ToJObject().To<T>();
		}

		/// <summary>
		/// Record id
		/// </summary>
		public virtual int? Id {
			get { FieldInfo id = idField; return id == null ? null : (int?)id.GetValue(this); }
			set { FieldInfo id = idField; if(id != null) id.SetValue(this, value); }
		}
		
		FieldInfo idField {
			get {
			FieldInfo found = null;
				foreach(FieldInfo f in GetType().GetFieldsInOrder(BindingFlags.Public | BindingFlags.Instance)) {
					PrimaryAttribute p = f.GetCustomAttribute<PrimaryAttribute>();
					if(p != null) {
						if(p.Sequence > 0 || f.FieldType != typeof(int?))
							return null;
						found = f;
					}
				}
				return found;
			}
		}

		/// <summary>
		/// For debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return this.ToJson();
		}

	}

	/// <summary>
	/// Sorted list of tables, such that tables referred to in a foreign key come before the referring table,
	/// and tables referred to in a view come before the view
	/// </summary>
	public class TableList : List<Table> {
		List<Table> _allTables;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="allTables">The tables to add to the list</param>
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

	/// <summary>
	/// Exception thrown by database code - contains the SQL and/or the table causing the exception
	/// </summary>
	public class DatabaseException : Exception {

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ex">Exception caught</param>
		/// <param name="table">Table causing the exception</param>
		public DatabaseException(DatabaseException ex, Table table)
			: base(ex.InnerException.Message, ex.InnerException) {
			Sql = ex.Sql;
			Table = table.Name;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ex">Exception caught</param>
		/// <param name="sql">SQL causing the exception</param>
		public DatabaseException(Exception ex, string sql)
			: base(ex.Message, ex) {
			Sql = sql;
		}

		/// <summary>
		/// SQL causing the exception
		/// </summary>
		public string Sql;

		/// <summary>
		/// Table causing the exception
		/// </summary>
		public string Table;

		/// <summary>
		/// Message (also shows table causing the exception, if known)
		/// </summary>
		public override string Message {
			get {
				return Table == null ? base.Message : Table + ":" + base.Message;
			}
		}

		/// <summary>
		/// Returns exception string and SQL
		/// </summary>
		public override string ToString() {
			return base.ToString() + "\r\nSQL:" + Sql;

		}
	}

}
