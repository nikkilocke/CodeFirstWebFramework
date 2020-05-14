using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	/// <summary>
	/// Login user with permissions
	/// </summary>
	[Table]
    public class User : JsonObject {
		/// <summary>
		/// Unique id.
		/// </summary>
		[Primary]
		public int? idUser;
		/// <summary>
		/// Login name.
		/// </summary>
		[Unique("Login")]
		public string Login;
		/// <summary>
		/// Email -c an be used instead of login name to login.
		/// </summary>
		[Unique("Email")]
		public string Email;
		/// <summary>
		/// Password.
		/// </summary>
		[Field(Type = "passwordInput")]
		public string Password;
		/// <summary>
		/// See AccessLevel class for possible values
		/// </summary>
		public int AccessLevel;
		/// <summary>
		/// True if user has different permissions for different modules/methods.
		/// </summary>
		public bool ModulePermissions;

		/// <summary>
		/// Hash a password using SHA and Base64
		/// </summary>
		public virtual string HashPassword(string password) {
			Encoding enc = Encoding.GetEncoding(1252);
			return Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(enc.GetBytes(password)));
		}

		/// <summary>
		/// Check supplied password is valid
		/// </summary>
		/// <returns>null if valid, or error message explaining why if not</returns>
		public virtual string PasswordValid(string password) {
			if (string.IsNullOrWhiteSpace(password))
				return "Password may not be empty";
			if (password.Length < 6)
				return "Password must be at least 6 characters";
			return null;
		}
	}

	/// <summary>
	/// Module level permissions for a user
	/// </summary>
	[Table]
	public class Permission : JsonObject {
		/// <summary>
		/// User to whom this Permission applies.
		/// </summary>
		[Field(Visible = false)]
		[ForeignKey("User")]
		[Primary(1, AutoIncrement = false)]
		public int? UserId;
		/// <summary>
		/// Module name, or Name from module level AuthAttribute if there is one specified.
		/// Multiple module-level AuthAttributes with the same name have the same Permission record.
		/// </summary>
		[Primary(2, AutoIncrement = false)]
		[ReadOnly]
		public string Module;
		/// <summary>
		/// Method name, or Name from method level AuthAttribute if there is one specified.
		/// "-" for a module-level Permission.
		/// Multiple method-level AuthAttributes with the same name have the same Permission record.
		/// </summary>
		[Primary(3, AutoIncrement = false)]
		[ReadOnly]
		public string Method;
		/// <summary>
		/// Show method name in human-readable format.
		/// </summary>
		[DoNotStore]
		public string Function {
			get {
				return Method == "-" ? "All" : Method.UnCamel();
			}
		}
		/// <summary>
		/// The AccessLevel granted to this user for this module/method.
		/// </summary>
		public int FunctionAccessLevel;
		/// <summary>
		/// Min access level needed for a user to be able to access this module/method.
		/// For example. for a module, the lowest access level of all the methods (not counting
		/// AccessLevel.Any), or the module access level, whichever is the less.
		/// Used in the UI to remove irrelevant access levels from the drop-down list.
		/// </summary>
		[DoNotStore]
		public int MinAccessLevel;
	}

	/// <summary>
	/// Attribute to use on AppModule or method to limit access
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
	public class AuthAttribute : Attribute {
		/// <summary>
		/// Constructor - level will be set to Any.
		/// </summary>
		public AuthAttribute() : this(0) {
		}

		/// <summary>
		/// Constructor with specific access level.
		/// </summary>
		/// <param name="AccessLevel"></param>
		public AuthAttribute(int AccessLevel) {
			this.AccessLevel = AccessLevel;
		}

		/// <summary>
		/// The AccessLevel required to see this module or method.
		/// </summary>
		public int AccessLevel;

		/// <summary>
		/// True if this AuthAttribute is not to appear on the list of module permissions
		/// </summary>
		public bool Hide;

		/// <summary>
		/// Name to use (instead of module/method). AuthAttributes with the same name are grouped in the UI.
		/// </summary>
		public string Name;
	}

	/// <summary>
	/// Predefined access levels.
	/// Derive a class from this to provide additional levels.
	/// </summary>
	public class AccessLevel {
		/// <summary>
		/// Allow access to anyone
		/// </summary>
		public const int Any = -1;
		/// <summary>
		/// No level specified - you will check the level in code, presumably
		/// </summary>
		public const int Unspecified = 0;
		/// <summary>
		/// Read only
		/// </summary>
		public const int ReadOnly = 10;
		/// <summary>
		/// Read Write
		/// </summary>
		public const int ReadWrite = 20;
		/// <summary>
		/// Administrator - do not change this value - should allow access to anything
		/// </summary>
		public const int Admin = 1000;
		/// <summary>
		/// // No access (a User.AccessLevel)
		/// </summary>
		public const int None = 0;

		/// <summary>
		/// Return the options for a selectInput field to select AccessLevel.
		/// NB Must always have 0, "None" as the first item.
		/// </summary>
		public virtual IEnumerable<JObject> Select(int maxLevel = int.MaxValue) {
			List<JObject> values = new List<JObject>();
			values.Add(new JObject().AddRange("id", 0, "value", "None"));
			foreach (FieldInfo c in GetType()
					.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
					.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(int))) {
				int id = (int)c.GetRawConstantValue();
				if(id > 0 && id <= maxLevel && !values.Any(v => v.AsInt("id") == id))
					values.Add(new JObject().AddRange("id", id, "value", c.Name.UnCamel()));
			}
			return values.OrderBy(v => v.AsInt("id"));
		}
	}

}
