using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation.PBKDF2;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Tls;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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
		[Length(90)]
		[Field(Type = "passwordInput")]
		public string Password;
		/// <summary>
		/// Date and time the password was last changed, or 1/1/2000 if not changed after this field was created
		/// </summary>
		[DefaultValue("2000-01-01")]
		[Field(Type = "datetime")]
		public DateTime PasswordLastChanged;
		/// <summary>
		/// Random salt to add to password before hashing. 
		/// First character is $ in this version (may change if new hashing algorithm used)
		/// </summary>
		[Length(90)]
		[Field(Visible = false)]
		public string Salt;
		/// <summary>
		/// Determines if the password is stored using an obsolete hashing method, and needs to be updated
		/// </summary>
		[DoNotStore]
		public bool OldHashingMethod => Salt == null || !Salt.StartsWith("$");
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
		/// <param name="password">The password to hash into Password</param>
		/// <param name="forceLatestAlgorithm">True to always use the latest hashing method</param>
		public virtual string HashPassword(string password, bool forceLatestAlgorithm = false) {
			if (idUser == null)
				forceLatestAlgorithm = true;
			if(forceLatestAlgorithm && OldHashingMethod) {
				// New user or changing algorithm- create salt
				byte[] salt = new byte[64];
				RandomNumberGenerator.Create().GetBytes(salt);
				Salt = "$" + Convert.ToBase64String(salt);
			}
			if (string.IsNullOrEmpty(Salt))
				return LegacyHashPassword(password);
			var hash = KeyDerivation.Pbkdf2(
				password,
				GetSalt(),
				KeyDerivationPrf.HMACSHA256,
				10000,
				64);
			return Convert.ToBase64String(hash);
		}

		/// <summary>
		/// Update the stored Hashed password (using the latest hashing algortihm
		/// Also update the password age.
		/// It will be necessary to save the record to the database aftereards
		/// </summary>
		public virtual void UpdatePassword(string password) {
			Password = HashPassword(password, true);
			PasswordLastChanged = DateTime.Now;
		}

		/// <summary>
		/// Return the salt byte array
		/// </summary>
		protected byte[] GetSalt() {
			return Convert.FromBase64String(Salt.Substring(1));
		}

		/// <summary>
		/// Hash a password using SHA and Base64
		/// </summary>
		public string LegacyHashPassword(string password) {
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

		[DoNotStore]
		int[] _accessLevels;

		/// <summary>
		/// Get max access level for groups list
		/// </summary>
		public int GetAccessLevel(AppModule module, int[] groups) {
			if (idUser > 0 && ModulePermissions && _accessLevels == null)
				ReloadAccessLevels(module);
			if (_accessLevels == null || groups.Length == 0)
				return AccessLevel;
			int level = groups.Max(g => _accessLevels[g]);
			// Level AccessLevel.Any means use user access level
			return level == CodeFirstWebFramework.AccessLevel.Any ? AccessLevel : level;
		}

		/// <summary>
		/// List of AccessLevels, 1 per AuthGroup, or null if ModulePermissions is not on
		/// </summary>
		public int[] AccessLevels(AppModule module) {
			if (idUser > 0 && ModulePermissions && _accessLevels == null)
				ReloadAccessLevels(module);
			return _accessLevels;
		}

		/// <summary>
		/// Reload AccessLevels from ModulePermissions
		/// </summary>
		public virtual void ReloadAccessLevels(AppModule module) {
			if (idUser > 0 && ModulePermissions) {
				Namespace space = module.Server.NamespaceDef;
				_accessLevels = Enumerable.Repeat(CodeFirstWebFramework.AccessLevel.Any, space.AuthGroups.Count).ToArray();
				foreach (Permission p in module.Database.Query<Permission>("SELECT * FROM Permission WHERE UserId = " + idUser)) {
					string key = space.OldAuth ? (p.Module + ":" + p.Method) : p.Method;
					if (space.AuthGroups.TryGetValue(key, out int index))
						_accessLevels[index] = p.FunctionAccessLevel;
				}
			} else
				_accessLevels = null;
		}

		/// <summary>
		/// Return the maximum access level for this user in any of the AuthGroups provided
		/// </summary>
		public int GroupAccessLevel(Database db, params string[] groups) {
			if (idUser > 0 && ModulePermissions) {
				JObject level = db.Query($"SELECT Max(FunctionAccessLevel) AS Level FROM Permission WHERE UserId = {idUser} AND Method {db.In((IEnumerable<string>)groups)}")
						.FirstOrDefault();
				if(level != null)
					return Math.Max(level.AsInt("Level"), AccessLevel);
			}
			return AccessLevel;
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
				return Method == "-" ? "Other Methods" : Method.UnCamel();
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

		/// <summary>
		/// Auth Groups 
		/// </summary>
		public int [] Groups;
	}

	/// <summary>
	/// Attribute to use on AppModule or method to allow special access to methods in the group for a user
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class AuthGroupAttribute : Attribute {
		/// <summary>
		/// Constructor with name.
		/// </summary>
		/// <param name="name">Name of group</param>
		public AuthGroupAttribute(string name) {
			Name = name;
		}

		/// <summary>
		/// Name to use. AuthAttributes with the same name are grouped in the UI.
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
		/// Add the possible values of AccessLevel to the dictionary, base classes first,
		/// so the derived values override the base ones
		/// </summary>
		/// <param name="values">The dictionary</param>
		/// <param name="t">The type to process</param>
		/// <param name="maxLevel">Ignore values above maxLevel</param>
		void addValues(Dictionary<int, JObject> values, Type t, int maxLevel = int.MaxValue) {
			if (t.BaseType.Name == "AccessLevel")
				addValues(values, t.BaseType, maxLevel);
            foreach (FieldInfo c in t
					.GetFieldsInOrder(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
					.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(int))) {
				int id = (int)c.GetRawConstantValue();
				if (id > 0 && id <= maxLevel)
					values[id] = new JObject().AddRange("id", id, "value", c.Name.UnCamel());
			}
		}

		/// <summary>
		/// Return the options for a selectInput field to select AccessLevel.
		/// NB Must always have 0, "None" as the first item.
		/// </summary>
		public virtual IEnumerable<JObject> Select(int maxLevel = int.MaxValue) {
			Dictionary<int, JObject> values = new Dictionary<int, JObject>();
			values[0] = new JObject().AddRange("id", 0, "value", "None");
			addValues(values, GetType(), maxLevel);
			return values.Values.OrderBy(v => v.AsInt("id"));
		}
	}

}
