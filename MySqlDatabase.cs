using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	/// <summary>
	/// Interface to MySql
	/// </summary>
	public class MySqlDatabase : DbInterface {
		string _connectionString;
		MySqlConnection _conn;
		MySqlTransaction _tran;
		Database _db;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="db"></param>
		/// <param name="connectionString"></param>
		public MySqlDatabase(Database db, string connectionString) {
			_db = db;
			_connectionString = connectionString;
			_conn = new MySqlConnection();
			_conn.ConnectionString = connectionString;
			_conn.Open();
		}

		/// <summary>
		/// Begin transaction
		/// </summary>
		public void BeginTransaction() {
			if (_tran == null)
				_tran = _conn.BeginTransaction();
		}

		/// <summary>
		/// Return SQL to cast a value to a type
		/// </summary>
		public string Cast(string value, string type) {
			return string.Format("CAST({0} AS {1})", value, type);
		}

		/// <summary>
		/// Clean up database
		/// </summary>
		public void CleanDatabase() {
			foreach (string table in _db.TableNames) {
				Execute("ALTER TABLE " + table + " AUTO_INCREMENT = 1");
				Execute("OPTIMIZE TABLE " + table);
			}
		}

		/// <summary>
		/// Commit transaction
		/// </summary>
		public void Commit() {
			if (_tran != null) {
				_tran.Commit();
				_tran.Dispose();
				_tran = null;
			}
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
			List<string> defs = new List<string>(t.Fields.Select(f => fieldDef(f)));
			for (int i = 0; i < t.Indexes.Length; i++) {
				Index index = t.Indexes[i];
				if (i == 0)
					defs.Add(string.Format("PRIMARY KEY ({0})", string.Join(",", index.Fields.Select(f => "`" + f.Name + "`").ToArray())));
				else
					defs.Add(string.Format("{2}INDEX `{0}` ({1})", index.Name,
						string.Join(",", index.Fields.Select(f => "`" + f.Name + "` ASC")), index.Unique ? "UNIQUE " : ""));
			}
			defs.AddRange(t.Fields.Where(f => f.ForeignKey != null).Select(f => string.Format(@"CONSTRAINT `fk_{0}_{1}_{2}`
    FOREIGN KEY (`{2}`)
    REFERENCES `{1}` (`{3}`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION", t.Name, f.ForeignKey.Table.Name, f.Name, f.ForeignKey.Table.PrimaryKey.Name)));
			defs.AddRange(t.Fields.Where(f => f.ForeignKey != null && t.Indexes.FirstOrDefault(i => i.Fields[0] == f) == null).Select(f => string.Format(@"INDEX `fk_{0}_{1}_{2}_idx` (`{2}` ASC)",
				t.Name, f.ForeignKey.Table.Name, f.Name)));
			executeLog(string.Format("CREATE TABLE `{0}` ({1}) ENGINE=InnoDB", t.Name, string.Join(",\r\n", defs.ToArray())));
		}

		/// <summary>
		/// Create an index from a table and index definition
		/// </summary>
		public void CreateIndex(Table t, Index index) {
			executeLog(string.Format("ALTER TABLE `{0}` ADD {3} INDEX `{1}` ({2})", t.Name, index.Name,
				string.Join(",", index.Fields.Select(f => "`" + f.Name + "` ASC")), index.Unique ? "UNIQUE " : ""));
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
		/// Execute arbitrary sql
		/// </summary>
		public int Execute(string sql) {
			using (MySqlCommand cmd = command(sql)) {
				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Return SQL to cast a value to a type
		/// </summary>
		public string GroupConcat(string expression, string separator = null) {
			return $"GROUP_CONCAT({expression}{(separator == null ? "" : " SEPARATOR " + Quote(separator))})";
		}

		/// <summary>
		/// Insert a record
		/// </summary>
		/// <param name="table">Table</param>
		/// <param name="sql">SQL INSERT statement</param>
		/// <param name="updatesAutoIncrement">True if the insert may update an auto-increment field</param>
		/// <returns>The value of the auto-increment record id of the newly inserted record</returns>
		public int Insert(Table table, string sql, bool updatesAutoIncrement) {
			using (MySqlCommand cmd = command(sql)) {
				cmd.ExecuteNonQuery();
				if (!table.PrimaryKey.AutoIncrement)
					return 0;
				return (int)cmd.LastInsertedId;
			}
		}

		/// <summary>
		/// Do the table names in code and database match (some implementations are case insensitive)
		/// </summary>
		public bool TableNamesMatch(Table code, Table database) {
			return string.Compare(code.Name, database.Name, true) == 0;
		}

		/// <summary>
		/// Determine whether two fields are the same
		/// </summary>
		public bool FieldsMatch(Table t, Field code, Field database) {
			if (code.DatabaseTypeName != database.DatabaseTypeName) return false;
			if (t.IsView) return true;	// Database does not always give correct values for view columns
			if (code.AutoIncrement != database.AutoIncrement) return false;
			if (code.Length != database.Length) {
				return false;
			}
			// NB: LONGTEXT Fields (length 0) MUST be nullable in MySql, otherwise you get errors because they do not allow default values
			if (code.Nullable != database.Nullable && (database.Length != 0 || !database.Nullable)) return false;
			// NB: MySql cannot show the difference between null and empty string default values!
			if(code.DatabaseTypeName == "string" && string.IsNullOrEmpty(code.DefaultValue) && string.IsNullOrEmpty(database.DefaultValue)) return true;
			if (code.DefaultValue != database.DefaultValue) return false;
			return true;
		}

		/// <summary>
		/// Query the database, and return JObjects for each record returned
		/// </summary>
		public IEnumerable<JObject> Query(string query) {
			using (MySqlCommand cmd = command(query)) {
				using (MySqlDataReader r = executeReader(cmd, query)) {
					JObject row;
					while ((row = readRow(r, query)) != null) {
						yield return row;
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
			return "'" + o.ToString().Replace("'", "''").Replace("\\", "\\\\").Replace("\0", "\\\0") + "'";
		}

		/// <summary>
		/// Rollback transaction
		/// </summary>
		public void Rollback() {
			if (_tran != null) {
				_tran.Rollback();
				_tran.Dispose();
				_tran = null;
			}
		}

		/// <summary>
		/// Get a Dictionary of existing tables in the database
		/// </summary>
		public Dictionary<string, Table> Tables() {
			// NB: By default. MySql table names are case insensitive
			Dictionary<string, Table> tables = new Dictionary<string, Table>(StringComparer.OrdinalIgnoreCase);
			string schema = Regex.Match(_connectionString, "database=(.*?);").Groups[1].Value;
			DataTable tabs = _conn.GetSchema("Tables");
			DataTable cols = _conn.GetSchema("Columns");
			DataTable fkeyCols = _conn.GetSchema("Foreign Key Columns");
			DataTable indexes = _conn.GetSchema("Indexes");
			DataTable indexCols = _conn.GetSchema("IndexColumns");
			DataTable views = _conn.GetSchema("Views");
			DataTable viewCols = _conn.GetSchema("ViewColumns");
			foreach(DataRow table in tabs.Rows) {
				string name = table["TABLE_NAME"].ToString();
				string filter = "TABLE_NAME = " + Quote(name);
				Field[] fields = cols.Select(filter, "ORDINAL_POSITION")
					.Select(delegate(DataRow c) {
						Type t = typeFor(c["DATA_TYPE"].ToString());
						return new Field(c["COLUMN_NAME"].ToString(), t,
							lengthFromColumn(c), c["IS_NULLABLE"].ToString() == "YES", c["EXTRA"].ToString().Contains("auto_increment"),
							colDefault(c));
						}).ToArray();
				List<Index> tableIndexes = new List<Index>();
				foreach (DataRow ind in indexes.Select(filter + " AND PRIMARY = 'True'")) {
					string indexName = ind["INDEX_NAME"].ToString();
					tableIndexes.Add(new Index("PRIMARY", true, 
						indexCols.Select(filter + " AND INDEX_NAME = " + Quote(indexName), "ORDINAL_POSITION")
						.Select(r => fields.First(f => f.Name == r["COLUMN_NAME"].ToString())).ToArray()));
				}
				foreach (DataRow ind in indexes.Select(filter + " AND PRIMARY = 'False'")) {
					string indexName = ind["INDEX_NAME"].ToString();
					if (!indexName.StartsWith("fk_"))
						tableIndexes.Add(new Index(indexName, ind["UNIQUE"].ToString() == "True",
						indexCols.Select(filter + " AND INDEX_NAME = " + Quote(indexName), "ORDINAL_POSITION")
						.Select(r => fields.First(f => f.Name == r["COLUMN_NAME"].ToString())).ToArray()));
				}
				tables[name] = new Table(name, fields, tableIndexes.ToArray());
			}
			foreach (DataRow fk in fkeyCols.Rows) {
				// MySql 5 incorrectly returns lower case table and field names here
				Table detail = tables[fk["TABLE_NAME"].ToString()];
				Table master = tables[fk["REFERENCED_TABLE_NAME"].ToString()];
				Field masterField = fieldFor(master, fk["REFERENCED_COLUMN_NAME"].ToString());
				fieldFor(detail, fk["COLUMN_NAME"].ToString()).ForeignKey = new ForeignKey(master, masterField);
			}
			foreach (DataRow table in views.Select("TABLE_SCHEMA = " + Quote(schema))) {
				string name = table["TABLE_NAME"].ToString();
				string filter = "VIEW_NAME = " + Quote(name);
				Field[] fields = viewCols.Select(filter, "ORDINAL_POSITION")
					.Select(c => new Field(c["COLUMN_NAME"].ToString(), typeFor(c["DATA_TYPE"].ToString()), 
						lengthFromColumn(c), c["IS_NULLABLE"].ToString() == "YES", false,
						colDefault(c))).ToArray();
				Table updateTable = null;
				tables.TryGetValue(Regex.Replace(name, "^.*_", ""), out updateTable);
				tables[name] = new View(name, fields, fields.Length > 0 ? new Index[] { new Index("PRIMARY", true, fields[0]) } : new Index[0], 
					table["VIEW_DEFINITION"].ToString(), updateTable);
			}
			return tables;
		}

		string colDefault(DataRow c) {
			string def = c["COLUMN_DEFAULT"] == System.DBNull.Value ? null : c["COLUMN_DEFAULT"].ToString();
			if (def == "NULL")
				def = null;       // Mariadb puts "NULL" in default value for nullable strings
			else if (def != null && def.StartsWith("'") && def.EndsWith("'"))
				def = def.Substring(1, def.Length - 2);
			return def;
		}

		Field fieldFor(Table table, string name) {
			return table.Fields.FirstOrDefault(f => StringComparer.OrdinalIgnoreCase.Compare(f.Name, name) == 0);
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
			foreach (Index i in dropIndex)
				DropIndex(database, i);
			foreach (string s in dropFK.Where(f => code.Indexes.FirstOrDefault(i => i.Fields[0] == f) == null).Select(f => string.Format("DROP INDEX `fk_{0}_{1}_{2}_idx`",
				code.Name, f.ForeignKey.Table.Name, f.Name))) {
					executeLogSafe(string.Format("ALTER TABLE `{0}` {1}", code.Name, s));
			}
			foreach (string s in insertFK.Where(f => code.Indexes.FirstOrDefault(i => i.Fields[0] == f) == null).Select(f => string.Format("DROP INDEX `fk_{0}_{1}_{2}_idx`",
				code.Name, f.ForeignKey.Table.Name, f.Name))) {
				executeLogSafe(string.Format("ALTER TABLE `{0}` {1}", code.Name, s));
			}
			if (insert.Count != 0 || update.Count != 0 || remove.Count != 0
				|| insertFK.Count != 0 || dropFK.Count != 0) {
				List<string> defs = new List<string>(dropFK.Select(f => string.Format("DROP FOREIGN KEY `fk_{0}_{1}_{2}`",
					code.Name, f.ForeignKey.Table.Name, f.Name)));
				defs.AddRange(remove.Select(f => string.Format("DROP COLUMN `{0}`", f.Name)));
				defs.AddRange(insert.Select(f => "ADD COLUMN " + fieldDef(f)));
				defs.AddRange(update.Select(f => string.Format("CHANGE COLUMN `{0}` {1}", f.Name, fieldDef(f))));
				defs.AddRange(insertFK.Select(f => string.Format(@"ADD CONSTRAINT `fk_{0}_{1}_{2}`
		FOREIGN KEY (`{2}`)
		REFERENCES `{1}` (`{3}`)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION", code.Name, f.ForeignKey.Table.Name, f.Name, f.ForeignKey.Table.PrimaryKey.Name)));
				executeLog(string.Format("ALTER TABLE `{0}` {1}", code.Name, string.Join(",\r\n", defs.ToArray())));
			}
			foreach (Index i in insertIndex)
				CreateIndex(code, i);
			foreach (string s in insertFK.Where(f => code.Indexes.FirstOrDefault(i => i.Fields[0] == f) == null).Select(f => string.Format("ADD INDEX `fk_{0}_{1}_{2}_idx` (`{2}` ASC)",
				code.Name, f.ForeignKey.Table.Name, f.Name))) {
				executeLog(string.Format("ALTER TABLE `{0}` {1}", code.Name, s));
			}
		}

		/// <summary>
		/// Determine whether two views are the same
		/// </summary>
		public bool? ViewsMatch(View code, View database) {
			return null;
		}

		MySqlCommand command(string sql) {
			try {
				return new MySqlCommand(sql, _conn, _tran);
			} catch (Exception ex) {
				throw new DatabaseException(ex, sql);
			}
		}

		int executeLog(string sql) {
			Log.Startup.WriteLine(sql);
			using (MySqlCommand cmd = command(sql)) {
				return cmd.ExecuteNonQuery();
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

		MySqlDataReader executeReader(MySqlCommand cmd, string sql) {
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
			bool nullable = f.Nullable;
			switch (f.Type.Name) {
				case "Int64":
					b.Append("BIGINT");
					break;
				case "Int32":
					b.Append("INT");
					break;
				case "Decimal":
					b.AppendFormat("DECIMAL({0})", f.Length.ToString("0.0").Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, ","));
					break;
				case "Double":
					b.Append("DOUBLE");
					break;
				case "Boolean":
					b.Append("TINYINT(1)");
					break;
				case "DateTime":
					b.Append("DATETIME");
					break;
				case "String":
					if (f.Length == 0) {
						b.Append("LONGTEXT");
						defaultValue = null;        // Default Value not allowed on LONGTEXT
						nullable = true;			// So make it nullable
					} else
						b.AppendFormat("VARCHAR({0})", f.Length);
					break;
				default:
                    if (f.Type.IsEnum) {
                        b.Append("INT");
                        break;
                    }
                    throw new CheckException("Unknown type {0}", f.Type.Name);
			}
			b.AppendFormat(" {0}NULL", nullable ? "" : "NOT ");
			if (f.AutoIncrement)
				b.Append(" AUTO_INCREMENT");
			else if(defaultValue != null)
				b.AppendFormat(" DEFAULT {0}", Quote(defaultValue));
			return b.ToString();
		}

		decimal lengthFromColumn(DataRow c) {
			string type = c["COLUMN_TYPE"].ToString();
			if (type == "double") return 10.4M;
			if (type == "bigint") return 20;
			if (type == "int") return 11;
			Match m = Regex.Match(type, @"[\d,]+");
			return m.Success ? decimal.Parse(m.Value.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)) : 0;
		}

		JObject readRow(MySqlDataReader r, string sql) {
			try {
				if (!r.Read()) return null;
				JObject row = new JObject();
				for (int i = 0; i < r.FieldCount; i++) {
					row.Add(r.GetName(i), r[i].ToJToken());
				}
				return row;
			} catch (Exception ex) {
				throw new DatabaseException(ex, sql);
			}
		}

		static Type typeFor(string s) {
			switch (s.ToLower()) {
				case "bigint":
					return typeof(long);
				case "int":
					return typeof(int);
				case "tinyint":
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
}
