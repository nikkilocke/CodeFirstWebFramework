using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CodeFirstWebFramework {
	/// <summary>
	/// Web Server - listens for connections, and services them
	/// </summary>
	public class WebServer {
		HttpListener _listener;
		bool _running;
		Dictionary<string, Session> _sessions;
		static object _lock = new object();
		Dictionary<string, Namespace> webmodules;      // All the different web modules we are running
		HashSet<string> loadedAssemblies;

		/// <summary>
		/// Constructor.
		/// You must call Config.Load before calling this.
		/// Sets up all servers specified in the config file, loading any additional assemblies required.
		/// Upgrades all databases to match the latest code.
		/// </summary>
		public WebServer() {
			try {
				AppVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
				VersionSuffix = "-v" + AppVersion;
				webmodules = new Dictionary<string, Namespace>();
				loadedAssemblies = new HashSet<string>();
				var baseType = typeof(AppModule);
				HashSet<string> databases = new HashSet<string>();
				foreach (ServerConfig server in Config.Default.Servers) {
					registerServer(databases, server);
				}
				registerServer(databases, Config.Default.DefaultServer);
			} catch (Exception ex) {
				Log.Error.WriteLine(ex.ToString());
				throw;
			}
		}

		/// <summary>
		/// Add namespace to modules list, and upgrade database, if not done already.
		/// </summary>
		/// <param name="databases">HashSet of databases already upgraded</param>
		/// <param name="server">ServerConfig to register</param>
		void registerServer(HashSet<string> databases, ServerConfig server) {
			if (webmodules.ContainsKey(server.Namespace)) {
				server.NamespaceDef = webmodules[server.Namespace];
			} else {
				foreach(string assembly in server.AdditionalAssemblies) {
					if (loadedAssemblies.Contains(assembly))
						continue;
					Assembly.Load(assembly);
					loadedAssemblies.Add(assembly);
				}
				server.NamespaceDef = Namespace.Create(server);
				webmodules[server.Namespace] = server.NamespaceDef;
			}
			using (Database db = server.NamespaceDef.GetDatabase(server)) {
				if (!databases.Contains(db.UniqueIdentifier)) {
					databases.Add(db.UniqueIdentifier);
					db.Upgrade();
				}
			}
		}

		/// <summary>
		/// The version number from the application.
		/// </summary>
		static public string AppVersion;

		/// <summary>
		/// Version suffix for including in url's to defeat long-term caching of (e.g.) javascript and css files
		/// </summary>
		static public string VersionSuffix;

		/// <summary>
		/// Start WebServer listening for connections
		/// </summary>
		public void Start() {
			try {
				_listener = new HttpListener();
				HashSet<int> ports = new HashSet<int>();
				ports.Add(Config.Default.Port);
				foreach (ServerConfig server in Config.Default.Servers) {
					if (server.Port > 0)
						ports.Add(server.Port);
				}
				foreach (int port in ports) {
					_listener.Prefixes.Add("http://+:" + port + "/");
					Log.Startup.WriteLine("Listening on port {0}", port);
				}
				_sessions = new Dictionary<string, Session>();
				// Start thread to expire sessions after 30 mins of inactivity
				new Task(delegate () {
					for (;;) {
						Thread.Sleep(Config.Default.SessionExpiryMinutes * 1000);
						DateTime now = Utils.Now;
						lock (_sessions) {
							foreach (string key in _sessions.Keys.ToList()) {
								Session s = _sessions[key];
								if (s.Expires < now) {
									s.Dispose();
									_sessions.Remove(key);
								}
							}
						}
					}
				}).Start();
				_running = true;
				_listener.Start();
				while (_running) {
					try {
						HttpListenerContext request = _listener.GetContext();
						ThreadPool.QueueUserWorkItem(ProcessRequest, request);
					} catch {
					}
				}
			} catch (HttpListenerException ex) {
				Log.Error.WriteLine(ex.ToString());
			} catch (ThreadAbortException) {
			} catch (Exception ex) {
				Log.Error.WriteLine(ex.ToString());
			}
		}

		/// <summary>
		/// Stop the server
		/// </summary>
		public void Stop() {
			_running = false;
			_listener.Stop();
		}

		/// <summary>
		/// All Active Sessions
		/// </summary>
		public IEnumerable<Session> Sessions {
			get {
				return _sessions.Values;
			}
		}

		/// <summary>
		/// Process a single request
		/// </summary>
		/// <param name="listenerContext"></param>
		void ProcessRequest(object listenerContext) {
			DateTime started = DateTime.Now;			// For timing response
			HttpListenerContext context = null;
			AppModule module = null;
			StringBuilder log = new StringBuilder();	// Session log writes to here, and it is displayed at the end
			context = (HttpListenerContext)listenerContext;
			ServerConfig server = Config.Default.SettingsForHost(context.Request.Url);
			log.AppendFormat("{0} {1}:{2}:[ms]:",
				context.Request.RemoteEndPoint.Address,
				context.Request.Headers["X-Forwarded-For"],
				context.Request.Url.OriginalString);
			if (server == null) {
				// Request not matching any of the Server array, and not on the default port
				context.Response.StatusCode = 404;
				context.Response.ContentType = "text/plain;charset=" + AppModule.Charset;
				byte[] msg = AppModule.Encoding.GetBytes("Server not found");
				context.Response.ContentLength64 = msg.Length;
				log.Append("404 Server not found ");
				using (Stream r = context.Response.OutputStream) {
					r.Write(msg, 0, msg.Length);
				}
			} else {
				Session session = null;
				try {
					ModuleInfo info = webmodules[server.Namespace].ParseUri(context.Request.Url.AbsolutePath, out string filename);
					string moduleName = null;
					string methodName = null;
					// Urls of the form /ModuleName[/MethodName][.html] call a C# AppModule
					if (info != null) {
						// The AppModule exists - does it handle this extension?
						string extension = Path.GetExtension(filename);
						HandlesAttribute handles = info.Type.GetCustomAttribute<HandlesAttribute>(true);
						if (string.IsNullOrWhiteSpace(extension) || handles.Extensions.Contains(extension.ToLower())) {
							module = (AppModule)Activator.CreateInstance(info.Type);
							module.Info = info;
							moduleName = Path.GetDirectoryName(filename);
							methodName = Path.GetFileNameWithoutExtension(filename);
						}
					}
					if (moduleName == null) {
						// No AppModule found - treat url as a file request
						moduleName = "FileSender";
						module = server.NamespaceDef.GetInstanceOf<FileSender>(filename);
					}
					// AppModule found - retrieve or create a session for it
					Cookie cookie = context.Request.Cookies["session"];
					if(cookie == null) {
						string hdr = context.Request.Headers.Get("Authorization");
						if (!string.IsNullOrEmpty(hdr) && hdr.StartsWith("Bearer "))
							cookie = new Cookie("session", hdr.Substring(7).Trim(), "/");
					}
					// Set up module
					module.Server = server;
					module.ActiveModule = webmodules[server.Namespace];
					module.LogString = log;
					if (cookie != null) {
						if (!_sessions.TryGetValue(cookie.Value, out session) && server.PersistentSessions)
							session = Session.FromStore(this, module, cookie.Value);
						Log.Session.WriteLine("[{0}{1}]", cookie.Value, session == null ? " not found" : "");
					}
					if (session == null) {
						if (moduleName == "FileSender") {
							session = server.NamespaceDef.GetInstanceOf<Session>((WebServer)null);
						} else {
							session = server.NamespaceDef.GetInstanceOf<Session>(this);
							cookie = new Cookie("session", session.Cookie, "/");
							Log.Session.WriteLine("[{0} new session]", cookie.Value);
						}
					}
					if (cookie != null) {
						context.Response.Cookies.Add(cookie);
						cookie.Expires = session.Expires = Utils.Now.AddMinutes(server.CookieTimeoutMinutes);
					}
					module.Session = session;
					if (moduleName.EndsWith("Module"))
						moduleName = moduleName.Substring(0, moduleName.Length - 6);
					using (module) {
						// Call method
						module.Call(context, moduleName, methodName);
						if (server.PersistentSessions && session.Server != null && session != server.NamespaceDef.EmptySession)
							session.ToStore(module.Database);
					}
				} catch (Exception ex) {
					while (ex is TargetInvocationException)
						ex = ex.InnerException;
					if (ex is System.Net.Sockets.SocketException) {
						log.AppendFormat("Request error: {0}\r\n", ex.Message);
					} else {
						Log.Error.WriteLine($"Request error: {ex}");
						if (module == null || !module.ResponseSent) {
							try {
								ModuleInfo info = server.NamespaceDef.GetModuleInfo("error");
								module = info == null ? new ErrorModule() : (AppModule)Activator.CreateInstance(info.Type);
								module.Session = server.NamespaceDef.EmptySession;
								module.Server = server;
								module.ActiveModule = webmodules[server.Namespace];
								module.LogString = log;
								module.Context = context;
								module.Module = "exception";
								module.Method = "default";
								module.Title = "Exception";
								module.Exception = ex;
								module.WriteResponse(module.Template("exception", module), "text/html", HttpStatusCode.InternalServerError);
							} catch (Exception ex1) {
								log.AppendFormat("Error displaying exception: {0}\r\n", ex1);
								if (module != null && !module.ResponseSent) {
									try {
										module.WriteResponse("Error displaying exception:" + ex.Message, "text/plain", HttpStatusCode.InternalServerError);
									} catch {
									}
								}
							}
						}
					}
				} finally {
					if (session != null && session.Server == null && session != server.NamespaceDef.EmptySession)
						session.Dispose();		// Dispose of temporary session
				}
			}
			if (context != null) {
				try {
					context.Response.Close();
				} catch {
				}
			}
			try {
				Log.Info.WriteLine(log.ToString().Replace(":[ms]:", ":" + Math.Round((DateTime.Now - started).TotalMilliseconds, 0) + " ms:"));
			} catch {
			}
		}

		/// <summary>
		/// Simple session
		/// </summary>
		[Table]
		public class Session : JsonObject, IDisposable {
			/// <summary>
			/// The session cookie
			/// </summary>
			[Primary(AutoIncrement = false)]
			public string idSession;
			/// <summary>
			/// Logged in user (or null if none)
			/// </summary>
			public int UserId;
			/// <summary>
			/// When the session expires
			/// </summary>
			public DateTime Expires;
			/// <summary>
			/// Logged in user (or null if none)
			/// </summary>
			[DoNotStore]
			[JsonIgnore]
			public User User;
			/// <summary>
			/// Arbitrary JObject stored in session for later access
			/// </summary>
			[DoNotStore]
			[JsonIgnore]
			public JObject Object { get; private set; }
			/// <summary>
			/// The session cookie
			/// </summary>
			public string Cookie {
				get { return idSession; }
				private set { idSession = value; }
			}
			/// <summary>
			/// The WebServer owning the session (or null)
			/// </summary>
			[DoNotStore]
			[JsonIgnore]
			public WebServer Server;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="server"></param>
			public Session(WebServer server) : this() {
				if (server != null) {
					Session session;
					Random r = new Random();

					lock (server._sessions) {
						do {
							Cookie = "";
							for (int i = 0; i < 20; i++)
								Cookie += (char)('A' + r.Next(26));
						} while (server._sessions.TryGetValue(Cookie, out session));
						server._sessions[Cookie] = this;
					}
					Server = server;
				}
			}

			/// <summary>
			/// Empty constructor for when read from database
			/// </summary>
			public Session() {
				Object = new JObject();
			}

			/// <summary>
			/// Create session from store
			/// </summary>
			public static Session FromStore(WebServer server, AppModule module, string cookie) {
				Session session = module.Server.NamespaceDef.GetInstanceOf<Session>();
				session.idSession = cookie;
				if (!module.Database.TryGet(session))
					return null;
				if (session.Expires < Utils.Now) {
					module.Database.Execute("DELETE FROM Session WHERE Expires < " + module.Database.Quote(Utils.Now));
					return null;
				}
				session.Server = server;
				if (session.UserId == 0 || !module.Database.TryGet(session.UserId, out session.User))
					session.User = null;
				server._sessions[session.Cookie] = session;
				return session;
			}

			/// <summary>
			/// Save session to store (or delete from store if expired)
			/// </summary>
			public void ToStore(Database db) {
				UserId = User == null ? 0 : User.idUser.GetValueOrDefault();
				if (Expires < Utils.Now) {
					db.Delete(this);
					return;
				}
				db.Update(this);
			}

			/// <summary>
			/// Free any resources
			/// </summary>
			public virtual void Dispose() {
			}
		}

	}

}
