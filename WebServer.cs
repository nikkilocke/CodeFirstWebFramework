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
		Dictionary<string, int> databases;

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
				databases = new Dictionary<string, int>();
				foreach (ServerConfig server in Config.Default.Servers) {
					registerServer(server);
				}
				registerServer(Config.Default.DefaultServer);
			} catch (Exception ex) {
				Log.Error.WriteLine(ex.ToString());
				throw;
			}
		}

		/// <summary>
		/// Add namespace to modules list, and upgrade database, if not done already.
		/// </summary>
		/// <param name="server">ServerConfig to register</param>
		void registerServer(ServerConfig server) {
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
				if (!databases.TryGetValue(db.UniqueIdentifier, out int index)) {
					databases[db.UniqueIdentifier] = index = databases.Count;
					db.Upgrade();
				}
				server.DatabaseId = index;
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
					module.GenerateNonce();
					if (cookie != null) {
						if (!_sessions.TryGetValue(cookie.Value, out session) && server.PersistentSessions)
							session = Session.FromStore(this, module, cookie.Value);
						Log.Session.WriteLine("[{0}{1}]", cookie.Value, session == null ? " not found" : "");
					}
					if (session == null) {
						if (moduleName == "FileSender") {
							session = server.NamespaceDef.GetInstanceOf<Session>();
						} else {
							session = server.NamespaceDef.GetInstanceOf<Session>(this, server);
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
								module.GenerateNonce();
								HttpStatusCode code = HttpStatusCode.InternalServerError;
								string template = "exception";
								if (ex is CheckException check) {
									code = check.Code;
									template = check.Template ?? template;
								}
								module.WriteResponse(module.Template(template, module), "text/html", code);
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
					try {
						if (server != null && session != null && session != server.NamespaceDef.EmptySession) {
							if (session.Server == null)
								session.Dispose();      // Dispose of temporary session
							else if (server.PersistentSessions)
								session.ToStore(server);
						}
					} catch(Exception ex) {
						System.Diagnostics.Debug.WriteLine("Session save error:" + ex);
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
				Log.Info.WriteLine(log.ToString().Replace(":[ms]:", ":" + Math.Round((DateTime.Now - started).TotalMilliseconds, 0) + " ms:"));
			} catch {
			}
		}

		/// <summary>
		/// Message text/html for displaying in a new screen.
		/// This is stored in the Session, so people can't hack the text.
		/// </summary>
		public class MessageInfo {
			/// <summary>
			/// The text of the message
			/// </summary>
			public string Text;
			/// <summary>
			/// Day number since 1/1/2000 the message was created
			/// </summary>
			public int Date;
			/// <summary>
			/// Sequence number of this message (resets to 1 at midnight)
			/// </summary>
			public int Sequence;
			/// <summary>
			/// Unique id of this message
			/// </summary>
			public string Handle { get; private set; }
			/// <summary>
			/// Next sequence number today
			/// </summary>
			static int NextSequence = 1;
			static int LastDate;
			static DateTime Base = new DateTime(2000, 1, 1);
			/// <summary>
			/// Convert a DateTime into days since Base
			/// </summary>
			public static int Days(DateTime d) {
				return (int)(DateTime.Today - Base).TotalDays;
			}
			/// <summary>
			/// Object to prevent two threads updating NextSequence at the same time
			/// </summary>
			static object Lock = new object();
			/// <summary>
			/// Create a new message
			/// </summary>
			public MessageInfo(string text) {
				Text = text;
				lock (Lock) {
					Date = Days(DateTime.Today);
					if (Date != LastDate)
						NextSequence = 1;
					Sequence = NextSequence++;
				}
				byte[] bytes = BitConverter.GetBytes(Date).Concat(BitConverter.GetBytes(Sequence)).ToArray();
				Handle = Utils.UniqueId(bytes);
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
			/// The Database id
			/// </summary>
			[DoNotStore]
			[JsonIgnore]
			public ServerConfig Config { get; private set; }
			[JsonIgnore]
			[DoNotStore]
			List<MessageInfo> Messages;

			/// <summary>
			/// Constructor
			/// </summary>
			public Session(WebServer server, ServerConfig config) : this() {
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
				Config = config;
			}

			/// <summary>
			/// Empty constructor for when read from database
			/// </summary>
			public Session() {
				Object = new JObject();
			}

			/// <summary>
			/// Add a message to the message list
			/// </summary>
			/// <returns>Handle of the added message</returns>
			public string AddMessage(string message) {
				MessageInfo m = new MessageInfo(message);
				if (Messages == null)
					Messages = new List<MessageInfo>();
				else {
					// Get rid of messages more than a day old
					lock(Object) {
						int old = MessageInfo.Days(DateTime.Today.AddDays(-1));
						while (Messages.Count > 0 && Messages[0].Date < old)
							Messages.RemoveAt(0);
					}
				}
				Messages.Add(m);
				return m.Handle;
			}

			/// <summary>
			/// Add a message to the message list (encoding to avoid any html, but keeping newlines as <br/>)
			/// </summary>
			/// <returns>Handle of the added message</returns>
			public string AddTextMessage(string message) {
				message = string.Join("<br/>\n", message.Replace("\r", "").Split('\n').Select(s => HttpUtility.HtmlEncode(s)));
				return AddMessage(message);
			}

			/// <summary>
			/// Get the text of a message from the list for this session (and remove it)
			/// </summary>
			/// <returns>Message text, or null if no such message</returns>
			public string GetMessage(string handle) {
				if (Messages == null)
					return null;
				MessageInfo r = Messages.FirstOrDefault(m => m.Handle == handle);
				if(r == null) return null;
				Messages.Remove(r);
				return r.Text;
			}

			/// <summary>
			/// Create session from store
			/// </summary>
			public static Session FromStore(WebServer server, AppModule module, string cookie) {
				Namespace nameSpace = module.Server.NamespaceDef;
				lock (nameSpace) {
					if (!module.Database.TryGet(out Session session, cookie))
						return null;
					if (session.Expires < Utils.Now) {
						module.Database.Execute("DELETE FROM Session WHERE Expires < " + module.Database.Quote(Utils.Now));
						return null;
					}
					session.Server = server;
					session.Config = module.Server;
					if (session.UserId == 0 || !module.Database.TryGet(session.UserId, out session.User))
						session.User = null;
					server._sessions[session.Cookie] = session;
					return session;
				}
			}

			/// <summary>
			/// Save session to store (or delete from store if expired)
			/// </summary>
			public void ToStore(ServerConfig server) {
				Namespace nameSpace = server.NamespaceDef;
				lock (nameSpace) {
					UserId = User == null ? 0 : User.idUser.GetValueOrDefault();
					using (Database Database = nameSpace.GetDatabase(server)) {
						if (Expires < Utils.Now)
							Database.Delete(this);
						else
							Database.Update(this);
					}
				}
			}

			/// <summary>
			/// Free any resources
			/// </summary>
			public virtual void Dispose() {
			}
		}

	}

}
