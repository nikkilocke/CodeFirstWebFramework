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

namespace CodeFirstWebFramework {
	/// <summary>
	/// Log destinations
	/// </summary>
	[Flags]
	public enum LogDestination {
		Log = 1,
		StdOut = 2,
		StdErr = 4,
		Null = 0
	}
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
		/// True if all session creation is to be logged
		/// </summary>
		public bool SessionLogging;
		/// <summary>
		/// True if all database access sql is to be logged
		/// </summary>
		public int DatabaseLogging;
		/// <summary>
		/// True if post parameters are to be logged
		/// </summary>
		public bool PostLogging;
		/// <summary>
		/// Where log output goes to for each LogType enum
		/// </summary>
		public LogDestination[] LogDestinations = new LogDestination[] {
			LogDestination.Log,
			LogDestination.Log,
			LogDestination.Log,
			LogDestination.Log | LogDestination.StdOut,
			LogDestination.Log | LogDestination.StdErr
			};
		/// <summary>
		/// Command line flags extracted from program command line
		/// </summary>
		static public NameValueCollection CommandLineFlags;

		/// <summary>
		/// The default (and only) Config file
		/// </summary>
		public static Config Default = new Config();

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
							Namespace = Namespace
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
			WebServer.Log(LogType.Startup, "Loading config from {0}", filename);
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
				new DailyLog(Path.Combine(DataPath, EntryModule + "Logs")).WriteLine("Started:config=" + configName);
				Utils.Check(!string.IsNullOrEmpty(Config.Default.ConnectionString), "You must specify a ConnectionString in the " + configName + " file");
			} catch (Exception ex) {
				WebServer.Log(LogType.Error, ex.ToString());
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
		/// Details of the namespace
		/// </summary>
		[JsonIgnore]
		public Namespace NamespaceDef;

		/// <summary>
		/// Search the list of folders for a file matching the filename
		/// </summary>
		/// <param name="filename">Like a url - e.g. "admin/settings.html"</param>
		/// <returns></returns>
		public FileInfo FileInfo(string filename) {
			FileInfo f;
			string folder = Path.Combine(Config.DataPath, ServerName);
			if (filename.StartsWith("/"))
				filename = filename.Substring(1);
			f = new FileInfo(Path.Combine(folder, filename));
			Utils.Check(f.FullName.StartsWith(new FileInfo(folder).FullName), "Illegal file access");
			if (f.Exists)
				return f;
			f = new FileInfo(Path.Combine(ServerName, filename));
			if (f.Exists)
				return f;
			f = new FileInfo(Path.Combine(Config.DataPath, Namespace, filename));
			if (f.Exists)
				return f;
			f = new FileInfo(Path.Combine(Namespace, filename));
			if (f.Exists)
				return f;
			f = new FileInfo(Path.Combine(Config.DataPath, CodeFirstWebFramework.Config.Default.ServerName, filename));
			if (f.Exists)
				return f;
			f = new FileInfo(Path.Combine(CodeFirstWebFramework.Config.Default.ServerName, filename));
			if (f.Exists)
				return f;
			f = new FileInfo(Path.Combine(Config.DataPath, CodeFirstWebFramework.Config.DefaultNamespace, filename));
			if (f.Exists)
				return f;
			return new FileInfo(Path.Combine(CodeFirstWebFramework.Config.DefaultNamespace, filename));
		}

		/// <summary>
		/// Search the list of folders for a folder matching the foldername
		/// </summary>
		public DirectoryInfo DirectoryInfo(string foldername) {
			DirectoryInfo f;
			string folder = Path.Combine(Config.DataPath, ServerName);
			if (foldername.StartsWith("/"))
				foldername = foldername.Substring(1);
			f = new DirectoryInfo(Path.Combine(folder, foldername));
			Utils.Check(f.FullName.StartsWith(new FileInfo(folder).FullName), "Illegal file access");
			if (f.Exists)
				return f;
			f = new DirectoryInfo(Path.Combine(ServerName, foldername));
			if (f.Exists)
				return f;
			f = new DirectoryInfo(Path.Combine(Config.DataPath, Namespace, foldername));
			if (f.Exists)
				return f;
			f = new DirectoryInfo(Path.Combine(Namespace, foldername));
			if (f.Exists)
				return f;
			f = new DirectoryInfo(Path.Combine(Config.DataPath, CodeFirstWebFramework.Config.Default.ServerName, foldername));
			if (f.Exists)
				return f;
			f = new DirectoryInfo(Path.Combine(CodeFirstWebFramework.Config.Default.ServerName, foldername));
			if (f.Exists)
				return f;
			f = new DirectoryInfo(Path.Combine(Config.DataPath, CodeFirstWebFramework.Config.DefaultNamespace, foldername));
			if (f.Exists)
				return f;
			return new DirectoryInfo(Path.Combine(CodeFirstWebFramework.Config.DefaultNamespace, foldername));
		}

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

		/// <summary>
		/// Load the contents of the file from one of the search folders.
		/// </summary>
		/// <param name="filename">Like a url</param>
		public string LoadFile(string filename) {
			FileInfo f = FileInfo(filename.ToLower());
			if (!f.Exists)
				f = FileInfo(filename);
			Utils.Check(f.Exists, "File not found:{0}", filename);
			return LoadFile(f);
		}

		/// <summary>
		/// Load the contents of a specific file.
		/// If it is a .tmpl file, perform our extra substitutions to support {{include}}, //{{}}, '!{{}} and {{{}}}
		/// </summary>
		public string LoadFile(FileInfo f) {
			Utils.Check(f.Exists, "File not found:{0}", f.Name);
			using (StreamReader s = new StreamReader(f.FullName, AppModule.Encoding)) {
				string text = s.ReadToEnd();
				if (f.Extension == ".tmpl") {
					text = Regex.Replace(text, @"\{\{ *include +(.*) *\}\}", delegate (Match m) {
						return LoadFile(m.Groups[1].Value);
					});
					text = Regex.Replace(text, @"//[\s]*{{([^{}]+)}}[\s]*$", "{{$1}}");
					text = Regex.Replace(text, @"'!{{([^{}]+)}}'", "{{$1}}");
					text = Regex.Replace(text, @"{{{([^{}]+)}}}", "\x1{{$1}}\x2");
				}
				return text;
			}
		}

		/// <summary>
		/// Load a template and perform Mustache substitutions on it using obj.
		/// </summary>
		public string LoadTemplate(string filename, object obj) {
			try {
				if (Path.GetExtension(filename) == "")
					filename += ".tmpl";
				return TextTemplate(LoadFile(filename), obj);
			} catch (DatabaseException) {
				throw;
			} catch (Exception ex) {
				throw new CheckException(ex, "{0}:{1}", filename, ex.Message);
			}
		}

		/// <summary>
		/// Perform Mustache substitutions on a template
		/// </summary>
		/// <param name="text">The template</param>
		/// <param name="obj">The object to use</param>
		public string TextTemplate(string text, object obj) {
			FormatCompiler compiler = new FormatCompiler();
			compiler.RemoveNewLines = false;
			Generator generator = compiler.Compile(text);
			string result = generator.Render(obj);
			result = Regex.Replace(result, "\x1(.*?)\x2", delegate(Match m) {
				return HttpUtility.HtmlEncode(m.Groups[1].Value).Replace("\n", "\n<br />");
			}, RegexOptions.Singleline);
			return result;
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
