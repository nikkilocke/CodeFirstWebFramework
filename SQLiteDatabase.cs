using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	/// <summary>
	/// Interface to SQLite
	/// </summary>
	public class SQLiteDatabase : DbInterface {
		static object _lock = new object();
		SQLiteConnection _conn;
		SQLiteTransaction _tran;
		Database _db;
		bool _viewsDropped;

		/// <summary>
		/// Static constructor registers the extension functions to make SQLite more like MySql
		/// </summary>
		static SQLiteDatabase() {
			SQLiteDateDiff.RegisterFunction(typeof(SQLiteDateDiff));
			SQLiteSum.RegisterFunction(typeof(SQLiteSum));
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public SQLiteDatabase(Database db, string connectionString) {
			_db = db;
			createDatabase(connectionString);
			_conn = new SQLiteConnection();
			_conn.ConnectionString = connectionString;
			_conn.Open();
		}

		/// <summary>
		/// Begin transaction
		/// </summary>
		public void BeginTransaction() {
			lock (_lock) {
				if (_tran == null)
					_tran = _conn.BeginTransaction();
			}
		}

		/// <summary>
		/// Return SQL to cast a value to a type
		/// </summary>
		public string Cast(string value, string type) {
			return value;
		}

		/// <summary>
		/// Clean up database
		/// </summary>
		public void CleanDatabase() {
			foreach (string table in _db.TableNames) {
				Table t = _db.TableFor(table);
				if(t.PrimaryKey.AutoIncrement)
					Execute(string.Format("UPDATE sqlite_sequence SET seq = (SELECT MAX({1}) FROM {0}) WHERE name='{0}'",
						table, t.PrimaryKey.Name));
			}
			Execute("VACUUM");
		}

		/// <summary>
		/// Create a table from a Table definition
		/// </summary>
		public void CreateTable(Table t) {
			View v = t as View;
			if (v != null) {
				executeLog(string.Format("CREATE VIEW `{0}` AS {1}", v.Name, v.Sql));
				return;
			}
			createTable(t, t.Name);
			createIndexes(t);
		}

		/// <summary>
		/// Create an index from a table and index definition
		/// </summary>
		public void CreateIndex(Table t, Index index) {
			executeLog(index.Unique ? string.Format("ALTER TABLE `{0}` ADD CONSTRAINT `{1}` UNIQUE ({2})", t.Name, index.Name,
				string.Join(",", index.Fields.Select(f => "`" + f.Name + "` ASC").ToArray())) :
				string.Format("ALTER TABLE `{0}` ADD INDEX `{1}` ({2})", t.Name, index.Name,
				string.Join(",", index.Fields.Select(f => "`" + f.Name + "` ASC"))));
		}

		/// <summary>
		/// Commit transaction
		/// </summary>
		public void Commit() {
			if (_tran != null) {
				lock (_lock) {
					_tran.Commit();
					_tran.Dispose();
					_tran = null;
				}
			}
		}

		/// <summary>
		/// Roll back any uncommitted transaction and close the connection
		/// </summary>
		public void Dispose() {
			Rollback();
			if (_conn != null) {
				_conn.Dispose();
				_conn = null;
			}
		}

		/// <summary>
		/// Drop a table
		/// </summary>
		public void DropTable(Table t) {
			executeLogSafe("DROP TABLE IF EXISTS " + t.Name);
			executeLogSafe("DROP VIEW IF EXISTS " + t.Name);
		}

		/// <summary>
		/// Drop an index
		/// </summary>
		public void DropIndex(Table t, Index index) {
			executeLogSafe(string.Format("ALTER TABLE `{0}` DROP INDEX `{1}`", t.Name, index.Name));
		}

		/// <summary>
		/// Return SQL to cast a value to a type
		/// </summary>
		public string GroupConcat(string expression, string separator = null) {
			return $"GROUP_CONCAT({expression}{(separator == null ? "" : "," + Quote(separator))})";
		}

		/// <summary>
		/// Execute arbitrary sql
		/// </summary>
		public int Execute(string sql) {
			lock (_lock) {
				using (SQLiteCommand cmd = command(sql)) {
					return cmd.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// Insert a record
		/// </summary>
		/// <param name="table">Table</param>
		/// <param name="sql">SQL INSERT statement</param>
		/// <param name="updatesAutoIncrement">True if the insert may update an auto-increment field</param>
		/// <returns>The value of the auto-increment record id of the newly inserted record</returns>
		public int Insert(Table table, string sql, bool updatesAutoIncrement) {
			lock (_lock) {
				using (SQLiteCommand cmd = command(sql)) {
					cmd.ExecuteNonQuery();
					if (!table.PrimaryKey.AutoIncrement)
						return 0;
					cmd.CommandText = "select last_insert_rowid()";
					return (int)(Int64)cmd.ExecuteScalar();
				}
			}
		}

		/// <summary>
		/// Do the table names in code and database match (some implementations are case insensitive)
		/// </summary>
		public bool TableNamesMatch(Table code, Table database) {
			return string.Compare(code.Name, database.Name, false) == 0;
		}

		/// <summary>
		/// Do the fields in code and database match (some implementations are case insensitive)
		/// </summary>
		public bool FieldsMatch(Table t, Field code, Field database) {
			if (code.DatabaseTypeName != database.DatabaseTypeName) return false;
			if (t.IsView) return true;	// Database does not always give correct values for view columns
			if (code.AutoIncrement != database.AutoIncrement) return false;
			if (code.Length != database.Length) return false;
			if (code.Nullable != database.Nullable) return false;
			if (code.DefaultValue != database.DefaultValue) return false;
			return true;
		}

		/// <summary>
		/// Query the database, and return JObjects for each record returned
		/// </summary>
		public IEnumerable<JObject> Query(string query) {
			lock (_lock) {
				using (SQLiteCommand cmd = command(query)) {
					using (SQLiteDataReader r = executeReader(cmd, query)) {
						JObject row;
						while ((row = readRow(r, query)) != null) {
							yield return row;
						}
					}
				}
			}
		}

		/// <summary>
		/// Query the database, and return the first record matching the query
		/// </summary>
		public JObject QueryOne(string query) {
			return _db.Query(query + " LIMIT 1").FirstOrDefault();
		}

		/// <summary>
		/// Quote any kind of data for inclusion in a SQL query
		/// </summary>
		public string Quote(object o) {
			if (o == null || o == DBNull.Value) return "NULL";
			if (o is int || o is long || o is double) return o.ToString();
			if (o is decimal) return ((decimal)o).ToString("0.00");
			if (o is double) return (Math.Round((decimal)o, 4)).ToString();
			if (o is double) return ((decimal)o).ToString("0");
			if (o is bool) return (bool)o ? "1" : "0";
			if (o is DateTime) {
				DateTime dt = (DateTime)o;
				return "'" + dt.ToString(dt.TimeOfDay == TimeSpan.Zero ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm:ss") + "'";
			}
			return "'" + o.ToString().Replace("'", "''") + "'";
		}

		/// <summary>
		/// Quote a field or table name when creating SQL
		/// </summary>
		public string QuoteName(string name) {
			return '`' + name + '`';
		}

		/// <summary>
		/// Rollback transaction
		/// </summary>
		public void Rollback() {
			if (_tran != null) {
				lock (_lock) {
					_tran.Rollback();
					_tran.Dispose();
					_tran = null;
				}
			}
		}

		/// <summary>
		/// Get a Dictionary of existing tables in the database
		/// </summary>
		public Dictionary<string, Table> Tables() {
			Dictionary<string, Table> tables = new Dictionary<string, Table>();
			createDatabase(Config.Default.ConnectionString);
			DataTable tabs = _conn.GetSchema("Tables");
			DataTable cols = _conn.GetSchema("Columns");
			DataTable fkeyCols = _conn.GetSchema("ForeignKeys");
			DataTable indexes = _conn.GetSchema("Indexes");
			DataTable indexCols = _conn.GetSchema("IndexColumns");
			DataTable views = _conn.GetSchema("Views");
			DataTable viewCols = _conn.GetSchema("ViewColumns");
			foreach(DataRow table in tabs.Rows) {
				string name = table["TABLE_NAME"].ToString();
				string filter = "TABLE_NAME = " + Quote(name);
				Field[] fields = cols.Select(filter, "ORDINAL_POSITION")
					.Select(c => new Field(c["COLUMN_NAME"].ToString(), typeFor(c["DATA_TYPE"].ToString()), 
						lengthFromColumn(c), c["IS_NULLABLE"].ToString() == "True", c["AUTOINCREMENT"].ToString() == "True", 
						defaultFromColumn(c))).ToArray();
				List<Index> tableIndexes = new List<Index>();
				foreach (DataRow ind in indexes.Select(filter + " AND PRIMARY_KEY = 'True'")) {
					string indexName = ind["INDEX_NAME"].ToString();
					tableIndexes.Add(new Index("PRIMARY", true, 
						indexCols.Select(filter + " AND INDEX_NAME = " + Quote(indexName), "ORDINAL_POSITION")
						.Select(r => fields.First(f => f.Name == r["COLUMN_NAME"].ToString())).ToArray()));
				}
				foreach (DataRow ind in indexes.Select(filter + " AND PRIMARY_KEY = 'False'")) {
					string indexName = ind["INDEX_NAME"].ToString();
					if (!indexName.StartsWith("fk_")) {
						string prefix = "i_" + name + "_";
						string indName = indexName.StartsWith(prefix) ? indexName.Substring(prefix.Length) : indexName;
						tableIndexes.Add(new Index(indName, ind["UNIQUE"].ToString() == "True",
							indexCols.Select(filter + " AND INDEX_NAME = " + Quote(indexName), "ORDINAL_POSITION")
							.Select(r => fields.First(f => f.Name == r["COLUMN_NAME"].ToString())).ToArray()));
					}
				}
				tables[name] = new Table(name, fields, tableIndexes.ToArray());
			}
			foreach (DataRow fk in fkeyCols.Rows) {
				Table detail = tables[fk["TABLE_NAME"].ToString()];
				Table master = tables[fk["FKEY_TO_TABLE"].ToString()];
				Field masterField = master.FieldFor(fk["FKEY_TO_COLUMN"].ToString());
				detail.FieldFor(fk["FKEY_FROM_COLUMN"].ToString()).ForeignKey = new ForeignKey(master, masterField);
			}
			foreach (DataRow table in views.Select()) {
				string name = table["TABLE_NAME"].ToString();
				string filter = "VIEW_NAME = " + Quote(name);
				Field[] fields = viewCols.Select(filter, "ORDINAL_POSITION")
					.Select(c => new Field(c["VIEW_COLUMN_NAME"].ToString(), typeFor(c["DATA_TYPE"].ToString()), 
						lengthFromColumn(c), c["IS_NULLABLE"].ToString() == "True", false,
						defaultFromColumn(c))).ToArray();
				Table updateTable = null;
				tables.TryGetValue(Regex.Replace(name, "^.*_", ""), out updateTable);
				tables[name] = new View(name, fields, fields.Length > 0 ? new Index[] { new Index("PRIMARY", true, fields[0]) } : new Index[0],
					table["VIEW_DEFINITION"].ToString(), updateTable);
			}
			return tables;
		}

        /// <summary>
        /// Get a Dictionary of existing tables in the database
        /// </summary>
        void dropAllViews() {
            if (!_viewsDropped) {
                // Need to drop all views, in case one of them depends on this table
                DataTable views = _conn.GetSchema("Views");
                List<string> viewNames = new List<string>(views.Select().Select(table => table["TABLE_NAME"].ToString())).ToList();
                foreach (string view in viewNames)
                    executeLog($"DROP VIEW IF EXISTS `{view}`");
                _viewsDropped = true;
            }
        }

        /// <summary>
        /// Upgrade the table definition
        /// </summary>
        /// <param name="code">Defintiion required, from code</param>
        /// <param name="database">Definition in database</param>
        /// <param name="insert">Fields to insert</param>
        /// <param name="update">Fields to change</param>
        /// <param name="remove">Fields to remove</param>
        /// <param name="insertFK">Foreign keys to insert</param>
        /// <param name="dropFK">Foreign keys to remove</param>
        /// <param name="insertIndex">Indexes to insert</param>
        /// <param name="dropIndex">Indexes to remove</param>
        public void UpgradeTable(Table code, Table database, List<Field> insert, List<Field> update, List<Field> remove,
			List<Field> insertFK, List<Field> dropFK, List<Index> insertIndex, List<Index> dropIndex) {
				for (int i = dropIndex.Count; i-- > 0; ) {
					Index ind = dropIndex[i];
					if ((ind.Fields.Length == 1 && ind.Fields[0].Name == code.PrimaryKey.Name) || ind.Name.StartsWith("sqlite_autoindex_"))
						dropIndex.RemoveAt(i);
				}
			if (update.Count > 0 || remove.Count > 0 || insertFK.Count > 0 || dropFK.Count > 0 || insertIndex.Count > 0 || dropIndex.Count > 0) {
				reCreateTable(code, database);
				return;
			}
			if (insert.Count != 0) {
				foreach(string def in insert.Select(f => "ADD COLUMN " + fieldDef(f))) {
					executeLog(string.Format("ALTER TABLE `{0}` {1}", code.Name, def));
				}
			}
		}

		/// <summary>
		/// Do the views in code and database match
		/// </summary>
		public bool? ViewsMatch(View code, View database) {
			return false;	// Always return false, in case views have been dropped
		}

		SQLiteCommand command(string sql) {
			try {
				return new SQLiteCommand(sql, _conn, _tran);
			} catch (Exception ex) {
				throw new DatabaseException(ex, sql);
			}
		}

		static void createDatabase(string connectionString) {
			Match m = Regex.Match(connectionString, @"Data Source=([^;]+)", RegexOptions.IgnoreCase);
			if (m.Success && !File.Exists(m.Groups[1].Value)) {
				Log.Startup.WriteLine("Creating SQLite database {0}", m.Groups[1].Value);
				Directory.CreateDirectory(Path.GetDirectoryName(m.Groups[1].Value));
				SQLiteConnection.CreateFile(m.Groups[1].Value);
			}
		}

		void createTable(Table t, string name) {
			List<string> defs = new List<string>(t.Fields.Select(f => fieldDef(f)));
			for (int i = 0; i < t.Indexes.Length; i++) {
				Index index = t.Indexes[i];
				if (i == 0) {
					if (index.Fields.Length != 1 || !index.Fields[0].AutoIncrement)
						defs.Add(string.Format("CONSTRAINT `PRIMARY` PRIMARY KEY ({0})", string.Join(",", index.Fields
							.Select(f => "`" + f.Name + "`").ToArray())));
				} else if(index.Unique)
					defs.Add(string.Format("CONSTRAINT `{0}` UNIQUE ({1})", index.Name,
						string.Join(",", index.Fields.Select(f => "`" + f.Name + "` ASC"))));
			}
			defs.AddRange(t.Fields.Where(f => f.ForeignKey != null).Select(f => string.Format(@"CONSTRAINT `fk_{0}_{1}_{2}`
    FOREIGN KEY (`{2}`)
    REFERENCES `{1}` (`{3}`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION", t.Name, f.ForeignKey.Table.Name, f.Name, f.ForeignKey.Table.PrimaryKey.Name)));
			executeLog(string.Format("CREATE TABLE `{0}` ({1})", name, string.Join(",\r\n", defs.ToArray())));
		}

		void createIndexes(Table t) {
			foreach (string sql in t.Fields.Where(f => f.ForeignKey != null && t.Indexes.FirstOrDefault(i => i.Fields[0] == f) == null)
				.Select(f => string.Format(@"CREATE INDEX `fk_{0}_{1}_{2}_idx` ON {0} (`{2}` ASC)",
				t.Name, f.ForeignKey.Table.Name, f.Name)))
				executeLog(sql);
			foreach (string sql in t.Indexes.Where(i => !i.Unique)
				.Select(i => string.Format(@"CREATE INDEX `i_{0}_{1}` ON {0} ({2})",
				t.Name, i.Name, string.Join(",", i.Fields.Select(f => "`" + f.Name + "` ASC")))))
				executeLog(sql);
		}

		static string defaultFromColumn(DataRow def) {
			if (def.IsNull("COLUMN_DEFAULT"))
				return null;
			string r = def["COLUMN_DEFAULT"].ToString();
			Match m = Regex.Match(r, @"^'(.*)'$");
			return m.Success ? m.Groups[1].Value : r;
		}

		int executeLog(string sql) {
			Log.Startup.WriteLine(sql);
			lock (_lock) {
				using (SQLiteCommand cmd = command(sql)) {
					return cmd.ExecuteNonQuery();
				}
			}
		}

		int executeLogSafe(string sql) {
			try {
				return executeLog(sql);
			} catch (Exception ex) {
				Log.Error.WriteLine(ex.Message);
				return -1;
			}
		}

		SQLiteDataReader executeReader(SQLiteCommand cmd, string sql) {
			try {
				return cmd.ExecuteReader();
			} catch (Exception ex) {
				throw new DatabaseException(ex, sql);
			}
		}

		string fieldDef(Field f) {
			StringBuilder b = new StringBuilder();
			b.AppendFormat("`{0}` ", f.Name);
            string defaultValue = f.DefaultValue;
            switch (f.Type.Name) {
				case "Int64":
					b.Append("BIGINT");
					break;
				case "Int32":
					b.Append("INTEGER");
					break;
				case "Decimal":
					b.AppendFormat("DECIMAL({0})", f.Length.ToString("0.0").Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, ","));
					break;
				case "Double":
					b.Append("DOUBLE");
					break;
				case "Boolean":
					b.Append("BIT");
					break;
				case "DateTime":
					b.Append("DATETIME");
                    if (defaultValue == null)
                        defaultValue = "1900-01-01";
                    break;
				case "String":
					if (f.Length == 0)
						b.Append("TEXT");
					else
						b.AppendFormat("VARCHAR({0})", f.Length);
					b.Append(" COLLATE NOCASE");
					break;
				default:
					if(f.Type.IsEnum) {
                        b.Append("INTEGER");
                        break;
                    }
                    throw new CheckException("Unknown type {0}", f.Type.Name);
			}
			if (f.AutoIncrement)
				b.Append(" PRIMARY KEY AUTOINCREMENT");
			else {
				b.AppendFormat(" {0}NULL", f.Nullable ? "" : "NOT ");
				if (defaultValue != null)
					b.AppendFormat(" DEFAULT {0}", Quote(defaultValue));
			}
			return b.ToString();
		}

		decimal lengthFromColumn(DataRow c) {
			try {
				switch (c["DATA_TYPE"].ToString().ToLower()) {
					case "bigint":
						return 20;
					case "int":
					case "integer":
						return 11;
					case "tinyint":
					case "bit":
						return 1;
					case "decimal":
						string s = c["NUMERIC_PRECISION"] + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + c["NUMERIC_SCALE"];
						return s == "." ? 10.2M : decimal.Parse(s);
					case "double":
					case "float":
						return 10.4M;
					case "varchar":
						return Convert.ToDecimal(c["CHARACTER_MAXIMUM_LENGTH"]);
					default:
						return 0;
				}
			} catch (Exception ex) {
				Log.Error.WriteLine(ex.ToString());
				return 0;
			}
		}

		JObject readRow(SQLiteDataReader r, string sql) {
			try {
				lock (_lock) {
					if (!r.Read()) return null;
				}
				JObject row = new JObject();
				for (int i = 0; i < r.FieldCount; i++) {
					row.Add(Regex.Replace(r.GetName(i), @"^.*\.", ""), r[i].ToJToken());
				}
				return row;
			} catch (Exception ex) {
				throw new DatabaseException(ex, sql);
			}
		}

		void reCreateTable(Table code, Table database) {
			string newTable = "_NEW_" + code.Name;
			try {
				executeLogSafe("PRAGMA foreign_keys=OFF");
				executeLog("BEGIN TRANSACTION");
				// We may need to drop views, in case they depend on this table
				dropAllViews();
                createTable(code, newTable);
				executeLog(string.Format("INSERT INTO {0} ({2}) SELECT {2} FROM {1}", newTable, database.Name,
					string.Join(", ", code.Fields.Select(f => f.Name)
						.Where(f => database.Fields.FirstOrDefault(d => d.Name == f) != null).ToArray())));
				DropTable(database);
				executeLog("ALTER TABLE " + newTable + " RENAME TO " + code.Name);
				createIndexes(code);
				executeLog("PRAGMA foreign_key_check");
				executeLog("COMMIT TRANSACTION");
			} catch (Exception ex) {
				Log.Error.WriteLine("Exception: {0}", ex);
				executeLogSafe("ROLLBACK TRANSACTION");
				throw;
			} finally {
				executeLogSafe("PRAGMA foreign_keys=ON");
			}
		}

		static Type typeFor(string s) {
			switch (s.ToLower()) {
				case "bigint":
					return typeof(long);
				case "int":
				case "integer":
					return typeof(int);
				case "tinyint":
				case "bit":
					return typeof(bool);
				case "decimal":
					return typeof(decimal);
				case "double":
				case "float":
					return typeof(double);
				case "datetime":
				case "date":
					return typeof(DateTime);
				case "varchar":
				case "text":
				default:
					return typeof(string);
			}
		}

	}

	/// <summary>
	/// DATEDIFF function (like MySql's)
	/// </summary>
	[SQLiteFunctionAttribute(Name = "DATEDIFF", Arguments = 2, FuncType = FunctionType.Scalar)]
	class SQLiteDateDiff : SQLiteFunction {
		public override object Invoke(object[] args) {
			if (args.Length < 2 || args[0] == null || args[0] == DBNull.Value || args[1] == null || args[1] == DBNull.Value)
				return null;
			try {
				DateTime d1 = DateTime.Parse(args[0].ToString());
				DateTime d2 = DateTime.Parse(args[1].ToString());
				return (d1 - d2).TotalDays;
			} catch (Exception ex) {
				Log.Error.WriteLine("Exception: {0}", ex);
				return null;
			}
		}
	}

	[SQLiteFunctionAttribute(Name = "NOW", Arguments = 0, FuncType = FunctionType.Scalar)]
	class Now : SQLiteFunction {
		public override object Invoke(object[] args) {
			try {
				return Utils.Now.ToString("yyyy-MM-ddThh:mm:ss");
			} catch (Exception ex) {
				Log.Error.WriteLine("Exception: {0}", ex);
				return null;
			}
		}
	}

	/// <summary>
	/// SUM function which rounds as it sums, so it works like MySql's
	/// </summary>
	[SQLiteFunctionAttribute(Name = "SUM", Arguments = 1, FuncType = FunctionType.Aggregate)]
	class SQLiteSum : SQLiteFunction {
		public override void Step(object[] args, int stepNumber, ref object contextData) {
			if (args.Length < 1 || args[0] == null || args[0] == DBNull.Value)
				return;
			try {
				decimal d = Math.Round(Convert.ToDecimal(args[0]), 4);
				if (contextData != null) d += (Decimal)contextData;
				contextData = d;
			} catch (Exception ex) {
				Log.Error.WriteLine("Exception: {0}", ex);
			}
		}

		public override object Final(object contextData) {
			return contextData;
		}
	}

	/// <summary>
	/// CONCAT function - just like MySql
	/// </summary>
	[SQLiteFunctionAttribute(Name = "CONCAT", FuncType = FunctionType.Scalar)]
	class SQLiteConcat : SQLiteFunction {
		public override object Invoke(object[] args) {
			if (args.Length < 1 || args.Any(a => a == null || a == DBNull.Value))
				return null;
			try {
				return String.Join("", args.Select(a => a.ToString()).ToArray());
			} catch (Exception ex) {
				Log.Error.WriteLine("Exception: {0}", ex);
				return null;
			}
		}
	}
}
