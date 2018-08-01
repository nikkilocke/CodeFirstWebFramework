using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using System.Reflection;
using System.Threading;
using Mustache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeFirstWebFramework {
	/// <summary>
	/// Attribute to indicate a string field in an AppModule is to be filled from an XML element
	/// of the same name (but in lower case) in a template (if such an element exists) when
	/// the template is substituted in default.tmpl
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class TemplateSectionAttribute : Attribute {
	}

	/// <summary>
	/// Attribute to indicate what file extensions a module can handle
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public class HandlesAttribute : Attribute {

		/// <summary>
		/// Attribute to indicate which extensions an AppModule can handle
		/// </summary>
		public HandlesAttribute(params string [] extensions) {
			Extensions = extensions;
		}

		/// <summary>
		/// List of extensions this AppModule can handle
		/// </summary>
		public IEnumerable<string> Extensions;

	}
	/// <summary>
	/// Base class for all app modules.
	/// Derive a class from this to server a folder of that name (you can add "Module" on the end of the name to avoid name clashes)
	/// Create public methods to serve requests in that folder. If the method has arguments, the named arguments will be filled
	/// in from the GET or POST request arguments (converting json to C# objects as required) - see Call below. 
	/// If the method returns something, it will be returned using WriteResponse (below).
	/// If the method is void, a template in the corresponding folder will be filled in, with the AppModule as the argument,
	/// and returned.
	/// </summary>
	
	[Handles(".html")]
	public abstract class AppModule : IDisposable {
		static int lastJob;							// Last batch job
		static Dictionary<int, BatchJob> jobs = new Dictionary<int, BatchJob>();
		private Settings _settings;

		/// <summary>
		/// Character encoding for web output
		/// </summary>
		public static Encoding Encoding = Encoding.GetEncoding(1252);

		/// <summary>
		/// Charset for web output
		/// </summary>
		public static string Charset = "windows-1252";

		/// <summary>
		/// Set to true if the web server is allowed to cache this page. Normally false, as pages are generated dynamically.
		/// </summary>
		public bool CacheAllowed { get; protected set; }

		/// <summary>
		/// Security information about this module
		/// </summary>
		public ModuleInfo Info;

		private bool? _securityOn;

		/// <summary>
		/// True if there are users in the database, so security should be checked
		/// </summary>
		public bool SecurityOn {
			get {
				if (_securityOn == null)
					_securityOn = Database.QueryOne("SELECT idUser FROM User") != null;
				return (bool)_securityOn;
			}
		}

		/// <summary>
		/// Access level of the currently logged in user
		/// </summary>
		public int UserAccessLevel;

		/// <summary>
		/// True if user does not have write access
		/// </summary>
		public bool ReadOnly {
			get { return UserAccessLevel <= AccessLevel.ReadOnly; }
		}

		/// <summary>
		/// True if user does have write access
		/// </summary>
		public bool ReadWrite {
			get { return UserAccessLevel >= AccessLevel.ReadWrite; }
		}

		/// <summary>
		/// True if user has Admin access
		/// </summary>
		public bool Admin {
			get { return UserAccessLevel >= AccessLevel.Admin; }
		}

		/// <summary>
		/// Version suffix for including in url's to defeat long-term caching of (e.g.) javascript and css files
		/// </summary>
		public string VersionSuffix {
			get { return WebServer.VersionSuffix; }
		}

		Database _db;

		/// <summary>
		/// Close the database
		/// </summary>
		public void CloseDatabase() {
			if (_db != null) {
				_db.Dispose();
				_db = null;
			}
		}

		/// <summary>
		/// The Database for this AppModule
		/// </summary>
		public virtual Database Database {
			get {
				lock (this) {
					if (_db == null) {
						_db = Server.NamespaceDef.GetDatabase(Server);
						_db.Module = this;
					}
				}
				return _db;
			}
		}

		/// <summary>
		/// The Settings record from the database
		/// </summary>
		public Settings Settings {
			get {
				if (_settings == null) {
					_settings = Database.Get<Settings>(1);
				}
				return _settings;
			}
		}

		/// <summary>
		/// Force the Settings record to be reloaded from the database
		/// </summary>
		public void ReloadSettings() {
			_settings = null;
		}

		/// <summary>
		/// So templates can access Session
		/// </summary>
		[JsonIgnore]
		public WebServer.Session Session;

		/// <summary>
		/// Session data in dynamic form
		/// </summary>
		[JsonIgnore]
		public dynamic SessionData {
			get { return Session.Object; }
		}

		/// <summary>
		/// The data which is logged when the web request completes.
		/// </summary>
		public StringBuilder LogString;

		/// <summary>
		/// Log to LogString (for showing in console with response data)
		/// </summary>
		public void Log(string s) {
			if (LogString != null) LogString.AppendLine(s);
		}

		/// <summary>
		/// Log to LogString (for showing in console with response data)
		/// </summary>
		public void Log(string format, params object[] args) {
			if (LogString != null) LogString.AppendFormat(format + "\r\n", args);
		}

		/// <summary>
		/// Close the database (unless a batch job is using it)
		/// </summary>
		public void Dispose() {
			if (_db != null && Batch == null) {
				// Don't close database if a batch job is using it
				CloseDatabase();
			}
		}

		/// <summary>
		/// The Context from the web request that created this AppModule
		/// </summary>
		public HttpListenerContext Context;

		/// <summary>
		/// Any exception thrown handling a web request
		/// </summary>
		public Exception Exception;

		/// <summary>
		/// Module menu - line 2 of page top menu
		/// </summary>
		public List<MenuOption> Menu;
		
		/// <summary>
		/// Alert message to show user
		/// </summary>
		public string Message;

		/// <summary>
		/// The current Method (from the url - lower case).
		/// Used by Respond to decide which template file to use.
		/// </summary>
		public string Method;

		/// <summary>
		/// The current module (from the url - lower case).
		/// Used by Respond to decide which template file to use.
		/// </summary>
		public string Module;

		/// <summary>
		/// The original value of Method (from the url - lower case).
		/// </summary>
		public string OriginalMethod;

		/// <summary>
		/// The original value of Module (from the url - lower case).
		/// </summary>
		public string OriginalModule;

		/// <summary>
		/// The Server handling this request
		/// </summary>
		public ServerConfig Server;

		/// <summary>
		/// The Namespace in which this module is running
		/// </summary>
		public Namespace ActiveModule;

		/// <summary>
		/// Parameters from Url
		/// </summary>
		public NameValueCollection GetParameters;

		/// <summary>
		/// Get &amp; Post parameters combined
		/// </summary>
		public JObject Parameters = new JObject();

		/// <summary>
		/// Parameters from POST
		/// </summary>
		public JObject PostParameters;

		/// <summary>
		/// The Web Request (from Context)
		/// </summary>
		public HttpListenerRequest Request {
			get { return Context.Request; }
		}

		/// <summary>
		/// The Web Response (from Context)
		/// </summary>
		public HttpListenerResponse Response {
			get { return Context.Response; }
		}

		/// <summary>
		/// Used for the web page title
		/// </summary>
		[TemplateSection]
		public string Title;

		/// <summary>
		/// All Modules running batch jobs
		/// </summary>
		static public IEnumerable<BatchJob> Jobs {
			get { return jobs.Values; }
		}

		/// <summary>
		/// The Config file data
		/// </summary>
		public Config Config {
			get { return Config.Default; }
		}

		/// <summary>
		/// Goes into the web page header
		/// </summary>
		[TemplateSection]
		public string Head;

		/// <summary>
		/// Additional text to include in the template header.
		/// </summary>
		public string HeaderScript;

		/// <summary>
		/// Goes into the web page body
		/// </summary>
		[TemplateSection]
		public string Body;

		/// <summary>
		/// True if a response has been sent to the request (and the default response should not be created)
		/// </summary>
		public bool ResponseSent { get; private set; }

		/// <summary>
		/// Today's date (yyyy-MM-dd)
		/// </summary>
		public string Today {
			get { return Utils.Today.ToString("yyyy-MM-dd"); }
		}

		/// <summary>
		/// List of modules for templates (e.g. to auto-generate a module menu)
		/// </summary>
		public virtual IEnumerable<ModuleInfo> Modules {
			get { return Server.NamespaceDef.Modules; }
		}


		/// <summary>
		/// The Form to render, if any
		/// </summary>
		public BaseForm Form;

		/// <summary>
		/// Background batch job (e.g. import, restore)
		/// </summary>
		public class BatchJob {
			AppModule _module;
			string _redirect;
			int _record;

			/// <summary>
			/// Create a batch job that redirects back to the module's original method on completion
			/// </summary>
			/// <param name="module">Module containing Database, Session, etc.</param>
			/// <param name="action">Action to run the job</param>
			public BatchJob(AppModule module, Action action)
				: this(module, null, action) {
			}

			/// <summary>
			/// Create a batch job that redirects somewhere specific
			/// </summary>
			/// <param name="module">Module containing Database, Session, etc.</param>
			/// <param name="redirect">Where to redirect to after batch (or null for default)</param>
			/// <param name="action">Action to run the job</param>
			public BatchJob(AppModule module, string redirect, Action action) {
				_module = module;
				_redirect = redirect ?? "/" + module.Module.ToLower() + "/" + module.Method.ToLower() + ".html";
				Status = "";
				Records = 100;
				module.Batch = this;
				// Get the next job number
				lock (jobs) {
					Id = ++lastJob;
					jobs[Id] = this;
				}
				module.Log("Started batch job {0}", Id);
				new Task(delegate() {
					Thread.Sleep(500);	// 1/2 second to allow module page to return
					runBatch(action);
					_module.CloseDatabase();
					Thread.Sleep(60000);	// 1 minute
					lock (jobs) {
						jobs.Remove(Id);
					}
				}).Start();
				module.Module = "admin";
				module.Method = "batch";
			}

			void runBatch(Action action) {
				try {
					_module.LogString = new StringBuilder();
					_module.Log("Running batch job {0}", Id);
					action();
				} catch (Exception ex) {
					_module.Log("Batch job {0} Exception: {1}", Id, ex);
					Status = "An error occurred";
					Error = ex.Message;
				}
				_module.Log("Finished batch job {0}", Id);
				CodeFirstWebFramework.Log.Info.WriteLine(_module.LogString.ToString());
				_module.LogString = null;
				_module.Batch = null;
				Finished = true;
			}

			/// <summary>
			/// Job id
			/// </summary>
			public int Id { get; private set; }

			/// <summary>
			/// Error message (e.g. on exception)
			/// </summary>
			public string Error;

			/// <summary>
			/// True if the batch job has finished
			/// </summary>
			public bool Finished;

			/// <summary>
			/// For progress display
			/// </summary>
			public int PercentComplete {
				get {
					return Records == 0 ? 100 : 100 * Record / Records;
				}
			}

			/// <summary>
			/// To indicate progress (0...Records)
			/// </summary>
			public virtual int Record {
				get {
					return _record;
				}
				set {
					_record = value;
				}
			}

			/// <summary>
			/// Total number of records (for progress bar)
			/// </summary>
			public int Records;

			/// <summary>
			/// Where redirecting to on completion
			/// </summary>
			public string Redirect {
				get {
					return _redirect == null ? null : _redirect + (_redirect.Contains('?') ? '&' : '?') + "message=" 
						+ (string.IsNullOrEmpty(_module.Message) ? "Job completed" : HttpUtility.UrlEncode(_module.Message));
				}
			}

			/// <summary>
			/// For status/progress display
			/// </summary>
			public string Status;
		}

		/// <summary>
		/// BatchJob started by this module
		/// </summary>
		public BatchJob Batch;

		/// <summary>
		/// Get batch job from id (for status/progress display)
		/// </summary>
		public static BatchJob GetBatchJob(int id) {
			BatchJob job;
			return jobs.TryGetValue(id, out job) ? job : null;
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public AppModule() {
		}

		/// <summary>
		/// Make a new module with settings copied from another
		/// </summary>
		public AppModule(AppModule module) {
			CopyFrom = module;
		}

		/// <summary>
		/// Copy main settings from another AppModule
		/// </summary>
		public AppModule CopyFrom {
			set {
				Server = value.Server;
				ActiveModule = value.ActiveModule;
				Session = value.Session;
				LogString = value.LogString;
				Context = value.Context;
				Title = value.Title;
				Module = value.Module;
				OriginalModule = value.OriginalModule;
				Method = value.Method;
				OriginalMethod = value.OriginalMethod;
				GetParameters = value.GetParameters;
				Parameters = value.Parameters;
				PostParameters = value.PostParameters;
				Info = value.Info;
				UserAccessLevel = value.UserAccessLevel;
			}
		}

		/// <summary>
		/// Convert a "binary" string (decoded using windows-1252) to the correct encoding
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		protected string ConvertEncoding(string s) {
			if (Charset == "windows-1252")
				return s;
			return Encoding.GetString(Encoding.GetEncoding(1252).GetBytes(s));
		}

		/// <summary>
		/// Responds to a Url request. Set up the AppModule variables and call the given method
		/// </summary>
		public void Call(HttpListenerContext context, string moduleName, string methodName) {
			Context = context;
			Log("({0}) ", Server.ServerName);
			OriginalModule = Module = moduleName.ToLower();
			OriginalMethod = Method = (methodName ?? "default").ToLower();
			LogString.Append(GetType().Name + ":" + Title + ":");
			// Collect get parameters
			GetParameters = new NameValueCollection();
			for (int i = 0; i < Request.QueryString.Count; i++) {
				string key = Request.QueryString.GetKey(i);
				string value = Request.QueryString[i];
				if (key == null) {
					GetParameters[value] = "";
				} else {
					GetParameters[key] = value;
					if (key == "message")
						Message = value;
				}
			}
			// Add into parameters array
			Parameters.AddRange(GetParameters);
			// Collect POST parameters
			if (context.Request.HttpMethod == "POST") {
				PostParameters = new JObject();
				if (context.Request.ContentType != null) {
					string data;
					// Encoding 1252 will give exactly 1 character per input character, without translation
					using (StreamReader s = new StreamReader(context.Request.InputStream, Encoding.GetEncoding(1252))) {
						data = s.ReadToEnd();
					}
					if (context.Request.ContentType.StartsWith("multipart/form-data")) {
						string boundary = Regex.Split(context.Request.ContentType, "boundary=")[1].Trim();
						if (boundary.StartsWith("\"") && boundary.EndsWith("\""))
							boundary = boundary.Substring(1, boundary.Length - 2);
						boundary = "--" + boundary;
						foreach (string part in Regex.Split("\r\n" + data, ".." + boundary, RegexOptions.Singleline)) {
							if (part.Trim() == "" || part.Trim() == "--") continue;
							int pos = part.IndexOf("\r\n\r\n");
							string headers = part.Substring(0, pos);
							string value = part.Substring(pos + 4);
							Match match = new Regex(@"form-data; name=""?(\w+)""?").Match(headers);
							if (match.Success) {
								// This is a file upload
								string field = match.Groups[1].Value;
								match = new Regex(@"; filename=""(.*)""").Match(headers);
								if (match.Success) {
									PostParameters.Add(field, new UploadedFile(Path.GetFileName(match.Groups[1].Value), value).ToJToken());
								} else {
									PostParameters.Add(field, ConvertEncoding(value));
								}
							}
						}
					} else {
						PostParameters.AddRange(HttpUtility.ParseQueryString(ConvertEncoding(data)));
					}
					Parameters.AddRange(PostParameters);
					if (CodeFirstWebFramework.Log.PostData.On) {
						foreach (KeyValuePair<string, JToken> p in PostParameters) {
							CodeFirstWebFramework.Log.PostData.WriteLine("\t{0}={1}", p.Key,
								p.Key == "json" ? JObject.Parse(p.Value.ToString()).ToString(Formatting.Indented).Replace("\n", "\n\t") :
								p.Value.Type == JTokenType.Object ? "file " + ((JObject)p.Value)["Name"] :
								p.Value.ToString());
						}
					}
				}
			}
			MethodInfo method = null;
			try {
				object o = CallMethod(out method);
				if (method == null) {
					WriteResponse("Page /" + Module + "/" + Method + ".html not found", "text/html", HttpStatusCode.NotFound);
					return;
				}
				if (!ResponseSent) {
					// Method has not sent a response - do the default response
					Response.AddHeader("Expires", DateTime.UtcNow.ToString("R"));
					if (method.ReturnType == typeof(void))
						Respond();									// Builds response from template
					else if(o is BaseForm)
						(o as BaseForm).Show();
					else
						WriteResponse(o, null, HttpStatusCode.OK);	// Builds response from return value
				}
			} catch (Exception ex) {
				Log("Exception: {0}", ex);
				while (ex is TargetInvocationException) {
					// Strip off TargetInvokationExceptions so message is meaningful
					ex = ex.InnerException;
				}
				if (ex is DatabaseException)
					Log(((DatabaseException)ex).Sql);	// Log Sql of all database exceptions
				if (method == null || method.ReturnType == typeof(void) || method.ReturnType.IsSubclassOf(typeof(BaseForm))) throw;	// Will produce exception page
				// Send an AjaxReturn object indicating the error
				WriteResponse(new AjaxReturn() { error = ex.Message }, null, HttpStatusCode.OK);
			}
		}

		/// <summary>
		/// Call the method named by Method, and return its result
		/// </summary>
		/// <param name="method">Also return the MethodInfo so caller knows what return type it has.
		/// Will be set to null if there is no such named method.</param>
		public virtual object CallMethod(out MethodInfo method) {
			object self = this;
			List<object> parms = new List<object>();
			method = this.GetType().GetMethod(Method, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			if (method == null) {
				// AppModule doesn't have the method - see if it has an Implementation attribute whose Helper does
				self = null;
				foreach(ImplementationAttribute implementation in this.GetType().GetCustomAttributes<ImplementationAttribute>()) {
					method = implementation.Helper.GetMethod(Method, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
					if (method != null) {
						self = Activator.CreateInstance(implementation.Helper, this);
						break;
					}
				}
				if (self == null)
					return null;
			}
			if (!HasAccess(Info, Method, out UserAccessLevel)) {
				if (Session.User == null && method.ReturnType != typeof(AjaxReturn)) {
					SessionData.redirect = Request.Url.PathAndQuery;
					Redirect("/admin/login");
					return null;
				}
				throw new CheckException("Unauthorised access");
			}
			string moduleName = GetType().Name;
			if (moduleName.EndsWith("Module"))
				moduleName = moduleName.Substring(0, moduleName.Length - 6);
			Title = moduleName.UnCamel();
			if (method.Name != "Default") Title += " - " + method.Name.UnCamel();
			// Collect any parameters required by the method from the GET/POST parameters
			foreach (ParameterInfo p in method.GetParameters()) {
				JToken val = Parameters[p.Name];
				object o;
				Utils.Check(val != null, "Missing parameter {0}", p.Name);
				try {
					if (p.ParameterType == typeof(int)
						|| p.ParameterType == typeof(decimal)
						|| p.ParameterType == typeof(string)
						|| p.ParameterType == typeof(DateTime)) {
						// Plain parameter - convert directly
						o = val.ToObject(p.ParameterType);
					} else if (p.ParameterType == typeof(UploadedFile)) {
						// Uploaded file - "null" means null
						if (val.ToString() == "null")
							o = null;
						else
							o = val.ToObject(typeof(UploadedFile));
					} else if (val.Type == JTokenType.String && val.ToString() == "null") {
						o = null;		// "null" means null for any other type too
					} else if (p.ParameterType == typeof(int?)
						|| p.ParameterType == typeof(decimal?)) {
						// Have dealt with "null" - convert in? or decimal?
						o = val.ToObject(p.ParameterType);
					} else {
						// Everything else is assumed to arrive as a json string
						o = val.ToObject<string>().JsonTo(p.ParameterType);
					}
					parms.Add(o);
				} catch(Exception ex) {
					Match m = Regex.Match(ex.Message, "Error converting value (.*) to type '(.*)'. Path '(.*)', line");
					if(m.Success)
						throw new CheckException(ex, "{0} is an invalid value for {1}", m.Groups[1], m.Groups[3]);
					throw new CheckException(ex, "Could not convert {0} to {1}", val, p.ParameterType.Name);
				}
			}
			Init();
			return method.Invoke(self, parms.Count == 0 ? null : parms.ToArray());
		}

		/// <summary>
		/// Check the security for access to a url
		/// </summary>
		public bool HasAccess(string uri) {
			ModuleInfo info = Server.NamespaceDef.ParseUri(uri, out string filename);
			return info == null ? true : HasAccess(info, Path.GetFileNameWithoutExtension(filename) + (Array.IndexOf(uri.Split('?','&'), "id=0") > 0 ? "save" : ""), out int accesslevel);
		}

		/// <summary>
		/// Check the security for access to a method
		/// </summary>
		/// <param name="info">The ModuleInfo for the relevant module</param>
		/// <param name="mtd">The method name (lower case)</param>
		/// <param name="accessLevel">The user's access level to this method</param>
		public bool HasAccess(ModuleInfo info, string mtd, out int accessLevel) {
			if (!SecurityOn) {
				// No security
				accessLevel = AccessLevel.Admin;
				return true;
			} else if (info != null) {
				int level = info.Auth.AccessLevel;
				if (info.AuthMethods.TryGetValue(mtd, out AuthAttribute l)) {
					level = l.AccessLevel;
					mtd = l.Name;
				} else {
					bool writeAccess = false;
					if (mtd.EndsWith("save")) {
						mtd = mtd.Substring(0, mtd.Length - 4);
						writeAccess = true;
					} else if (mtd.EndsWith("delete")) {
						mtd = mtd.Substring(0, mtd.Length - 6);
						writeAccess = true;
					}
					if (writeAccess) {
						if (info.AuthMethods.TryGetValue(mtd, out l)) {
							level = l.AccessLevel;
							mtd = l.Name;
						} else
							mtd = "-";
						if (level == AccessLevel.ReadOnly)
							level = AccessLevel.ReadWrite;
					} else {
						mtd = "-";
					}
				}
				if (Session.User != null) {
					accessLevel = Session.User.AccessLevel;
					if (Session.User.ModulePermissions) {
						JObject p = Database.QueryOne("SELECT FunctionAccessLevel FROM Permission WHERE UserId = " + Session.User.idUser
							+ " AND Module = " + Database.Quote(info.Auth.Name) + " AND Method = " + Database.Quote(mtd));
						if (p != null)
							accessLevel = p.AsInt("FunctionAccessLevel");
					}
				} else
					accessLevel = AccessLevel.None;
				return accessLevel >= level;
			} else {
				accessLevel = AccessLevel.None;
				return true;
			}
		}

		/// <summary>
		/// Load the contents of the file from one of the search folders.
		/// </summary>
		/// <param name="filename">Like a url</param>
		public string LoadFile(string filename) {
			IFileInfo f = FileInfo(filename.ToLower());
			if (!f.Exists)
				f = FileInfo(filename);
			Utils.Check(f.Exists, "File not found:'{0}'", filename);
			return LoadFile(f);
		}

		/// <summary>
		/// Load the contents of a specific file.
		/// If it is a .tmpl file, perform our extra substitutions to support {{include}}, //{{}}, '!{{}} and {{{}}}
		/// </summary>
		public string LoadFile(IFileInfo f) {
			Utils.Check(f.Exists, "File not found:'{0}'", f.Path + f.Name);
			string text = f.Content(this);
			if (f.Extension == ".tmpl") {
				text = Regex.Replace(text, @"\{\{ *include +(.*?) *\}\}", delegate (Match m) {
					return LoadFile(m.Groups[1].Value);
				});
				text = Regex.Replace(text, @"//[\s]*{{([^{}]+)}}[\s]*$", "{{$1}}");
				text = Regex.Replace(text, @"'!{{([^{}]+)}}'", "{{$1}}");
				text = Regex.Replace(text, @"{{{([^{}]+)}}}", "\x1{{$1}}\x2");
			}
			return text;
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
			result = Regex.Replace(result, "\x1(.*?)\x2", delegate (Match m) {
				return HttpUtility.HtmlEncode(m.Groups[1].Value).Replace("\n", "\n<br />");
			}, RegexOptions.Singleline);
			return result;
		}


		/// <summary>
		/// Method to call if no method supplied in url
		/// </summary>
		public virtual void Default() {
			if (!FileInfo("/" + Module + "/default.tmpl").Exists) {
				Body = "Please choose an option above";
				WriteResponse(LoadTemplate("default", this), "text/html", System.Net.HttpStatusCode.OK);
			}
		}

		string _help;

		/// <summary>
		/// Return the url of any help file found in the help folder which applies to this module (and maybe method)
		/// </summary>
		public string Help
		{
			get
			{
				if (_help == null) {
					IFileInfo h = FileInfo("/help/" + Module + "_" + Method + ".md");
					if (!h.Exists)
						h = FileInfo("/help/" + Module + ".md");
					_help = h.Exists ? "/help/" + h.Name + ".md" : "";
				}
				return _help;
			}
		}

		/// <summary>
		/// Perform any initialisation or validation that applies to all calls to this module
		/// (e.g. login or supervisor checks)
		/// </summary>
		protected virtual void Init() {
		}

		/// <summary>
		/// Add multiple menu options to the default Menu (checking security - no add if no access)
		/// </summary>
		/// <param name="opts"></param>
		public void InsertMenuOptions(params MenuOption [] opts) {
			foreach (MenuOption o in opts)
				InsertMenuOption(o);
		}
		/// <summary>
		/// Add a menu option to the default Menu (checking security - no add if no access)
		/// </summary>
		public void InsertMenuOption(MenuOption o) {
			if (!HasAccess(o.Url))
				return;
			if (Menu == null)
				Menu = new List<MenuOption>();
			int i;
			for (i = 0; i < Menu.Count; i++) {
				if (Menu[i].Text.StartsWith("New "))
					break;
			}
			Menu.Insert(i, o);
		}

		/// <summary>
		/// Get the IFileInfo matching the filename
		/// </summary>
		/// <param name="filename">Like a url - e.g. "admin/settings.html"</param>
		public IFileInfo FileInfo(string filename) {
			return Server.NamespaceDef.FileSystem.FileInfo(this, filename);
		}

		/// <summary>
		/// Get the IDirectoryInfo matching the foldername
		/// </summary>
		/// <param name="foldername">Like a url - e.g. "admin/settings"</param>
		public IDirectoryInfo DirectoryInfo(string foldername) {
			return Server.NamespaceDef.FileSystem.DirectoryInfo(this, foldername);
		}

		/// <summary>
		/// Load the named template, and render using Mustache from the supplied object.
		/// E.g. {{Body}} in the template will be replaced with the obj.Body.ToString()
		/// Then split into &lt;head&gt; (goes to this.Head) and &lt;body&gt; (goes to this.Body)
		/// (and also any other fields with the TemplateSection attribute).
		/// If no body section, the whole remaining template goes into this.Body.
		/// Then render the default template from this.
		/// </summary>
		public string Template(string filename, object obj) {
			string body = LoadTemplate(filename, obj);
			foreach(FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)) {
				if (!field.IsDefined(typeof(TemplateSectionAttribute)))
					continue;
				field.SetValue(this, ExtractSection(field.Name.ToLower(), ref body, (string)field.GetValue(this) ?? ""));
			}
			if (string.IsNullOrWhiteSpace(Body))
				Body = body;
			return LoadTemplate("default", this);
		}

		/// <summary>
		/// Extract the named html element from the template
		/// </summary>
		/// <param name="name">Element to extract</param>
		/// <param name="template">The template - will have the whole element removed</param>
		/// <param name="defaultValue">Text to return if the named element is not present</param>
		/// <returns>The content of the element, or defaultValue</returns>
		protected string ExtractSection(string name, ref string template, string defaultValue = "") {
			Match m = Regex.Match(template, "<" + name + ">(.*)</" + name + ">", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			if (m.Success) {
				template = template.Replace(m.Value, "");
				return m.Groups[1].Value;
			} else {
				return defaultValue;
			}
		}

		/// <summary>
		/// Perform a web redirect to redirect the browser to another url.
		/// </summary>
		public void Redirect(string url) {
			if (Context == null)
				return;
			Response.Redirect(url);
			WriteResponse("", "text/plain", HttpStatusCode.Redirect);
		}

		/// <summary>
		/// Render the template Module/Method.tmpl from this.
		/// </summary>
		public void Respond() {
			try {
				string filename = Path.Combine(Module, Method).ToLower();
				string page = Template(filename, this);
				WriteResponse(page, "text/html", HttpStatusCode.OK);
			} catch (System.IO.FileNotFoundException ex) {
				Log(ex.ToString());
				Exception = ex;
				WriteResponse(Template("exception", this), "text/html", HttpStatusCode.NotFound);
			} catch (Exception ex) {
				Log(ex.ToString());
				Exception = ex;
				WriteResponse(Template("exception", this), "text/html", HttpStatusCode.InternalServerError);
			}
		}

		/// <summary>
		/// Save an arbitrary JObject to the database
		/// </summary>
		public AjaxReturn SaveRecord(JsonObject record) {
			AjaxReturn retval = new AjaxReturn();
			try {
				if (record.Id <= 0)
					record.Id = null;
				Database.Update(record);
				retval.id = record.Id;
			} catch (Exception ex) {
				Message = ex.Message;
				retval.error = ex.Message;
			}
			return retval;
		}

		/// <summary>
		/// Delete a record from the database
		/// </summary>
		public AjaxReturn DeleteRecord(string tablename, int id) {
			AjaxReturn retval = new AjaxReturn();
			try {
				Database.Delete(tablename, id);
			} catch (Exception ex) {
				Message = ex.Message;
				retval.error = ex.Message;
			}
			return retval;
		}

		/// <summary>
		/// Write the response to an Http request.
		/// </summary>
		/// <param name="o">The object to write ("Operation complete" if null). 
		/// May be a Stream, a string, a byte array or an object. If it is an object,
		/// it is converted to json representation.</param>
		/// <param name="contentType">The content type (suitable default is used if null)</param>
		/// <param name="status">The Http return code</param>
		public void WriteResponse(object o, string contentType, HttpStatusCode status) {
			if (ResponseSent) throw new CheckException("Response already sent");
			ResponseSent = true;
			if (!CacheAllowed) {
				Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
				Response.AddHeader("Pragma", "no-cache");
				Response.AddHeader("Expires", "0");
			}
			Response.StatusCode = (int)status;
			if(status >= HttpStatusCode.BadRequest)
				CodeFirstWebFramework.Log.NotFound.WriteLine("{0} {1}:{2}:Response {3} {4}",
				Request.RemoteEndPoint.Address,
				Request.Headers["X-Forwarded-For"],
				Request.Url,
				(int)status,
				status);
			Response.ContentEncoding = Encoding;
			switch (contentType) {
				case "text/plain":
				case "text/html":
					contentType += ";charset=" + Charset;
					break;
			}
			string logStatus = status.ToString();
			byte[] msg;
			if (o != null) {
				if (o is Stream) {
					// Stream is sent unchanged
					Response.ContentType = contentType ?? "application/binary";
					Response.ContentLength64 = ((Stream)o).Length;
					Log("{0}:{1} bytes ", status, Response.ContentLength64);
					using (Stream r = Response.OutputStream) {
						((Stream)o).CopyTo(r);
					}
					return;
				} else if (o is string) {
					// String is sent unchanged
					msg = Encoding.GetBytes((string)o);
					Response.ContentType = contentType ?? "text/plain;charset=" + Charset;
				} else if (o is byte[]) {
					msg = o as byte[];
					Response.ContentType = contentType ?? "application/binary";
				} else {
					// Anything else is sent as json
					Response.ContentType = contentType ?? "application/json;charset=" + Charset;
					msg = Encoding.GetBytes(o.ToJson());
					if (o is AjaxReturn)
						logStatus = o.ToString();
				}
			} else {
				msg = Encoding.GetBytes("Operation complete");
				Response.ContentType = contentType ?? "text/plain;charset=" + Charset;
			}
			Response.ContentLength64 = msg.Length;
			Log("{0}:{1} bytes ", logStatus, Response.ContentLength64);
			using (Stream r = Response.OutputStream) {
				r.Write(msg, 0, msg.Length);
			}
		}

	}

	/// <summary>
	/// Class to show errors
	/// </summary>
	public class ErrorModule : AppModule {
	}

	/// <summary>
	/// Class to hold details of an uploaded file (from an &lt;input type="file" /&gt;)
	/// </summary>
	public class UploadedFile {

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">field name</param>
		/// <param name="content">file data</param>
		public UploadedFile(string name, string content) {
			Name = name;
			Content = content;
		}

		/// <summary>
		/// File contents - Windows1252 was used to read it in, so saving it as Windows1252 will be an exact binary copy
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// Field name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The file contents as a stream
		/// </summary>
		public Stream Stream() {
			return new MemoryStream(Encoding.GetEncoding(1252).GetBytes(Content));
		}
	}

	/// <summary>
	/// Menu option for the second level menu
	/// </summary>
	public class MenuOption {

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="text">Menu text</param>
		/// <param name="url">Url to go to</param>
		public MenuOption(string text, string url) : this(text, url, true) {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="text">Menu text</param>
		/// <param name="url">Url to go to</param>
		/// <param name="enabled">False if disabled</param>
		public MenuOption(string text, string url, bool enabled) {
			Text = text;
			Url = url;
			Enabled = enabled;
		}

		/// <summary>
		/// Is the option disabled (used in Mustache templates)
		/// </summary>
		public bool Disabled { 
			get { return !Enabled; } 
		}

		/// <summary>
		/// Is the option enabled
		/// </summary>
		public bool Enabled;

		/// <summary>
		/// Html element id - text with no spaces
		/// </summary>
		public string Id {
			get { return Text.Replace(" ", ""); }
		}

		/// <summary>
		/// Menu text
		/// </summary>
		public string Text;

		/// <summary>
		/// Url to go to
		/// </summary>
		public string Url;
	}

	/// <summary>
	/// Generic return type used for Ajax requests
	/// </summary>
	public class AjaxReturn {
#pragma warning disable IDE1006 // Naming Styles
		/// <summary>
		/// Exception message - if not null or empty, request has failed
		/// </summary>
		public string error;
		/// <summary>
		/// Message for user
		/// </summary>
		public string message;
		/// <summary>
		/// Where to redirect to on completion
		/// </summary>
		public string redirect;
		/// <summary>
		/// Ask the user to confirm something, and resubmit with confirm parameter if the user says yes
		/// </summary>
		public string confirm;
		/// <summary>
		/// If a record has been saved, this is the id of the record.
		/// Usually used to re-read the page, especially when the request was to create a new record.
		/// </summary>
		public int? id;
		/// <summary>
		/// Arbitrary data which the caller needs
		/// </summary>
		public object data;
#pragma warning restore IDE1006 // Naming Styles

		/// <summary>
		/// Show as a string (for logs)
		/// </summary>
		public override string ToString() {
			StringBuilder b = new StringBuilder("AjaxReturn");
			if (!string.IsNullOrEmpty(error)) b.AppendFormat(",error:{0}", error);
			if (!string.IsNullOrEmpty(message)) b.AppendFormat(",message:{0}", message);
			if (!string.IsNullOrEmpty(confirm)) b.AppendFormat(",confirm:{0}", confirm);
			if (!string.IsNullOrEmpty(redirect)) b.AppendFormat(",redirect:{0}", redirect);
			return b.ToString();
		}
	}

}
