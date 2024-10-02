using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	/// <summary>
	/// Interface through which all interaction between Database and a database-specific implementation happens.
	/// </summary>
	interface DbInterface : IDisposable {
		void BeginTransaction();

		/// <summary>
		/// Return SQL to cast a value to a type
		/// </summary>
		string Cast(string value, string type);

		/// <summary>
		/// Clean up the database
		/// </summary>
		void CleanDatabase();

		/// <summary>
		/// Commit current transaction
		/// </summary>
		void Commit();

		void CreateTable(Table t);

		void CreateIndex(Table t, Index index);

		void DropTable(Table t);

		void DropIndex(Table t, Index index);

		/// <summary>
		/// Execute sql, returning id of any record inserted
		/// </summary>
		int Execute(string sql);

		/// <summary>
		/// Return SQL to do a GROUP_CONCAT
		/// </summary>
		string GroupConcat(string expression, string separator = null);

		/// <summary>
		/// Do the table names in code and database match (some implementations are case insensitive)
		/// </summary>
		bool TableNamesMatch(Table code, Table database);

		/// <summary>
		/// Do the fields in code and database match (some implementations are case insensitive)
		/// </summary>
		bool FieldsMatch(Table t, Field code, Field database);

		/// <summary>
		/// Insert data in table
		/// <param name="table">Table</param>
		/// <param name="sql">SQL INSERT statement</param>
		/// <param name="updatesAutoIncrement">True if the SQL statement updates the AutoIncrement field</param>
		/// <returns>Id of last row inserted</returns>
		/// </summary>
		int Insert(Table table, string sql, bool updatesAutoIncrement);

		IEnumerable<JObject> Query(string sql);

		JObject QueryOne(string query);

		/// <summary>
		/// Quote any kind of data for inclusion in a SQL query
		/// </summary>
		string Quote(object o);

		/// <summary>
		/// Quote a field or table name when creating SQL
		/// </summary>
		string QuoteName(string name);

		/// <summary>
		/// Rollback transaction
		/// </summary>
		void Rollback();

		/// <summary>
		/// Dictionary of Tables by name
		/// </summary>
		Dictionary<string, Table> Tables();

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
		void UpgradeTable(Table code, Table database, List<Field> insert, List<Field> update, List<Field> remove,
			List<Field> insertFK, List<Field> dropFK, List<Index> insertIndex, List<Index> dropIndex);

		/// <summary>
		/// Do the views in code and database match
		/// </summary>
		bool? ViewsMatch(View code, View database);

	}
}
