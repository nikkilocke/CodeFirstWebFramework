using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using Mustache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	public class AppSettings {
		[JsonIgnore]
		ServerSettings _default;
		static string DefaultModule = System.Reflection.Assembly.GetExecutingAssembly().Name();
		public string Database = "SQLite";
		public string Module = DefaultModule;
		public string ConnectionString = "Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).Replace(@"\", "/")
			+ @"/" + DefaultModule + "/" + DefaultModule + ".db";
		[JsonIgnore]
		public string Filename;
		public int Port = 8080;
		public int SlowQuery = 100;
		public string ServerName = "localhost";
		public string Email = "root@localhost";
		public int SessionExpiryMinutes = 30;
		public List<ServerSettings> Servers = new List<ServerSettings>();
		public bool SessionLogging;
		public int DatabaseLogging;
		public bool PostLogging;
		static public NameValueCollection CommandLineFlags;

		public static AppSettings Default = new AppSettings();

		public void Save(string filename) {
			using (StreamWriter w = new StreamWriter(filename))
				w.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented));
		}

		[JsonIgnore]
		public ServerSettings DefaultServer {
			get {
				lock (this) {
					if (_default == null)
						_default = new ServerSettings() {
							ServerName = ServerName,
							Email = Email,
							Module = Module
						};
				}
				return _default;
			}
		}

		public ServerSettings SettingsForHost(string host) {
			if (Servers != null) {
				ServerSettings settings = Servers.FirstOrDefault(s => s.Matches(host));
				if (settings != null)
					return settings;
			}
			return DefaultServer;

		}

		static public void Load(string filename) {
			WebServer.Log("Loading config from {0}", filename);
			using (StreamReader s = new StreamReader(filename)) {
				Default = Utils.JsonTo<AppSettings>(s.ReadToEnd());
				Default.Filename = Path.GetFileNameWithoutExtension(filename);
			}
		}
 
	}

	public class ServerSettings {
		public string ServerName;
		public string ServerAlias;
		public string Module;
		public string Email;
		public string Title;
		public string Database;
		public string ConnectionString;
		[JsonIgnore]
		public ModuleDef ModuleDef;

		public FileInfo FileInfo(string filename) {
			FileInfo f;
			if (filename.StartsWith("/"))
				filename = filename.Substring(1);
			f = new FileInfo(Path.Combine(ServerName, filename));
			Utils.Check(f.FullName.StartsWith(new FileInfo(ServerName).FullName), "Illegal file access");
			if (f.Exists)
				return f;
			f = new FileInfo(Path.Combine(Module, filename));
			if (f.Exists)
				return f;
			return new FileInfo(Path.Combine(CodeFirstWebFramework.AppSettings.Default.ServerName, filename));
		}

		public bool Matches(string host) {
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

		public string LoadFile(string filename) {
			FileInfo f = FileInfo(filename.ToLower());
			if (!f.Exists)
				f = FileInfo(filename); ;
			Utils.Check(f.Exists, "File not found:{0}", filename);
			using (StreamReader s = new StreamReader(f.FullName, AppModule.Encoding)) {
				string text = s.ReadToEnd();
				text = Regex.Replace(text, @"\{\{ *include +(.*) *\}\}", delegate(Match m) {
					return LoadFile(m.Groups[1].Value);
				});
				text = Regex.Replace(text, @"//[\s]*{{([^{}]+)}}[\s]*$", "{{$1}}");
				text = Regex.Replace(text, @"'!{{([^{}]+)}}'", "{{$1}}");
				text = Regex.Replace(text, @"{{{([^{}]+)}}}", "\x1{{$1}}\x2");
				return text;
			}
		}

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

	[Table]
	public class Settings : JsonObject {
		public int DbVersion;
	}

}
