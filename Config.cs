using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using Mustache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace CodeFirstWebFramework {
	/// <summary>
	/// The config file from the data folder
	/// </summary>
	public class Config {
		static DateTime startup = DateTime.Now;
		[JsonIgnore]
		ServerConfig _default;
		/// <summary>
		/// The name of the program
		/// </summary>
		public static string EntryModule = Assembly.GetEntryAssembly().Name();
		/// <summary>
		/// The data folder
		/// </summary>
		public static string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), EntryModule);
		/// <summary>
		/// The namespace of the entry program
		/// </summary>
		public static string EntryNamespace = "CodeFirstWebFramework";
		/// <summary>
		/// The default namespace to use if none supplied
		/// </summary>
		public static string DefaultNamespace = "CodeFirstWebFramework";
		/// <summary>
		/// The default type of database
		/// </summary>
		public string Database = "SQLite";
		/// <summary>
		/// The default namespace
		/// </summary>
		public string Namespace = EntryNamespace;
		/// <summary>
		/// The default connection string
		/// </summary>
		public string ConnectionString = "Data Source=" + DataPath + "/" + EntryModule + ".db";
		/// <summary>
		/// The name of the file from which this config has been read
		/// </summary>
		[JsonIgnore]
		public string Filename;
		/// <summary>
		/// The default port the web server listens on
		/// </summary>
		public int Port = 8080;
		/// <summary>
		/// Log all queries that take longer than this
		/// </summary>
		public int SlowQuery = 100;
		/// <summary>
		/// The default server name
		/// </summary>
		public string ServerName = "localhost";
		/// <summary>
		/// The default email address to send from
		/// </summary>
		public string Email = "root@localhost";
		/// <summary>
		/// Expire sessions after this number of minutes
		/// </summary>
		public int SessionExpiryMinutes = 30;
		/// <summary>
		/// List of other servers listening
		/// </summary>
		public List<ServerConfig> Servers = new List<ServerConfig>();
		/// <summary>
		/// Logging configuration
		/// </summary>
		public Log.Config Logging = new Log.Config();
		/// <summary>
		/// Cookie timeout in minutes
		/// </summary>
		public int CookieTimeoutMinutes = 60;
		/// <summary>
		/// Cookies are stored in a database, rather than in memory
		/// </summary>
		public bool PersistentSessions;
		/// <summary>
		/// Set to true to generate a Nonce for each request.
		/// If set to true, the AppModule will have a different string in Nonce for every request,
		/// and the default template will use CSP to enforce nonces on script-src and style-src.
		/// If not set, or set to false, the Nonce string will be null.
		/// </summary>
		public bool Nonce;
		/// <summary>
		/// Command line flags extracted from program command line
		/// </summary>
		static public NameValueCollection CommandLineFlags;

		/// <summary>
		/// The default (and only) Config file
		/// </summary>
		public static Config Default;

		static Config() {
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				DataPath = Path.Combine("/Users/Shared/.config", EntryModule);
			Default = new Config();
		}

		/// <summary>
		/// Save this configuration by name
		/// </summary>
		/// <param name="filename">Simple name - no folders allowed - it will be saved in the data folder</param>
		public void Save(string filename) {
			filename = fileFor(filename);
			using (StreamWriter w = new StreamWriter(filename))
				w.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented));
		}

		/// <summary>
		/// A ServerConfig with the defaults from the main Config section
		/// </summary>
		[JsonIgnore]
		public ServerConfig DefaultServer {
			get {
				lock (this) {
					if (_default == null)
						_default = new ServerConfig() {
							ServerName = ServerName,
							Email = Email,
							Namespace = Namespace,
							CookieTimeoutMinutes = CookieTimeoutMinutes,
							PersistentSessions = PersistentSessions
						};
				}
				return _default;
			}
		}

		/// <summary>
		/// The ServerConfig which applies to the provided url host part
		/// </summary>
		public ServerConfig SettingsForHost(Uri uri) {
			if (Servers != null) {
				ServerConfig settings = Servers.FirstOrDefault(s => s.Matches(uri));
				if (settings != null)
					return settings;
			}
			return uri.Port == Port ? DefaultServer : null;

		}

		static string fileFor(string filename) {
			if (!filename.Contains("/") && !filename.Contains("\\")) {
				Directory.CreateDirectory(DataPath);
				filename = Path.Combine(DataPath, filename);
			}
			return filename;
		}

		/// <summary>
		/// Load a config by name
		/// </summary>
		/// <param name="filename">Plain filename - no folders allowed - will be in the data folder</param>
		static public void Load(string filename) {
			filename = fileFor(filename);
			Log.Startup.WriteLine("Loading config from {0}", filename);
			using (StreamReader s = new StreamReader(filename)) {
				Default = Utils.JsonTo<Config>(s.ReadToEnd());
				Default.Filename = Path.GetFileNameWithoutExtension(filename);
				Default._default = null;
			}
		}

		/// <summary>
		/// Read any config file specified in the command line, or ProgramName.config if none.
		/// Also fill in the CommandLineFlags.
		/// </summary>
		/// <param name="args">The program arguments</param>
		static public void Load(string[] args) {
			try {
				Config.Default.Namespace = Config.EntryNamespace = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().ReflectedType.Namespace;
				string configName = EntryModule + ".config";
				if (File.Exists(fileFor(configName)))
					Config.Load(configName);
				else
					Config.Default.Save(configName);
				NameValueCollection flags = new NameValueCollection();
				foreach (string arg in args) {
					string value = arg;
					string name = Utils.NextToken(ref value, "=");
					flags[name] = value;
					if (value == "") {
						switch (Path.GetExtension(arg).ToLower()) {
							case ".config":
								configName = arg;
								Config.Load(arg);
								continue;
						}
					}
				}
				Config.CommandLineFlags = flags;
				if(Config.Default.Logging != null)
					Config.Default.Logging.Update();	// Set up logging
				if (!string.IsNullOrWhiteSpace(flags["culture"])) {
					CultureInfo c = new CultureInfo(flags["culture"]);
					Thread.CurrentThread.CurrentCulture = c;
					CultureInfo.DefaultThreadCurrentCulture = c;
					CultureInfo.DefaultThreadCurrentUICulture = c;
				}
				if (!string.IsNullOrWhiteSpace(flags["tz"]))
					Utils._tz = TimeZoneInfo.FindSystemTimeZoneById(flags["tz"]);
#if DEBUG
				if (flags["now"] != null) {
					DateTime now = Utils.Now;
					DateTime newDate = DateTime.Parse(flags["now"]);
					if (newDate.Date == newDate)
						newDate = newDate.Add(now - now.Date);
					Utils._timeOffset = newDate - now;
				}
#endif
				Utils.Check(!string.IsNullOrEmpty(Config.Default.ConnectionString), "You must specify a ConnectionString in the " + configName + " file");
			} catch (Exception ex) {
				Log.Error.WriteLine(ex.ToString());
			}
		}
	}

	/// <summary>
	/// Configuration for a web server
	/// </summary>
	public class ServerConfig {
		/// <summary>
		/// Name part of the url
		/// </summary>
		public string ServerName;
		/// <summary>
		/// Other names allowed, separated by spaces
		/// </summary>
		public string ServerAlias;
		/// <summary>
		/// The port the web server listens on
		/// </summary>
		public int Port;
		/// <summary>
		/// Namespace in which to look for AppModules
		/// </summary>
		public string Namespace;
		/// <summary>
		/// Email address from which to send emails
		/// </summary>
		public string Email;
		/// <summary>
		/// Title for web pages
		/// </summary>
		public string Title;
		/// <summary>
		/// Database type
		/// </summary>
		public string Database;
		/// <summary>
		/// Database connection string
		/// </summary>
		public string ConnectionString;
		/// <summary>
		/// Additional Assemblies to load to provide the required functionality
		/// </summary>
		public string [] AdditionalAssemblies = new string [0];
		/// <summary>
		/// Cookie timeout in minutes
		/// </summary>
		public int CookieTimeoutMinutes = 60;
		/// <summary>
		/// Cookies are stored in a database, rather than in memory
		/// </summary>
		public bool PersistentSessions;
		/// <summary>
		/// Set to true to generate a Nonce for each request.
		/// If set to true, the AppModule will have a different string in Nonce for every request,
		/// and the default template will use CSP to enforce nonces on script-src and style-src.
		/// If not set, or set to false, the Nonce string will be null.
		/// </summary>
		public bool ?Nonce;
		/// <summary>
		/// Details of the namespace
		/// </summary>
		[JsonIgnore]
		public Namespace NamespaceDef;
		/// <summary>
		/// Unique database id
		/// </summary>
		[JsonIgnore]
		public int DatabaseId;

		/// <summary>
		/// Whether this server serves for the host name
		/// </summary>
		public bool Matches(Uri uri) {
			int port = Port == 0 ? Config.Default.Port : Port;
			if (uri.Port != port)
				return false;
			string host = uri.Host;
			if (host == ServerName)
				return true;
			host = host.ToLower();
			if (host == ServerName)
				return true;
			if (!string.IsNullOrWhiteSpace(ServerAlias)) {
				string pattern = "^(" + Regex.Escape(Regex.Replace(ServerAlias, " +", " ")).Replace(@"\ ", "|").Replace(@"\*", ".*").Replace(@"\?", ".") + ")$";
				if (Regex.IsMatch(host, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline))
					return true;
			}
			return false;
		}

	}

	/// <summary>
	/// The Settings record from the Settings table
	/// </summary>
	[Table]
	public class Settings : JsonObject {
		/// <summary>
		/// Record id
		/// </summary>
		[Primary]
		public int? idSettings;
		/// <summary>
		/// Database version
		/// </summary>
		[ReadOnly]
		public int DbVersion;
		/// <summary>
		/// Display skin to use
		/// </summary>
		[DefaultValue("default")]
		public string Skin;
		/// <summary>
		/// The application version
		/// </summary>
		[DoNotStore]
		public string AppVersion {
			get { return WebServer.AppVersion; }
		}
		/// <summary>
		/// Record id
		/// </summary>
		public override int? Id {
			get { return idSettings; }
			set { idSettings = value; }
		}
	}

}
