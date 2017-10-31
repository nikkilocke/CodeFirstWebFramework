using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	/// <summary>
	/// DbInterface for Sql Server
	/// </summary>
	public class SqlServerDatabase : DbInterface {
		string _connectionString;
		SqlConnection _conn;
		SqlTransaction _tran;
		Database _db;

		/// <summary>
		/// Constructor
		/// </summary>
		public SqlServerDatabase(Database db, string connectionString) {
			_db = db;
			_connectionString = connectionString;
			_conn = new SqlConnection();
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
		/// Clean up database (does nothing here)
		/// </summary>
		public void CleanDatabase() {
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
				executeLog(string.Format("CREATE VIEW \"{0}\" AS {1}", v.Name, v.Sql));
				return;
			}
			List<string> defs = new List<string>(t.Fields.Select(f => fieldDef(f)));
			if(t.Indexes.Length > 0 && (t.Indexes[0].Fields.Length > 1 || !t.Indexes[0].Fields[0].AutoIncrement)) {
				defs.Add(string.Format("PRIMARY KEY ({0})", string.Join(",", t.Indexes[0].Fields.Select(f => "\"" + f.Name + "\"").ToArray())));
			}
			defs.AddRange(t.Fields.Where(f => f.ForeignKey != null).Select(f => string.Format(@"CONSTRAINT ""FK_{0}_{1}_{2}""
    FOREIGN KEY (""{2}"")
    REFERENCES ""{1}"" (""{3}"")
    ON DELETE NO ACTION
    ON UPDATE NO ACTION", t.Name, f.ForeignKey.Table.Name, f.Name, f.ForeignKey.Table.PrimaryKey.Name)));
			executeLog(string.Format("CREATE TABLE \"{0}\" ({1})", t.Name, string.Join(",\r\n", defs.ToArray())));
					// SQL Server will auto create primary key
			for (int i = 1; i < t.Indexes.Length; i++) {
				Index index = t.Indexes[i];
				executeLog(string.Format("CREATE UNIQUE INDEX \"{0}\" ON \"{1}\" ({2})", index.Name, t.Name, 
					string.Join(",", index.Fields.Select(f => "\"" + f.Name + "\" ASC").ToArray())));
			}
			foreach (Field f in t.Fields.Where(f => f.ForeignKey != null && t.Indexes.FirstOrDefault(i => i.Fields[0] == f) == null)) {
				executeLog(string.Format(@"CREATE INDEX ""FK_{0}_{1}_{2}_idx"" ON ""{0}"" (""{2}"" ASC)",
					t.Name, f.ForeignKey.Table.Name, f.Name));
			}
		}

		/// <summary>
		/// Create an index from a table and index definition
		/// </summary>
		public void CreateIndex(Table t, Index index) {
			executeLog(string.Format("ALTER TABLE \"{0}\" ADD UNIQUE INDEX \"{1}\" ({2})", t.Name, index.Name,
				string.Join(",", index.Fields.Select(f => "\"" + f.Name + "\" ASC").ToArray())));
		}

		/// <summary>
		/// Drop a table
		/// </summary>
		public void DropTable(Table t) {
			executeLogSafe("DROP TABLE " + t.Name);
			executeLogSafe("DROP VIEW " + t.Name);
		}

		/// <summary>
		/// Drop an index
		/// </summary>
		public void DropIndex(Table t, Index index) {
			executeLogSafe(string.Format("ALTER TABLE \"{0}\" DROP INDEX \"{1}\"", t.Name, index.Name));
		}

		/// <summary>
		/// Execute arbitrary sql
		/// </summary>
		public int Execute(string sql) {
			using (SqlCommand cmd = command(sql)) {
				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Determine whether two fields are the same
		/// </summary>
		public bool FieldsMatch(Table t, Field code, Field database) {
			if (code.TypeName != database.TypeName) return false;
			if (code.AutoIncrement != database.AutoIncrement) return false;
			if (code.Length != database.Length) return false;
			if (code.Nullable != database.Nullable) return false;
			if (code.DefaultValue != database.DefaultValue) return false;
			return true;
		}

		/// <summary>
		/// Insert a record
		/// </summary>
		/// <param name="table">Table</param>
		/// <param name="sql">SQL INSERT statement</param>
		/// <param name="updatesAutoIncrement">True if the insert may update an auto-increment field</param>
		/// <returns>The value of the auto-increment record id of the newly inserted record</returns>
		public int Insert(Table table, string sql, bool updatesAutoIncrement) {
			using (SqlCommand cmd = command(sql)) {
				if (updatesAutoIncrement) {
					cmd.CommandText = "SET IDENTITY_INSERT \"" + table.Name + "\" ON";
					cmd.ExecuteNonQuery();
					cmd.CommandText = sql;
				}
				cmd.ExecuteNonQuery();
				if (updatesAutoIncrement) {
					cmd.CommandText = "SET IDENTITY_INSERT \"" + table.Name + "\" OFF";
					cmd.ExecuteNonQuery();
				} else if (table.PrimaryKey.AutoIncrement) {
					cmd.CommandText = "SELECT SCOPE_IDENTITY()";
					return (int)(decimal)cmd.ExecuteScalar();
				}
				return 0;
			}
		}

		/// <summary>
		/// Query the database, and return JObjects for each record returned
		/// </summary>
		public IEnumerable<Newtonsoft.Json.Linq.JObject> Query(string sql) {
			using (SqlCommand cmd = command(sql)) {
				using (SqlDataReader r = executeReader(cmd, sql)) {
					JObject row;
					while ((row = readRow(r, sql)) != null) {
						yield return row;
					}
				}
			}
		}

		/// <summary>
		/// Query the database, and return the first record matching the query
		/// </summary>
		public Newtonsoft.Json.Linq.JObject QueryOne(string query) {
			return Query(Regex.Replace(query, @"^\s*SELECT\b", "SELECT TOP 1", RegexOptions.IgnoreCase)).FirstOrDefault();
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
			Dictionary<string, Table> tables = new Dictionary<string, Table>();
			string schema = Regex.Match(_connectionString, "database=(.*?);").Groups[1].Value;
			DataTable tabs = _conn.GetSchema("Tables");
			DataTable cols = _conn.GetSchema("Columns");
			DataTable fkeyCols = _conn.GetSchema("ForeignKeys");
			DataTable indexes = _conn.GetSchema("Indexes");
			DataTable indexCols = _conn.GetSchema("IndexColumns");
			DataTable views = _conn.GetSchema("Views");
			DataTable viewCols = _conn.GetSchema("ViewColumns");
			foreach (DataRow table in tabs.Rows) {
				string name = table["TABLE_NAME"].ToString();
				string identityColumn = null;
				using (SqlCommand cmd = command("SELECT * FROM " + name + " WHERE 1 = 0")) {
					using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly)) {
						DataTable tblcols = reader.GetSchemaTable();
						foreach (DataRow r in tblcols.Rows) {
							if ((bool)r["IsIdentity"]) {
								identityColumn = r["ColumnName"].ToString();
							}
						}
					}
				}
				string filter = "TABLE_NAME = " + Quote(name);
				Field[] fields = cols.Select(filter, "ORDINAL_POSITION")
					.Select(c => new Field(c["COLUMN_NAME"].ToString(), typeFor(c["DATA_TYPE"].ToString()),
						lengthFromColumn(c), c["IS_NULLABLE"].ToString() == "YES", c["COLUMN_NAME"].ToString() == identityColumn,
						c["COLUMN_DEFAULT"] == System.DBNull.Value ? null : Regex.Replace(c["COLUMN_DEFAULT"].ToString(), @"^\('?(.*?)'?\)", "$1"))).ToArray();
				List<Index> tableIndexes = new List<Index>();
				foreach (DataRow ind in indexes.Select(filter)) {
					string indexName = ind["INDEX_NAME"].ToString();
					if (indexName.StartsWith("PK_" + name + "_"))
						indexName = "PRIMARY";
					else if (!indexName.StartsWith("FK_" + name + "_")) {
						tableIndexes.Add(new Index(indexName,
							indexCols.Select(filter + " AND INDEX_NAME = " + Quote(indexName), "ORDINAL_POSITION")
							.Select(r => fields.First(f => f.Name == r["COLUMN_NAME"].ToString())).ToArray()));
					}
				}
				tables[name] = new Table(name, fields, tableIndexes.ToArray());
			}
			using (SqlCommand cmd = command(@"SELECT 
CTU.TABLE_NAME,
KCU.COLUMN_NAME,
CTU2.TABLE_NAME AS REFERENCED_TABLE_NAME,
KCU2.COLUMN_NAME AS REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE CTU
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU ON KCU.CONSTRAINT_NAME = CTU.CONSTRAINT_NAME AND KCU.CONSTRAINT_SCHEMA = CTU.CONSTRAINT_SCHEMA
JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC ON RC.CONSTRAINT_NAME = CTU.CONSTRAINT_NAME AND RC.CONSTRAINT_SCHEMA = CTU.CONSTRAINT_SCHEMA
JOIN INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE CTU2 ON CTU2.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME AND CTU2.CONSTRAINT_SCHEMA = RC.UNIQUE_CONSTRAINT_SCHEMA
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2 ON KCU2.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME AND KCU2.CONSTRAINT_SCHEMA = RC.UNIQUE_CONSTRAINT_SCHEMA
WHERE CTU.CONSTRAINT_NAME LIKE 'FK_%'")) {
				using (SqlDataReader fk = cmd.ExecuteReader()) {
					while (fk.Read()) {
						Table detail = tables[fk["TABLE_NAME"].ToString()];
						Table master = tables[fk["REFERENCED_TABLE_NAME"].ToString()];
						Field masterField = fieldFor(master, fk["REFERENCED_COLUMN_NAME"].ToString());
						fieldFor(detail, fk["COLUMN_NAME"].ToString()).ForeignKey = new ForeignKey(master, masterField);
					}
				}
			}
			foreach (DataRow table in views.Select("TABLE_SCHEMA = " + Quote(schema))) {
				string name = table["TABLE_NAME"].ToString();
				string filter = "VIEW_NAME = " + Quote(name);
				Field[] fields = viewCols.Select(filter, "ORDINAL_POSITION")
					.Select(c => new Field(c["COLUMN_NAME"].ToString(), typeFor(c["DATA_TYPE"].ToString()),
						lengthFromColumn(c), c["IS_NULLABLE"].ToString() == "YES", false,
						c["COLUMN_DEFAULT"] == System.DBNull.Value ? null : c["COLUMN_DEFAULT"].ToString())).ToArray();
				Table updateTable = null;
				tables.TryGetValue(Regex.Replace(name, "^.*_", ""), out updateTable);
				tables[name] = new View(name, fields, new Index[] { new Index("PRIMARY", fields[0]) },
					table["VIEW_DEFINITION"].ToString(), updateTable);
			}
			return tables;
		}

		Field fieldFor(Table table, string name) {
			return table.Fields.FirstOrDefault(f => f.Name == name);
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
		public void UpgradeTable(Table code, Table database, List<Field> insert, List<Field> update, List<Field> remove, List<Field> insertFK, List<Field> dropFK, List<Index> insertIndex, List<Index> dropIndex) {
			foreach (Index i in dropIndex)
				DropIndex(database, i);
			foreach (string s in dropFK.Where(f => code.Indexes.FirstOrDefault(i => i.Fields[0] == f) == null).Select(f => string.Format("DROP INDEX \"FK_{0}_{1}_{2}_idx\"",
				code.Name, f.ForeignKey.Table.Name, f.Name))) {
					executeLogSafe(string.Format("ALTER TABLE \"{0}\" {1}", code.Name, s));
			}
			foreach (string s in insertFK.Where(f => code.Indexes.FirstOrDefault(i => i.Fields[0] == f) == null).Select(f => string.Format("DROP INDEX \"FK_{0}_{1}_{2}_idx\"",
				code.Name, f.ForeignKey.Table.Name, f.Name))) {
				executeLogSafe(string.Format("ALTER TABLE \"{0}\" {1}", code.Name, s));
			}
			if (insert.Count != 0 || update.Count != 0 || remove.Count != 0
				|| insertFK.Count != 0 || dropFK.Count != 0) {
					List<string> defs = new List<string>(dropFK.Select(f => string.Format("DROP FOREIGN KEY \"FK_{0}_{1}_{2}\"",
					code.Name, f.ForeignKey.Table.Name, f.Name)));
				defs.AddRange(remove.Select(f => string.Format("DROP COLUMN \"{0}\"", f.Name)));
				defs.AddRange(insert.Select(f => "ADD COLUMN " + fieldDef(f)));
				defs.AddRange(update.Select(f => string.Format("ALTER COLUMN \"{0}\" {1}", f.Name, fieldDef(f))));
				defs.AddRange(insertFK.Select(f => string.Format(@"ADD CONSTRAINT ""FK_{0}_{1}_{2}""
		FOREIGN KEY (""{2}"")
		REFERENCES ""{1}"" (""{3}"")
		ON DELETE NO ACTION
		ON UPDATE NO ACTION", code.Name, f.ForeignKey.Table.Name, f.Name, f.ForeignKey.Table.PrimaryKey.Name)));
				executeLog(string.Format("ALTER TABLE \"{0}\" {1}", code.Name, string.Join(",\r\n", defs.ToArray())));
			}
			foreach (Index i in insertIndex)
				CreateIndex(code, i);
			foreach (string s in insertFK.Where(f => code.Indexes.FirstOrDefault(i => i.Fields[0] == f) == null).Select(f => string.Format("ADD INDEX \"FK_{0}_{1}_{2}_idx\" (\"{2}\" ASC)",
				code.Name, f.ForeignKey.Table.Name, f.Name))) {
				executeLog(string.Format("ALTER TABLE \"{0}\" {1}", code.Name, s));
			}
		}

		/// <summary>
		/// Do the views in code and database match
		/// </summary>
		public bool? ViewsMatch(View code, View database) {
			return null;
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
				return "'" + dt.ToString(dt.TimeOfDay == TimeSpan.Zero ? "yyyy-MM-dd" : "yyyy-MM-ddTHH:mm:ss") + "'";
			}
			return "'" + o.ToString().Replace("'", "''") + "'";
		}

		SqlCommand command(string sql) {
			try {
				return new SqlCommand(sql, _conn, _tran);
			} catch (Exception ex) {
				throw new DatabaseException(ex, sql);
			}
		}

		int executeLog(string sql) {
			WebServer.Log(sql);
			using (SqlCommand cmd = command(sql)) {
				return cmd.ExecuteNonQuery();
			}
		}

		int executeLogSafe(string sql) {
			try {
				return executeLog(sql);
			} catch (Exception ex) {
				WebServer.Log(ex.Message);
				return -1;
			}
		}

		SqlDataReader executeReader(SqlCommand cmd, string sql) {
			try {
				return cmd.ExecuteReader();
			} catch (Exception ex) {
				throw new DatabaseException(ex, sql);
			}
		}

		string fieldDef(Field f) {
			StringBuilder b = new StringBuilder();
			b.AppendFormat("\"{0}\" ", f.Name);
			switch (f.Type.Name) {
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
					if (f.Length == 0)
						b.Append("TEXT");
					else
						b.AppendFormat("VARCHAR({0})", f.Length);
					break;
				default:
					throw new CheckException("Unknown type {0}", f.Type.Name);
			}
			b.AppendFormat(" {0}NULL", f.Nullable ? "" : "NOT ");
			if (f.AutoIncrement)
				b.Append(" IDENTITY(1,1) PRIMARY KEY");
			else if (f.DefaultValue != null)
				b.AppendFormat(" DEFAULT {0}", Quote(f.DefaultValue));
			return b.ToString();
		}

		decimal lengthFromColumn(DataRow c) {
			switch (c["DATA_TYPE"].ToString().ToLower()) {
				case "double":
					return 10.4M;
				case "int":
					return Convert.ToDecimal(c["NUMERIC_PRECISION"]) == 1 ? 1 : 11;
				case "decimal":
					return Convert.ToDecimal(c["NUMERIC_PRECISION"]) + Convert.ToDecimal(c["NUMERIC_PRECISION_RADIX"]) / 10;
				case "varchar":
					return Convert.ToDecimal(c["CHARACTER_MAXIMUM_LENGTH"]);
			}
			return 0;
		}

		JObject readRow(SqlDataReader r, string sql) {
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
