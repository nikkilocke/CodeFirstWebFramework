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

namespace CodeFirstWebFramework {
	/// <summary>
	/// Web Server - listens for connections, and services them
	/// </summary>
	public class WebServer {
		HttpListener _listener;
		bool _running;
		Dictionary<string, Session> _sessions;
		static object _lock = new object();
		Session _empty;
		Dictionary<string, Namespace> modules;      // All the different web modules we are running
		HashSet<string> loadedAssemblies;

		/// <summary>
		/// Constructor
		/// </summary>
		public WebServer() {
			try {
				modules = new Dictionary<string, Namespace>();
				loadedAssemblies = new HashSet<string>();
				var baseType = typeof(AppModule);
				HashSet<string> databases = new HashSet<string>();
				foreach (ServerConfig server in Config.Default.Servers) {
					registerServer(databases, server);
				}
				registerServer(databases, Config.Default.DefaultServer);
			} catch (Exception ex) {
				Log(ex.ToString());
				throw;
			}
		}

		/// <summary>
		/// Add namespace to modules list, and upgrade database, if not done already.
		/// </summary>
		/// <param name="databases">HashSet of databases already upgraded</param>
		/// <param name="server">ServerConfig to register</param>
		void registerServer(HashSet<string> databases, ServerConfig server) {
			if (modules.ContainsKey(server.Namespace)) {
				server.NamespaceDef = modules[server.Namespace];
			} else {
				foreach(string assembly in server.AdditionalAssemblies) {
					if (loadedAssemblies.Contains(assembly))
						continue;
					Assembly.Load(assembly);
					loadedAssemblies.Add(assembly);
				}
				server.NamespaceDef = new Namespace(server.Namespace);
				modules[server.Namespace] = server.NamespaceDef;
			}
			using (Database db = server.NamespaceDef.GetDatabase(server)) {
				if (!databases.Contains(db.UniqueIdentifier)) {
					databases.Add(db.UniqueIdentifier);
					db.Upgrade();
				}
			}
		}

		/// <summary>
		/// Log message to console and trace
		/// </summary>
		static public void Log(string s) {
			s = s.Trim();
			lock (_lock) {
				System.Diagnostics.Trace.WriteLine(s);
				Console.WriteLine(s);
			}
		}

		/// <summary>
		/// Log message to console and trace
		/// </summary>
		static public void Log(string format, params object[] args) {
			try {
				Log(string.Format(format, args));
			} catch (Exception ex) {
				Log(string.Format("{0}:Error logging {1}", format, ex.Message));
			}
		}

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
					Log("Listening on port {0}", port);
				}
				_sessions = new Dictionary<string, Session>();
				_empty = new Session(null);
				// Start thread to expire sessions after 30 mins of inactivity
				new Task(delegate () {
					for (;;) {
						Thread.Sleep(Config.Default.SessionExpiryMinutes * 1000);
						DateTime now = Utils.Now;
						lock (_sessions) {
							foreach (string key in _sessions.Keys.ToList()) {
								Session s = _sessions[key];
								if (s.Expires < now)
									_sessions.Remove(key);
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
				Log(ex.ToString());
			} catch (ThreadAbortException) {
			} catch (Exception ex) {
				Log(ex.ToString());
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
			if(server == null) {
				// Request not matching any of the Server array, and not on the default port
				context.Response.StatusCode = 404;
				context.Response.ContentType = "text/plain;charset=" + AppModule.Charset;
				byte[] msg = AppModule.Encoding.GetBytes("Server not found");
				context.Response.ContentLength64 = msg.Length;
				Log("404 Server not found ");
				using (Stream r = context.Response.OutputStream) {
					r.Write(msg, 0, msg.Length);
				}
				return;
			}
			try {
				log.AppendFormat("{0} {1}:{2}:[ms]:", 
					context.Request.RemoteEndPoint.Address,
					context.Request.Headers["X-Forwarded-For"],
					context.Request.RawUrl);
				Session session = null;
				ModuleInfo info = modules[server.Namespace].ParseUri(context.Request.Url.AbsolutePath, out string filename);
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
					module = new FileSender(filename);
				}
				// AppModule found - retrieve or create a session for it
				Cookie cookie = context.Request.Cookies["session"];
				if (cookie != null) {
					_sessions.TryGetValue(cookie.Value, out session);
					if (Config.Default.SessionLogging)
						log.AppendFormat("[{0}{1}]", cookie.Value, session == null ? " not found" : "");
				}
				if (session == null) {
					if (moduleName == "FileSender") {
						session = new Session(null);
					} else {
						session = new Session(this);
						cookie = new Cookie("session", session.Cookie, "/");
						if (Config.Default.SessionLogging)
							log.AppendFormat("[{0} new session]", cookie.Value);
					}
				}
				if (cookie != null) {
					context.Response.Cookies.Add(cookie);
					cookie.Expires = session.Expires = Utils.Now.AddHours(1);
				}
				// Set up module
				module.Server = server;
				module.ActiveModule = modules[server.Namespace];
				module.Session = session;
				module.LogString = log;
				if (moduleName.EndsWith("Module"))
					moduleName = moduleName.Substring(0, moduleName.Length - 6);
				using (module) {
					// Call method
					module.Call(context, moduleName, methodName);
				}
			} catch (Exception ex) {
				while (ex is TargetInvocationException)
					ex = ex.InnerException;
				if (ex is System.Net.Sockets.SocketException) {
					log.AppendFormat("Request error: {0}\r\n", ex.Message);
				} else {
					log.AppendFormat("Request error: {0}\r\n", ex);
					if (module == null || !module.ResponseSent) {
						try {
							module = new ErrorModule();
							module.Session = _empty;
							module.Server = server;
							module.ActiveModule = modules[server.Namespace];
							module.LogString = log;
							module.Context = context;
							module.Module = "exception";
							module.Method = "default";
							module.Title = "Exception";
							module.Exception = ex;
							module.WriteResponse(module.Template("exception", module), "text/html", HttpStatusCode.InternalServerError);
						} catch (Exception ex1) {
							log.AppendFormat("Error displaying exception: {0}\r\n", ex1);
							if (module == null || !module.ResponseSent) {
								try {
									module.WriteResponse("Error displaying exception:" + ex.Message, "text/plain", HttpStatusCode.InternalServerError);
								} catch {
								}
							}
						}
					}
				}
			}
			if (context != null) {
				try {
						context.Response.Close();
				} catch {
				}
			}
			try {
				Log(log.ToString().Replace(":[ms]:", ":" + Math.Round((DateTime.Now - started).TotalMilliseconds, 0) + " ms:"));
			} catch {
			}
		}

		/// <summary>
		/// Simple session
		/// </summary>
		public class Session {
			/// <summary>
			/// Logged in user (or null if none)
			/// </summary>
			public User User;
			/// <summary>
			/// Arbitrary JObject stored in session for later access
			/// </summary>
			public JObject Object { get; private set; }
			/// <summary>
			/// When the session expires
			/// </summary>
			public DateTime Expires;
			/// <summary>
			/// The session cookie
			/// </summary>
			public string Cookie { get; private set; }
			/// <summary>
			/// The WebServer owning the session (or null)
			/// </summary>
			public WebServer Server;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="server"></param>
			public Session(WebServer server) {
				if (server != null) {
					Session session;
					Random r = new Random();

					lock (server._sessions) {
						do {
							Cookie = "";
							for (int i = 0; i < 20; i++)
								Cookie += (char)('A' + r.Next(26));
						} while (server._sessions.TryGetValue(Cookie, out session));
						Object = new JObject();
						server._sessions[Cookie] = (Session)this;
					}
					Server = server;
				}
			}
		}
	}

}
