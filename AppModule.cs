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
using Markdig;

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
				WebServer.Log(_module.LogString.ToString());
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
					if (Config.PostLogging) {
						foreach (KeyValuePair<string, JToken> p in PostParameters) {
							Log("\t{0}={1}", p.Key,
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
			List<object> parms = new List<object>();
			method = this.GetType().GetMethod(Method, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			if (method == null)
				return null;
			if (!HasAccess(Info, Method, out UserAccessLevel)) {
				if (Session.User == null && method.ReturnType == typeof(void)) {
					SessionData.redirect = Request.Url.AbsoluteUri;
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
			return method.Invoke(this, parms.Count == 0 ? null : parms.ToArray());
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
		/// Method to call if no method supplied in url
		/// </summary>
		public virtual void Default() {
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
					FileInfo h = Server.FileInfo("/help/" + Module + "_" + Method + ".md");
					if (!h.Exists)
						h = Server.FileInfo("/help/" + Module + ".md");
					_help = h.Exists ? "/help/" + h.Name : "";
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
		protected void insertMenuOptions(params MenuOption [] opts) {
			foreach (MenuOption o in opts)
				insertMenuOption(o);
		}
		/// <summary>
		/// Add a menu option to the default Menu (checking security - no add if no access)
		/// </summary>
		protected void insertMenuOption(MenuOption o) {
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
		/// Read a Mustache template file from one of the search folders, and perform the 
		/// variable substitutions using obj as the object.
		/// </summary>
		/// <param name="filename">like a url - e.g. "admin/backup" </param>
		/// <param name="obj">Object to use for substitutions</param>
		public string LoadTemplate(string filename, object obj) {
			return Server.LoadTemplate(filename, obj);
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
				field.SetValue(this, extractSection(field.Name.ToLower(), ref body, (string)field.GetValue(this) ?? ""));
			}
			if (string.IsNullOrWhiteSpace(Body))
				Body = body;
			return LoadTemplate("default", this);
		}

		string extractSection(string name, ref string template, string defaultValue = "") {
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
	/// Class to serve files.
	/// This is used if there is no AppModule corresponding to the directory in the web request.
	/// If an html file is requested, it does not exist, but there is a corresponding tmpl file, the template is filled in and returned.
	/// </summary>
	public class FileSender : AppModule {
		/// <summary>
		/// The name of the file (as a url)
		/// </summary>
		protected string Filename;

		/// <summary>
		/// Dictionary to translate file extensions to content types in file responses.
		/// Add entries to this if you have your own mime types.
		/// </summary>
		public static Dictionary<string, string> ContentTypes = new Dictionary<string, string>() {
			{".1", "application/x-troff-man"},
			{".123", "application/vnd.lotus-1-2-3"},
			{".2", "application/x-troff-man"},
			{".3", "application/x-troff-man"},
			{".3dm", "text/vnd.in3d.3dml"},
			{".3dml", "text/vnd.in3d.3dml"},
			{".3g2", "video/3gpp2"},
			{".3gp", "video/3gpp"},
			{".3gpp", "video/3gpp"},
			{".3gpp2", "video/3gpp2"},
			{".4", "application/x-troff-man"},
			{".5", "application/x-troff-man"},
			{".6", "application/x-troff-man"},
			{".669", "audio/x-mod"},
			{".7", "application/x-troff-man"},
			{".726", "audio/32kadpcm"},
			{".8", "application/x-troff-man"},
			{".aa3", "audio/ATRAC3"},
			{".aal", "audio/ATRAC-ADVANCED-LOSSLESS"},
			{".abc", "text/vnd.abc"},
			{".ac", "application/vnd.nokia.n-gage.ac+xml"},
			{".ac3", "audio/ac3"},
			{".acc", "application/vnd.americandynamics.acc"},
			{".acu", "application/vnd.acucobol"},
			{".acutc", "application/vnd.acucorp"},
			{".aep", "application/vnd.audiograph"},
			{".afp", "application/vnd.ibm.modcap"},
			{".ai", "application/postscript"},
			{".aif", "audio/x-aiff"},
			{".aifc", "audio/x-aiff"},
			{".aiff", "audio/x-aiff"},
			{".ami", "application/vnd.amiga.ami"},
			{".amr", "audio/AMR"},
			{".application", "application/x-ms-application"},
			{".apr", "application/vnd.lotus-approach"},
			{".apxml", "application/auth-policy+xml"},
			{".art", "message/rfc822"},
			{".asc", "text/plain"},
			{".asf", "application/vnd.ms-asf"},
			{".aso", "application/vnd.accpac.simply.aso"},
			{".asx", "video/x-ms-asf"},
			{".at3", "audio/ATRAC3"},
			{".atc", "application/vnd.acucorp"},
			{".atom", "application/atom+xml"},
			{".atomcat", "application/atomcat+xml"},
			{".atomsvc", "application/atomsvc+xml"},
			{".atx", "audio/ATRAC-X"},
			{".au", "audio/basic"},
			{".avi", "video/x-msvideo"},
			{".awb", "audio/AMR-WB"},
			{".azf", "application/vnd.airzip.filesecure.azf"},
			{".azs", "application/vnd.airzip.filesecure.azs"},
			{".bar", "application/vnd.qualcomm.brew-app-res"},
			{".bcpio", "application/x-bcpio"},
			{".bdm", "application/vnd.syncml.dm+wbxml"},
			{".bed", "application/vnd.realvnc.bed"},
			{".bh2", "application/vnd.fujitsu.oasysprs"},
			{".bin", "application/octet-stream"},
			{".bkm", "application/vnd.nervana"},
			{".bmi", "application/vnd.bmi"},
			{".bmp", "image/bmp"},
			{".box", "application/vnd.previewsystems.box"},
			{".bpd", "application/vnd.hbci"},
			{".btf", "image/prs.btif"},
			{".btif", "image/prs.btif"},
			{".bz2", "application/x-bzip2"},
			{".c", "text/plain"},
			{".c4d", "application/vnd.clonk.c4group"},
			{".c4f", "application/vnd.clonk.c4group"},
			{".c4g", "application/vnd.clonk.c4group"},
			{".c4p", "application/vnd.clonk.c4group"},
			{".c4u", "application/vnd.clonk.c4group"},
			{".cab", "application/vnd.ms-cab-compressed"},
			{".cc", "text/plain"},
			{".ccc", "text/vnd.net2phone.commcenter.command"},
			{".ccxml", "application/ccxml+xml"},
			{".cdbcmsg", "application/vnd.contact.cmsg"},
			{".cdf", "application/x-netcdf"},
			{".cdkey", "application/vnd.mediastation.cdkey"},
			{".cdxml", "application/vnd.chemdraw+xml"},
			{".cdy", "application/vnd.cinderella"},
			{".cellml", "application/cellml+xml"},
			{".cer", "application/pkix-cert"},
			{".chm", "application/vnd.ms-htmlhelp"},
			{".chrt", "application/vnd.kde.kchart"},
			{".cif", "application/vnd.multiad.creator.cif"},
			{".cii", "application/vnd.anser-web-certificate-issue-initiation"},
			{".cil", "application/vnd.ms-artgalry"},
			{".cl", "application/simple-filter+xml"},
			{".cla", "application/vnd.claymore"},
			{".class", "application/octet-stream"},
			{".clkk", "application/vnd.crick.clicker.keyboard"},
			{".clkp", "application/vnd.crick.clicker.palette"},
			{".clkt", "application/vnd.crick.clicker.template"},
			{".clkw", "application/vnd.crick.clicker.wordbank"},
			{".clkx", "application/vnd.crick.clicker"},
			{".cmc", "application/vnd.cosmocaller"},
			{".cml", "application/cellml+xml"},
			{".cmp", "application/vnd.yellowriver-custom-menu"},
			{".cpio", "application/x-cpio"},
			{".cpkg", "application/vnd.xmpie.cpkg"},
			{".cpl", "application/cpl+xml"},
			{".cpt", "application/mac-compactpro"},
			{".crl", "application/pkix-crl"},
			{".crtr", "application/vnd.multiad.creator"},
			{".csh", "application/x-csh"},
			{".csp", "application/vnd.commonspace"},
			{".css", "text/css"},
			{".cst", "application/vnd.commonspace"},
			{".csv", "text/csv"},
			{".curl", "application/vnd.curl"},
			{".cw", "application/prs.cww"},
			{".cww", "application/prs.cww"},
			{".cxx", "text/plain"},
			{".daf", "application/vnd.Mobius.DAF"},
			{".dataless", "application/vnd.fsdn.seed"},
			{".davmount", "application/davmount+xml"},
			{".dcf", "application/vnd.oma.drm.content"},
			{".dcm", "application/dicom"},
			{".dcr", "application/x-director"},
			{".dd", "application/vnd.oma.dd+xml"},
			{".dd2", "application/vnd.oma.dd2+xml"},
			{".ddd", "application/vnd.fujixerox.ddd"},
			{".deploy", "application/octet-stream"},
			{".dfac", "application/vnd.dreamfactory"},
			{".dir", "application/x-director"},
			{".dis", "application/vnd.Mobius.DIS"},
			{".dist", "application/vnd.apple.installer+xml"},
			{".distz", "application/vnd.apple.installer+xml"},
			{".djv", "image/vnd.djvu"},
			{".djvu", "image/vnd.djvu"},
			{".dll", "application/octet-stream"},
			{".dls", "audio/dls"},
			{".dm", "application/vnd.oma.drm.message"},
			{".dms", "text/vnd.DMClientScript"},
			{".dna", "application/vnd.dna"},
			{".doc", "application/msword"},
			{".docm", "application/vnd.ms-word.document.macroEnabled.12"},
			{".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
			{".dor", "model/vnd.gdl"},
			{".dot", "text/vnd.graphviz"},
			{".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
			{".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
			{".dp", "application/vnd.osgi.dp"},
			{".dpg", "application/vnd.dpgraph"},
			{".dpgraph", "application/vnd.dpgraph"},
			{".dpkg", "application/vnd.xmpie.dpkg"},
			{".dr", "application/vnd.oma.drm.rights+xml"},
			{".drc", "application/vnd.oma.drm.rights+wbxml"},
			{".dsc", "text/prs.lines.tag"},
			{".dssc", "application/dssc+der"},
			{".dtd", "application/xml-dtd"},
			{".dts", "audio/vnd.dts"},
			{".dtshd", "audio/vnd.dts.hd"},
			{".dvc", "application/dvcs"},
			{".dvi", "application/x-dvi"},
			{".dwf", "model/vnd.dwf"},
			{".dxf", "image/vnd.dxf"},
			{".dxp", "application/vnd.spotfire.dxp"},
			{".dxr", "application/x-director"},
			{".ecelp4800", "audio/vnd.nuera.ecelp4800"},
			{".ecelp7470", "audio/vnd.nuera.ecelp7470"},
			{".ecelp9600", "audio/vnd.nuera.ecelp9600"},
			{".edm", "application/vnd.novadigm.EDM"},
			{".edx", "application/vnd.novadigm.EDX"},
			{".efif", "application/vnd.picsel"},
			{".ei6", "application/vnd.pg.osasli"},
			{".el", "text/plain"},
			{".eml", "message/rfc822"},
			{".emm", "application/vnd.ibm.electronic-media"},
			{".emma", "application/emma+xml"},
			{".ent", "text/xml-external-parsed-entity"},
			{".entity", "application/vnd.nervana"},
			{".eol", "audio/vnd.digital-winds"},
			{".eot", "application/vnd.ms-fontobject"},
			{".ep", "application/vnd.bluetooth.ep.oob"},
			{".eps", "application/postscript"},
			{".es3", "application/vnd.eszigno3+xml"},
			{".esf", "application/vnd.epson.esf"},
			{".et3", "application/vnd.eszigno3+xml"},
			{".etx", "text/x-setext"},
			{".evb", "audio/EVRCB"},
			{".evc", "audio/EVRC"},
			{".evw", "audio/EVRCWB"},
			{".exe", "application/octet-stream"},
			{".ext", "application/vnd.novadigm.EXT"},
			{".ez", "application/andrew-inset"},
			{".ez2", "application/vnd.ezpix-album"},
			{".ez3", "application/vnd.ezpix-package"},
			{".f90", "text/plain"},
			{".fbs", "image/vnd.fastbidsheet"},
			{".fdf", "application/vnd.fdf"},
			{".fe_launch", "application/vnd.denovo.fcselayout-link"},
			{".fg5", "application/vnd.fujitsu.oasysgp"},
			{".finf", "application/fastinfoset"},
			{".fit", "image/fits"},
			{".fits", "image/fits"},
			{".flo", "application/vnd.micrografx.flo"},
			{".flv", "video/x-flv"},
			{".flw", "application/vnd.kde.kivio"},
			{".flx", "text/vnd.fmi.flexstor"},
			{".fly", "text/vnd.fly"},
			{".fm", "application/vnd.framemaker"},
			{".fnc", "application/vnd.frogans.fnc"},
			{".fo", "application/vnd.software602.filler.form+xml"},
			{".fpx", "image/vnd.fpx"},
			{".frm", "application/vnd.ufdl"},
			{".fsc", "application/vnd.fsc.weblaunch"},
			{".fst", "image/vnd.fst"},
			{".ftc", "application/vnd.fluxtime.clip"},
			{".fti", "application/vnd.anser-web-funds-transfer-initiation"},
			{".fts", "image/fits"},
			{".fvt", "video/vnd.fvt"},
			{".fzs", "application/vnd.fuzzysheet"},
			{".g2w", "application/vnd.geoplan"},
			{".g3w", "application/vnd.geospace"},
			{".gac", "application/vnd.groove-account"},
			{".gdl", "model/vnd.gdl"},
			{".geo", "application/vnd.dynageo"},
			{".gex", "application/vnd.geometry-explorer"},
			{".ggb", "application/vnd.geogebra.file"},
			{".ggt", "application/vnd.geogebra.tool"},
			{".ghf", "application/vnd.groove-help"},
			{".gif", "image/gif"},
			{".gim", "application/vnd.groove-identity-message"},
			{".gph", "application/vnd.FloGraphIt"},
			{".gqf", "application/vnd.grafeq"},
			{".gqs", "application/vnd.grafeq"},
			{".gram", "application/srgs"},
			{".gre", "application/vnd.geometry-explorer"},
			{".grv", "application/vnd.groove-injector"},
			{".grxml", "application/srgs+xml"},
			{".gsm", "model/vnd.gdl"},
			{".gtar", "application/x-gtar"},
			{".gtm", "application/vnd.groove-tool-message"},
			{".gtw", "model/vnd.gtw"},
			{".gv", "text/vnd.graphviz"},
			{".gxt", "application/vnd.geonext"},
			{".gz", "application/x-gzip"},
			{".h", "text/plain"},
			{".hbc", "application/vnd.hbci"},
			{".hbci", "application/vnd.hbci"},
			{".hdf", "application/x-hdf"},
			{".hdr", "image/vnd.radiance"},
			{".hh", "text/plain"},
			{".hpgl", "application/vnd.hp-HPGL"},
			{".hpi", "application/vnd.hp-hpid"},
			{".hpid", "application/vnd.hp-hpid"},
			{".hps", "application/vnd.hp-hps"},
			{".hqx", "application/mac-binhex40"},
			{".htke", "application/vnd.kenameaapp"},
			{".htm", "text/html"},
			{".html", "text/html"},
			{".hvd", "application/vnd.yamaha.hv-dic"},
			{".hvp", "application/vnd.yamaha.hv-voice"},
			{".hvs", "application/vnd.yamaha.hv-script"},
			{".hxx", "text/plain"},
			{".ic0", "application/vnd.commerce-battelle"},
			{".ic1", "application/vnd.commerce-battelle"},
			{".ic2", "application/vnd.commerce-battelle"},
			{".ic3", "application/vnd.commerce-battelle"},
			{".ic4", "application/vnd.commerce-battelle"},
			{".ic5", "application/vnd.commerce-battelle"},
			{".ic6", "application/vnd.commerce-battelle"},
			{".ic7", "application/vnd.commerce-battelle"},
			{".ic8", "application/vnd.commerce-battelle"},
			{".ica", "application/vnd.commerce-battelle"},
			{".icc", "application/vnd.iccprofile"},
			{".icd", "application/vnd.commerce-battelle"},
			{".ice", "x-conference/x-cooltalk"},
			{".icf", "application/vnd.commerce-battelle"},
			{".icm", "application/vnd.iccprofile"},
			{".ico", "image/vnd.microsoft.icon"},
			{".ics", "text/calendar"},
			{".ief", "image/ief"},
			{".ifb", "text/calendar"},
			{".ifm", "application/vnd.shana.informed.formdata"},
			{".iges", "model/iges"},
			{".igl", "application/vnd.igloader"},
			{".igs", "model/iges"},
			{".igx", "application/vnd.micrografx.igx"},
			{".iif", "application/vnd.shana.informed.interchange"},
			{".img", "application/octet-stream"},
			{".imp", "application/vnd.accpac.simply.imp"},
			{".ims", "application/vnd.ms-ims"},
			{".ipfix", "application/ipfix"},
			{".ipk", "application/vnd.shana.informed.package"},
			{".irm", "application/vnd.ibm.rights-management"},
			{".irp", "application/vnd.irepository.package+xml"},
			{".ism", "model/vnd.gdl"},
			{".iso", "application/octet-stream"},
			{".itp", "application/vnd.shana.informed.formtemplate"},
			{".ivp", "application/vnd.immervision-ivp"},
			{".ivu", "application/vnd.immervision-ivu"},
			{".jad", "text/vnd.sun.j2me.app-descriptor"},
			{".jam", "application/vnd.jam"},
			{".jar", "application/x-java-archive"},
			{".jfif", "image/jpeg"},
			{".jisp", "application/vnd.jisp"},
			{".jlt", "application/vnd.hp-jlyt"},
			{".jnlp", "application/x-java-jnlp-file"},
			{".joda", "application/vnd.joost.joda-archive"},
			{".jp2", "image/jp2"},
			{".jpe", "image/jpeg"},
			{".jpeg", "image/jpeg"},
			{".jpf", "image/jpx"},
			{".jpg", "image/jpeg"},
			{".jpg2", "image/jp2"},
			{".jpgm", "image/jpm"},
			{".jpm", "image/jpm"},
			{".jpx", "image/jpx"},
			{".js", "text/javascript"},
			{".json", "application/json"},
			{".jtd", "text/vnd.esmertec.theme-descriptor"},
			{".kar", "audio/midi"},
			{".karbon", "application/vnd.kde.karbon"},
			{".kcm", "application/vnd.nervana"},
			{".kfo", "application/vnd.kde.kformula"},
			{".kia", "application/vnd.kidspiration"},
			{".kil", "application/x-killustrator"},
			{".kml", "application/vnd.google-earth.kml+xml"},
			{".kmz", "application/vnd.google-earth.kmz"},
			{".kne", "application/vnd.Kinar"},
			{".knp", "application/vnd.Kinar"},
			{".kom", "application/vnd.hbci"},
			{".kon", "application/vnd.kde.kontour"},
			{".koz", "audio/vnd.audikoz"},
			{".kpr", "application/vnd.kde.kpresenter"},
			{".kpt", "application/vnd.kde.kpresenter"},
			{".ksp", "application/vnd.kde.kspread"},
			{".ktr", "application/vnd.kahootz"},
			{".ktz", "application/vnd.kahootz"},
			{".kwd", "application/vnd.kde.kword"},
			{".kwt", "application/vnd.kde.kword"},
			{".l16", "audio/L16"},
			{".latex", "application/x-latex"},
			{".lbc", "audio/iLBC"},
			{".lbd", "application/vnd.llamagraphics.life-balance.desktop"},
			{".lbe", "application/vnd.llamagraphics.life-balance.exchange+xml"},
			{".les", "application/vnd.hhe.lesson-player"},
			{".lha", "application/octet-stream"},
			{".link66", "application/vnd.route66.link66+xml"},
			{".list3820", "application/vnd.ibm.modcap"},
			{".listafp", "application/vnd.ibm.modcap"},
			{".lmp", "model/vnd.gdl"},
			{".lostxml", "application/lost+xml"},
			{".lrm", "application/vnd.ms-lrm"},
			{".ltf", "application/vnd.frogans.ltf"},
			{".lvp", "audio/vnd.lucent.voice"},
			{".lwp", "application/vnd.lotus-wordpro"},
			{".lzh", "application/octet-stream"},
			{".m", "application/vnd.wolfram.mathematica.package"},
			{".m15", "audio/x-mod"},
			{".m21", "application/mp21"},
			{".m3u", "audio/x-mpegurl"},
			{".m3u8", "application/vnd.apple.mpegurl"},
			{".m4u", "video/vnd.mpegurl"},
			{".ma", "application/mathematica"},
			{".mag", "application/vnd.ecowin.chart"},
			{".mail", "message/rfc822"},
			{".man", "application/x-troff-man"},
			{".manifest", "application/x-ms-manifest"},
			{".mb", "application/mathematica"},
			{".mbk", "application/vnd.Mobius.MBK"},
			{".mbox", "application/mbox"},
			{".mc1", "application/vnd.medcalcdata"},
			{".mcd", "application/vnd.mcd"},
			{".mdc", "application/vnd.marlin.drm.mdcf"},
			{".mdi", "image/vnd.ms-modi"},
			{".me", "application/x-troff-me"},
			{".med", "audio/x-mod"},
			{".mesh", "model/mesh"},
			{".metalink", "application/metalink+xml"},
			{".mfm", "application/vnd.mfmp"},
			{".mgz", "application/vnd.proteus.magazine"},
			{".mid", "audio/midi"},
			{".midi", "audio/midi"},
			{".mif", "application/vnd.mif"},
			{".mj2", "video/mj2"},
			{".mjp2", "video/mj2"},
			{".mlp", "audio/vnd.dolby.mlp"},
			{".mmd", "application/vnd.chipnuts.karaoke-mmd"},
			{".mmf", "application/vnd.smaf"},
			{".mml", "application/mathml+xml"},
			{".mmr", "image/vnd.fujixerox.edmics-mmr"},
			{".mms", "application/vnd.wap.mms-message"},
			{".mod", "audio/x-mod"},
			{".model-inter", "application/vnd.vd-study"},
			{".moml", "model/vnd.moml+xml"},
			{".mov", "video/quicktime"},
			{".movie", "video/x-sgi-movie"},
			{".mp1", "audio/mpeg"},
			{".mp2", "audio/mpeg"},
			{".mp21", "application/mp21"},
			{".mp3", "audio/mpeg"},
			{".mp4", "video/mp4"},
			{".mpc", "application/vnd.mophun.certificate"},
			{".mpe", "video/mpeg"},
			{".mpeg", "video/mpeg"},
			{".mpf", "text/vnd.ms-mediapackage"},
			{".mpg", "video/mpeg"},
			{".mpg4", "video/mp4"},
			{".mpga", "audio/mpeg"},
			{".mpkg", "application/vnd.apple.installer+xml"},
			{".mpm", "application/vnd.blueice.multipass"},
			{".mpn", "application/vnd.mophun.application"},
			{".mpp", "application/vnd.ms-project"},
			{".mpy", "application/vnd.ibm.MiniPay"},
			{".mqy", "application/vnd.Mobius.MQY"},
			{".mrc", "application/marc"},
			{".ms", "application/x-troff-ms"},
			{".msd", "application/vnd.fdsn.mseed"},
			{".mseed", "application/vnd.fdsn.mseed"},
			{".mseq", "application/vnd.mseq"},
			{".msf", "application/vnd.epson.msf"},
			{".msh", "model/mesh"},
			{".msl", "application/vnd.Mobius.MSL"},
			{".msm", "model/vnd.gdl"},
			{".msty", "application/vnd.muvee.style"},
			{".mtm", "audio/x-mod"},
			{".mts", "model/vnd.mts"},
			{".mus", "application/vnd.musician"},
			{".mwc", "application/vnd.dpgraph"},
			{".mwf", "application/vnd.MFER"},
			{".mxf", "application/mxf"},
			{".mxi", "application/vnd.vd-study"},
			{".mxl", "application/vnd.recordare.musicxml"},
			{".mxmf", "audio/mobile-xmf"},
			{".mxml", "application/xv+xml"},
			{".mxs", "application/vnd.triscape.mxs"},
			{".mxu", "video/vnd.mpegurl"},
			{".n-gage", "application/vnd.nokia.n-gage.symbian.install"},
			{".nb", "application/mathematica"},
			{".nbp", "application/vnd.wolfram.player"},
			{".nc", "application/x-netcdf"},
			{".ndc", "application/vnd.osa.netdeploy"},
			{".ndl", "application/vnd.lotus-notes"},
			{".ngdat", "application/vnd.nokia.n-gage.data"},
			{".nim", "video/vnd.nokia.interleaved-multimedia"},
			{".nlu", "application/vnd.neurolanguage.nlu"},
			{".nml", "application/vnd.enliven"},
			{".nnd", "application/vnd.noblenet-directory"},
			{".nns", "application/vnd.noblenet-sealer"},
			{".nnw", "application/vnd.noblenet-web"},
			{".ns2", "application/vnd.lotus-notes"},
			{".ns3", "application/vnd.lotus-notes"},
			{".ns4", "application/vnd.lotus-notes"},
			{".nsf", "application/vnd.lotus-notes"},
			{".nsg", "application/vnd.lotus-notes"},
			{".nsh", "application/vnd.lotus-notes"},
			{".ntf", "application/vnd.lotus-notes"},
			{".o4a", "application/vnd.oma.drm.dcf"},
			{".o4v", "application/vnd.oma.drm.dcf"},
			{".oa2", "application/vnd.fujitsu.oasys2"},
			{".oa3", "application/vnd.fujitsu.oasys3"},
			{".oas", "application/vnd.fujitsu.oasys"},
			{".oda", "application/oda"},
			{".odb", "application/vnd.oasis.opendocument.database"},
			{".odc", "application/vnd.oasis.opendocument.chart"},
			{".odf", "application/vnd.oasis.opendocument.formula"},
			{".odg", "application/vnd.oasis.opendocument.graphics"},
			{".odi", "application/vnd.oasis.opendocument.image"},
			{".odm", "application/vnd.oasis.opendocument.text-master"},
			{".odp", "application/vnd.oasis.opendocument.presentation"},
			{".ods", "application/vnd.oasis.opendocument.spreadsheet"},
			{".odt", "application/vnd.oasis.opendocument.text"},
			{".oga", "audio/ogg"},
			{".ogg", "audio/ogg"},
			{".ogv", "video/ogg"},
			{".ogx", "application/ogg"},
			{".omg", "audio/ATRAC3"},
			{".opf", "application/oebps-package+xml"},
			{".oprc", "application/vnd.palm"},
			{".or2", "application/vnd.lotus-organizer"},
			{".or3", "application/vnd.lotus-organizer"},
			{".org", "application/vnd.lotus-organizer"},
			{".orq", "application/ocsp-request"},
			{".ors", "application/ocsp-response"},
			{".osf", "application/vnd.yamaha.openscoreformat"},
			{".otc", "application/vnd.oasis.opendocument.chart-template"},
			{".otf", "application/vnd.oasis.opendocument.formula-template"},
			{".otg", "application/vnd.oasis.opendocument.graphics-template"},
			{".oth", "application/vnd.oasis.opendocument.text-web"},
			{".oti", "application/vnd.oasis.opendocument.image-template"},
			{".otp", "application/vnd.oasis.opendocument.presentation-template"},
			{".ots", "application/vnd.oasis.opendocument.spreadsheet-template"},
			{".ott", "application/vnd.oasis.opendocument.text-template"},
			{".oxt", "application/vnd.openofficeorg.extension"},
			{".p10", "application/pkcs10"},
			{".p7c", "application/pkcs7-mime"},
			{".p7m", "application/pkcs7-mime"},
			{".p7s", "application/pkcs7-signature"},
			{".pack", "application/x-java-pack200"},
			{".package", "application/vnd.autopackage"},
			{".pbd", "application/vnd.powerbuilder6"},
			{".pbm", "image/x-portable-bitmap"},
			{".pcl", "application/vnd.hp-PCL"},
			{".pdb", "application/vnd.palm"},
			{".pdf", "application/pdf"},
			{".pfr", "application/font-tdpfr"},
			{".pgb", "image/vnd.globalgraphics.pgb"},
			{".pgm", "image/x-portable-graymap"},
			{".pgn", "application/x-chess-pgn"},
			{".pil", "application/vnd.piaccess.application-license"},
			{".pkd", "application/vnd.hbci"},
			{".pkg", "application/vnd.apple.installer+xml"},
			{".pkipath", "application/pkix-pkipath"},
			{".pl", "application/x-perl"},
			{".plb", "application/vnd.3gpp.pic-bw-large"},
			{".plc", "application/vnd.Mobius.PLC"},
			{".plf", "application/vnd.pocketlearn"},
			{".plj", "audio/vnd.everad.plj"},
			{".pls", "application/pls+xml"},
			{".pm", "text/plain"},
			{".pml", "application/vnd.ctc-posml"},
			{".png", "image/png"},
			{".pnm", "image/x-portable-anymap"},
			{".pod", "text/x-pod"},
			{".portpkg", "application/vnd.macports.portpkg"},
			{".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
			{".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
			{".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
			{".ppd", "application/vnd.cups-ppd"},
			{".ppkg", "application/vnd.xmpie.ppkg"},
			{".ppm", "image/x-portable-pixmap"},
			{".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
			{".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
			{".ppt", "application/vnd.ms-powerpoint"},
			{".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
			{".pqa", "application/vnd.palm"},
			{".prc", "application/vnd.palm"},
			{".pre", "application/vnd.lotus-freelance"},
			{".preminet", "application/vnd.preminet"},
			{".prz", "application/vnd.lotus-freelance"},
			{".ps", "application/postscript"},
			{".psb", "application/vnd.3gpp.pic-bw-small"},
			{".psd", "image/vnd.adobe.photoshop"},
			{".pseg3820", "application/vnd.ibm.modcap"},
			{".psid", "audio/prs.sid"},
			{".pti", "image/prs.pti"},
			{".ptid", "application/vnd.pvi.ptid1"},
			{".pvb", "application/vnd.3gpp.pic-bw-var"},
			{".pwn", "application/vnd.3M.Post-it-Notes"},
			{".pya", "audio/vnd.ms-playready.media.pya"},
			{".pyv", "video/vnd.ms-playready.media.pyv"},
			{".qam", "application/vnd.epson.quickanime"},
			{".qbo", "application/vnd.intu.qbo"},
			{".qca", "application/vnd.ericsson.quickcall"},
			{".qcall", "application/vnd.ericsson.quickcall"},
			{".qcp", "audio/qcelp"},
			{".qfx", "application/vnd.intu.qfx"},
			{".qps", "application/vnd.publishare-delta-tree"},
			{".qt", "video/quicktime"},
			{".qwd", "application/vnd.Quark.QuarkXPress"},
			{".qwt", "application/vnd.Quark.QuarkXPress"},
			{".qxb", "application/vnd.Quark.QuarkXPress"},
			{".qxd", "application/vnd.Quark.QuarkXPress"},
			{".qxl", "application/vnd.Quark.QuarkXPress"},
			{".qxt", "application/vnd.Quark.QuarkXPress"},
			{".ra", "audio/x-realaudio"},
			{".ram", "audio/x-pn-realaudio"},
			{".ras", "image/x-cmu-raster"},
			{".rcprofile", "application/vnd.ipunplugged.rcprofile"},
			{".rct", "application/prs.nprend"},
			{".rdf", "application/rdf+xml"},
			{".rdz", "application/vnd.data-vision.rdz"},
			{".rep", "application/vnd.businessobjects"},
			{".request", "application/vnd.nervana"},
			{".rgb", "image/x-rgb"},
			{".rgbe", "image/vnd.radiance"},
			{".rif", "application/reginfo+xml"},
			{".rl", "application/resource-lists+xml"},
			{".rlc", "image/vnd.fujixerox.edmics-rlc"},
			{".rld", "application/resource-lists-diff+xml"},
			{".rm", "audio/x-pn-realaudio"},
			{".rms", "application/vnd.jcp.javame.midlet-rms"},
			{".rnc", "application/relax-ng-compact-syntax"},
			{".rnd", "application/prs.nprend"},
			{".roff", "application/x-troff"},
			{".rp9", "application/vnd.cloanto.rp9"},
			{".rpm", "application/x-rpm"},
			{".rpss", "application/vnd.nokia.radio-presets"},
			{".rpst", "application/vnd.nokia.radio-preset"},
			{".rq", "application/sparql-query"},
			{".rs", "application/rls-services+xml"},
			{".rsm", "model/vnd.gdl"},
			{".rss", "application/rss+xml"},
			{".rst", "text/prs.fallenstein.rst"},
			{".rtf", "application/rtf"},
			{".rtx", "text/richtext"},
			{".s11", "video/vnd.sealed.mpeg1"},
			{".s14", "video/vnd.sealed.mpeg4"},
			{".s1a", "application/vnd.sealedmedia.softseal.pdf"},
			{".s1e", "application/vnd.sealed.xls"},
			{".s1g", "image/vnd.sealedmedia.softseal.gif"},
			{".s1h", "application/vnd.sealedmedia.softseal.html"},
			{".s1j", "image/vnd.sealedmedia.softseal.jpg"},
			{".s1m", "audio/vnd.sealedmedia.softseal.mpeg"},
			{".s1n", "image/vnd.sealed.png"},
			{".s1p", "application/vnd.sealed.ppt"},
			{".s1q", "video/vnd.sealedmedia.softseal.mov"},
			{".s1w", "application/vnd.sealed.doc"},
			{".s3df", "application/vnd.sealed.3df"},
			{".s3m", "audio/x-s3m"},
			{".saf", "application/vnd.yamaha.smaf-audio"},
			{".sam", "application/vnd.lotus-wordpro"},
			{".sc", "application/vnd.ibm.secure-container"},
			{".scd", "application/vnd.scribus"},
			{".scm", "application/vnd.lotus-screencam"},
			{".scq", "application/scvp-cv-request"},
			{".scs", "application/scvp-cv-response"},
			{".scsf", "application/vnd.sealed.csf"},
			{".sdf", "application/vnd.Kinar"},
			{".sdkd", "application/vnd.solent.sdkm+xml"},
			{".sdkm", "application/vnd.solent.sdkm+xml"},
			{".sdo", "application/vnd.sealed.doc"},
			{".sdoc", "application/vnd.sealed.doc"},
			{".sdp", "application/sdp"},
			{".see", "application/vnd.seemail"},
			{".seed", "application/vnd.fsdn.seed"},
			{".sem", "application/vnd.sealed.eml"},
			{".sema", "application/vnd.sema"},
			{".semd", "application/vnd.semd"},
			{".semf", "application/vnd.semf"},
			{".seml", "application/vnd.sealed.eml"},
			{".sfd", "application/vnd.font-fontforge-sfd"},
			{".sfd-hdstx", "application/vnd.hydrostatix.sof-data"},
			{".sfs", "application/vnd.spotfire.sfs"},
			{".sgi", "image/vnd.sealedmedia.softseal.gif"},
			{".sgif", "image/vnd.sealedmedia.softseal.gif"},
			{".sgm", "text/sgml"},
			{".sgml", "text/sgml"},
			{".sh", "application/x-sh"},
			{".shar", "application/x-shar"},
			{".shf", "application/shf+xml"},
			{".si", "text/vnd.wap.si"},
			{".sic", "application/vnd.wap.sic"},
			{".sid", "audio/prs.sid"},
			{".sieve", "application/sieve"},
			{".sig", "application/pgp-signature"},
			{".silo", "model/mesh"},
			{".sis", "application/vnd.symbian.install"},
			{".sisx", "x-epoc/x-sisx-app"},
			{".sit", "application/x-stuffit"},
			{".siv", "application/sieve"},
			{".sjp", "image/vnd.sealedmedia.softseal.jpg"},
			{".sjpg", "image/vnd.sealedmedia.softseal.jpg"},
			{".skd", "application/vnd.koan"},
			{".skm", "application/vnd.koan"},
			{".skp", "application/vnd.koan"},
			{".skt", "application/vnd.koan"},
			{".sl", "text/vnd.wap.sl"},
			{".sla", "application/vnd.scribus"},
			{".slaz", "application/vnd.scribus"},
			{".slc", "application/vnd.wap.slc"},
			{".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
			{".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
			{".slt", "application/vnd.epson.salt"},
			{".smh", "application/vnd.sealed.mht"},
			{".smht", "application/vnd.sealed.mht"},
			{".smi", "application/smil"},
			{".smil", "application/smil"},
			{".sml", "application/smil"},
			{".smo", "video/vnd.sealedmedia.softseal.mov"},
			{".smov", "video/vnd.sealedmedia.softseal.mov"},
			{".smp", "audio/vnd.sealedmedia.softseal.mpeg"},
			{".smp3", "audio/vnd.sealedmedia.softseal.mpeg"},
			{".smpg", "video/vnd.sealed.mpeg1"},
			{".sms", "application/vnd.3gpp2.sms"},
			{".smv", "audio/SMV"},
			{".snd", "audio/basic"},
			{".so", "application/octet-stream"},
			{".soa", "text/dns"},
			{".soc", "application/sgml-open-catalog"},
			{".spd", "application/vnd.sealedmedia.softseal.pdf"},
			{".spdf", "application/vnd.sealedmedia.softseal.pdf"},
			{".spf", "application/vnd.yamaha.smaf-phrase"},
			{".spl", "application/x-futuresplash"},
			{".spn", "image/vnd.sealed.png"},
			{".spng", "image/vnd.sealed.png"},
			{".spo", "text/vnd.in3d.spot"},
			{".spot", "text/vnd.in3d.spot"},
			{".spp", "application/scvp-vp-response"},
			{".sppt", "application/vnd.sealed.ppt"},
			{".spq", "application/scvp-vp-request"},
			{".spx", "audio/ogg"},
			{".src", "application/x-wais-source"},
			{".srx", "application/sparql-results+xml"},
			{".sse", "application/vnd.kodak-descriptor"},
			{".ssf", "application/vnd.epson.ssf"},
			{".ssml", "application/ssml+xml"},
			{".ssw", "video/vnd.sealed.swf"},
			{".sswf", "video/vnd.sealed.swf"},
			{".st", "application/vnd.sailingtracker.track"},
			{".stc", "application/vnd.sun.xml.calc.template"},
			{".std", "application/vnd.sun.xml.draw.template"},
			{".stf", "application/vnd.wt.stf"},
			{".sti", "application/vnd.sun.xml.impress.template"},
			{".stif", "application/vnd.sealed.tiff"},
			{".stk", "application/hyperstudio"},
			{".stm", "audio/x-stm"},
			{".stml", "application/vnd.sealedmedia.softseal.html"},
			{".study-inter", "application/vnd.vd-study"},
			{".stw", "application/vnd.sun.xml.writer.template"},
			{".sus", "application/vnd.sus-calendar"},
			{".susp", "application/vnd.sus-calendar"},
			{".sv4cpio", "application/x-sv4cpio"},
			{".sv4crc", "application/x-sv4crc"},
			{".svg", "image/svg+xml"},
			{".svgz", "image/svg+xml"},
			{".swf", "application/x-shockwave-flash"},
			{".swi", "application/vnd.aristanetworks.swi"},
			{".sxc", "application/vnd.sun.xml.calc"},
			{".sxd", "application/vnd.sun.xml.draw"},
			{".sxg", "application/vnd.sun.xml.writer.global"},
			{".sxi", "application/vnd.sun.xml.impress"},
			{".sxl", "application/vnd.sealed.xls"},
			{".sxls", "application/vnd.sealed.xls"},
			{".sxm", "application/vnd.sun.xml.math"},
			{".sxw", "application/vnd.sun.xml.writer"},
			{".t", "application/x-troff"},
			{".t38", "image/t38"},
			{".tag", "text/prs.lines.tag"},
			{".tao", "application/vnd.tao.intent-module-archive"},
			{".tar", "application/x-tar"},
			{".tcap", "application/vnd.3gpp2.tcap"},
			{".tcl", "application/x-tcl"},
			{".teacher", "application/vnd.smart.teacher"},
			{".tex", "application/x-tex"},
			{".texi", "application/x-texinfo"},
			{".texinfo", "application/x-texinfo"},
			{".text", "text/plain"},
			{".tfx", "image/tiff-fx"},
			{".tga", "image/x-targa"},
			{".tgz", "application/x-gzip"},
			{".tif", "image/tiff"},
			{".tiff", "image/tiff"},
			{".tlclient", "application/vnd.cendio.thinlinc.clientconf"},
			{".tmo", "application/vnd.tmobile-livetv"},
			{".tnef", "application/vnd.ms-tnef"},
			{".tnf", "application/vnd.ms-tnef"},
			{".torrent", "application/x-bittorrent"},
			{".tpl", "application/vnd.groove-tool-template"},
			{".tpt", "application/vnd.trid.tpt"},
			{".tr", "application/x-troff"},
			{".tra", "application/vnd.trueapp"},
			{".ts", "text/vnd.trolltech.linguist"},
			{".tsq", "application/timestamp-query"},
			{".tsr", "application/timestamp-reply"},
			{".tsv", "text/tab-separated-values"},
			{".twd", "application/vnd.SimTech-MindMapper"},
			{".twds", "application/vnd.SimTech-MindMapper"},
			{".txd", "application/vnd.genomatix.tuxedo"},
			{".txf", "application/vnd.Mobius.TXF"},
			{".txt", "text/plain"},
			{".u8dsn", "message/global-delivery-status"},
			{".u8hdr", "message/global-headers"},
			{".u8mdn", "message/global-disposition-notification"},
			{".u8msg", "message/global"},
			{".ufd", "application/vnd.ufdl"},
			{".ufdl", "application/vnd.ufdl"},
			{".ult", "audio/x-mod"},
			{".umj", "application/vnd.umajin"},
			{".uni", "audio/x-mod"},
			{".unityweb", "application/vnd.unity"},
			{".uo", "application/vnd.uoml+xml"},
			{".uoml", "application/vnd.uoml+xml"},
			{".upa", "application/vnd.hbci"},
			{".uri", "text/uri-list"},
			{".uric", "text/vnd.si.uricatalogue"},
			{".uris", "text/uri-list"},
			{".ustar", "application/x-ustar"},
			{".utz", "application/vnd.uiq.theme"},
			{".vbk", "audio/vnd.nortel.vbk"},
			{".vbox", "application/vnd.previewsystems.box"},
			{".vcd", "application/x-cdlink"},
			{".vcf", "text/x-vcard"},
			{".vcg", "application/vnd.groove-vcard"},
			{".vcx", "application/vnd.vcx"},
			{".vew", "application/vnd.lotus-approach"},
			{".vis", "application/vnd.visionary"},
			{".vpm", "multipart/voice-message"},
			{".vrml", "model/vrml"},
			{".vsc", "application/vnd.vidsoft.vidconference"},
			{".vsd", "application/vnd.visio"},
			{".vsf", "application/vnd.vsf"},
			{".vss", "application/vnd.visio"},
			{".vst", "application/vnd.visio"},
			{".vsw", "application/vnd.visio"},
			{".vtu", "model/vnd.vtu"},
			{".vwx", "application/vnd.vectorworks"},
			{".vxml", "application/voicexml+xml"},
			{".wadl", "application/vnd.sun.wadl+xml"},
			{".wav", "audio/x-wav"},
			{".wax", "audio/x-ms-wax"},
			{".wbmp", "image/vnd.wap.wbmp"},
			{".wbs", "application/vnd.criticaltools.wbs+xml"},
			{".wbxml", "application/vnd.wap.wbxml"},
			{".wcm", "application/vnd.ms-works"},
			{".wdb", "application/vnd.ms-works"},
			{".webm", "video/webm"},
			{".wif", "application/watcherinfo+xml"},
			{".win", "model/vnd.gdl"},
			{".wk1", "application/vnd.lotus-1-2-3"},
			{".wk3", "application/vnd.lotus-1-2-3"},
			{".wk4", "application/vnd.lotus-1-2-3"},
			{".wks", "application/vnd.ms-works"},
			{".wm", "video/x-ms-wm"},
			{".wma", "audio/x-ms-wma"},
			{".wmc", "application/vnd.wmc"},
			{".wml", "text/vnd.wap.wml"},
			{".wmlc", "application/vnd.wap.wmlc"},
			{".wmls", "text/vnd.wap.wmlscript"},
			{".wmlsc", "application/vnd.wap.wmlscriptc"},
			{".wmv", "video/x-ms-wmv"},
			{".wmx", "video/x-ms-wmx"},
			{".wpd", "application/vnd.wordperfect"},
			{".wpl", "application/vnd.ms-wpl"},
			{".wps", "application/vnd.ms-works"},
			{".wqd", "application/vnd.wqd"},
			{".wrl", "model/vrml"},
			{".wsc", "application/vnd.wfa.wsc"},
			{".wsdl", "application/wsdl+xml"},
			{".wspolicy", "application/wspolicy+xml"},
			{".wtb", "application/vnd.webturbo"},
			{".wv", "application/vnd.wv.csp+wbxml"},
			{".wvx", "video/x-ms-wvx"},
			{".x3d", "application/vnd.hzn-3d-crossword"},
			{".x_b", "model/vnd.parasolid.transmit.binary"},
			{".x_t", "model/vnd.parasolid.transmit.text"},
			{".xar", "application/vnd.xara"},
			{".xav", "application/xcap-att+xml"},
			{".xbd", "application/vnd.fujixerox.docuworks.binder"},
			{".xbm", "image/x-xbitmap"},
			{".xca", "application/xcap-caps+xml"},
			{".xdm", "application/vnd.syncml.dm+xml"},
			{".xdp", "application/vnd.adobe.xdp+xml"},
			{".xdssc", "application/dssc+xml"},
			{".xdw", "application/vnd.fujixerox.docuworks"},
			{".xel", "application/xcap-el+xml"},
			{".xer", "application/xcap-error+xml"},
			{".xfd", "application/vnd.xfdl"},
			{".xfdf", "application/vnd.adobe.xfdf"},
			{".xfdl", "application/vnd.xfdl"},
			{".xht", "application/xhtml+xml"},
			{".xhtm", "application/xhtml+xml"},
			{".xhtml", "application/xhtml+xml"},
			{".xhvml", "application/xv+xml"},
			{".xif", "image/vnd.xiff"},
			{".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
			{".xlim", "application/vnd.xmpie.xlim"},
			{".xls", "application/vnd.ms-excel"},
			{".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
			{".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
			{".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
			{".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
			{".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
			{".xml", "text/xml"},
			{".xmt_bin", "model/vnd.parasolid.transmit.binary"},
			{".xmt_txt", "model/vnd.parasolid.transmit.text"},
			{".xns", "application/xcap-ns+xml"},
			{".xo", "application/vnd.olpc-sugar"},
			{".xop", "application/xop+xml"},
			{".xpm", "image/x-xpixmap"},
			{".xpr", "application/vnd.is-xpr"},
			{".xps", "application/vnd.ms-xpsdocument"},
			{".xpw", "application/vnd.intercon.formnet"},
			{".xpx", "application/vnd.intercon.formnet"},
			{".xsl", "application/xslt+xml"},
			{".xslt", "application/xslt+xml"},
			{".xsm", "application/vnd.syncml+xml"},
			{".xul", "application/vnd.mozilla.xul+xml"},
			{".xvm", "application/xv+xml"},
			{".xvml", "application/xv+xml"},
			{".xwd", "image/x-xwindowdump"},
			{".xyz", "chemical/x-xyz"},
			{".xyze", "image/vnd.radiance"},
			{".xz", "application/x-xz"},
			{".zaz", "application/vnd.zzazz.deck+xml"},
			{".zfo", "application/vnd.software602.filler.form-xml-zip"},
			{".zip", "application/zip"},
			{".zir", "application/vnd.zul"},
			{".zirz", "application/vnd.zul"},
			{".zmm", "application/vnd.HandHeld-Entertainment+xml"},
			{".zone", "text/dns"}
		};

		/// <summary>
		/// Create a FileSender for a file
		/// </summary>
		public FileSender(string filename) {
			Filename = filename;
			CacheAllowed = true;
		}

		/// <summary>
		/// Default behaviour is to return the file contents, processing .tmpl and .md files as appropriate.
		/// </summary>
		public override void Default() {
			Title = "";
			if (Filename.IndexOf("..") >= 0) throw new FileNotFoundException("Illegal path " + Filename);
			FileInfo file = Server.FileInfo(Filename);
			if (!file.Exists && Path.GetExtension(Filename) == ".html") {
				file = Server.FileInfo(Path.ChangeExtension(Filename, ".tmpl"));
				if (file.Exists) {
					FileInfo def = Server.FileInfo("default.tmpl");
					if (def.LastWriteTimeUtc < file.LastWriteTimeUtc)
						file = def;
				}
			}
			if (!file.Exists) {
				WriteResponse("", "text/plain", HttpStatusCode.NotFound);
				return;
			}
			string ifModifiedSince = Request.Headers["If-Modified-Since"];
			if (!string.IsNullOrEmpty(ifModifiedSince)) {
				try {
					DateTime modifiedSince = DateTime.Parse(ifModifiedSince.Split(';')[0]);
					if (modifiedSince >= file.LastWriteTimeUtc) {
						WriteResponse("", "text/plain", HttpStatusCode.NotModified);
						return;
					}
				} catch {
				}
			}
			Response.AddHeader("Last-Modified", file.LastWriteTimeUtc.ToString("r"));
			string contentType;
			string ext = file.Extension.ToLower();
			switch (ext) {
				case ".tmpl":
					WriteResponse(Template(Path.ChangeExtension(Filename, ".tmpl"), this), "text/html", HttpStatusCode.OK);
					return;
				case ".md":
					contentType = "text/html";
					using (StreamReader r = new StreamReader(file.FullName)) {
						string s = r.ReadToEnd();
						s = Markdown.ToHtml(s, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());
						WriteResponse(s, contentType, HttpStatusCode.OK);
					}
					return;
				default:
					// Try our own list first
					if (!ContentTypes.TryGetValue(ext, out contentType)) {
						// Try the o/s list
						contentType = MimeMapping.GetMimeMapping(file.Name);
					}
					break;
			}
			using (Stream i = new FileStream(file.FullName, FileMode.Open, FileAccess.Read)) {
				WriteResponse(i, contentType, HttpStatusCode.OK);
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
