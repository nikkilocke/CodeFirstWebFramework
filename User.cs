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
		[Primary]
		public int? idUser;
		[Unique("Login")]
		public string Login;
		[Unique("Email")]
		public string Email;
		[Field(Type = "passwordInput")]
		public string Password;
		public int AccessLevel;
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
		public virtual string PasswordValid() {
			if (string.IsNullOrWhiteSpace(Password))
				return "Password may not be empty";
			if (Password.Length < 6)
				return "Password must be at least 6 characters";
			return null;
		}
	}

	/// <summary>
	/// Module level permissions for a user
	/// </summary>
	[Table]
	public class Permission : JsonObject {
		[Field(Visible = false)]
		[ForeignKey("User")]
		[Primary(1, AutoIncrement = false)]
		public int? UserId;
		[Primary(2, AutoIncrement = false)]
		[ReadOnly]
		public string Module;
		[Primary(3, AutoIncrement = false)]
		[ReadOnly]
		public string Method;
		public int AccessLevel;
		[DoNotStore]
		public int MinAccessLevel;
	}

	/// <summary>
	/// Attribute to use on AppModule or method to limit access
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
	public class AuthAttribute : Attribute {
		public AuthAttribute() : this(0) {
		}

		public AuthAttribute(int AccessLevel) {
			this.AccessLevel = AccessLevel;
		}

		public int AccessLevel;
	}

	/// <summary>
	/// Predefined access levels.
	/// Derive a class from this and override Select to provide additional levels.
	/// </summary>
	public class AccessLevel {
		public const int Any = -1;			// Allow access to anyone
		public const int Unspecified = 0;	// No level specified - you will check the level in code, presumably
		public const int ReadOnly = 1;
		public const int ReadWrite = 2;
		public const int Admin = 99;		// Administrator - do not change this value - should allow access to anything
		public const int None = 0;			// No access (a User.AccessLevel)

		/// <summary>
		/// Return the options for a selectInput field to select AccessLevel.
		/// NB Must always have 0, "None" as the first item.
		/// </summary>
		public virtual IEnumerable<JObject> Select() {
			yield return new JObject().AddRange("id", 0, "value", "None");
			yield return new JObject().AddRange("id", ReadOnly, "value", "Read Only");
			yield return new JObject().AddRange("id", ReadWrite, "value", "Read Write");
			yield return new JObject().AddRange("id", Admin, "value", "Admin");
		}
	}

}
