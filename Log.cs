using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace CodeFirstWebFramework {
	/// <summary>
	/// A class to manage logging to a variety of destinations
	/// </summary>
    public class Log {
		/// <summary>
		/// Log destinations
		/// </summary>
		[Flags]
		public enum Destination {
			/// <summary>
			/// Log to dated log file in the LogFolder directory
			/// </summary>
			Log = 1,
			/// <summary>
			/// Log to stdout
			/// </summary>
			StdOut = 2,
			/// <summary>
			/// Log to stderr
			/// </summary>
			StdErr = 4,
			/// <summary>
			/// Log to file (specify with "file:name"
			/// </summary>
			File = 8,
			/// <summary>
			/// Log to trace output
			/// </summary>
			Trace = 16,
			/// <summary>
			/// Log to debug output
			/// </summary>
			Debug = 32,
			/// <summary>
			/// Do not log
			/// </summary>
			Null = 0
		}

		Destination _destination;
		static object _lock = new object();
		static DateTime _lastDate = DateTime.MinValue;
		static StreamWriter _sw = null;
		StreamWriter _file = null;

		/// <summary>
		/// Create Log with specific destination
		/// </summary>
		public Log(Destination dest) {
			_destination = dest;
		}

		/// <summary>
		/// Parse config for log info and create Log accordingly
		/// </summary>
		/// <param name="config"></param>
		public Log(string config) {
			foreach(string c in config.Split(',')) {
				string trimmed = c.Trim();
				switch(trimmed.ToLower()) {
					case "":
					case "null":
						break;
					case "log":
						_destination |= Destination.Log;
						break;
					case "stdout":
					case "1":
						_destination |= Destination.StdOut;
						break;
					case "stderr":
					case "2":
						_destination |= Destination.StdErr;
						break;
					case "trace":
						_destination |= Destination.Trace;
						break;
					case "debug":
						_destination |= Destination.Debug;
						break;
					default:
						Utils.Check(trimmed.ToLower().StartsWith("file:"), "Unknown log parameter: {0}", c);
						_file = new StreamWriter(new FileStream(trimmed.Substring(5), FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
						_file.AutoFlush = true;
						_destination |= Destination.File;
						break;
				}
			}
		}

		/// <summary>
		/// Folder in which to put dated log files
		/// </summary>
		public static string LogFolder = Path.Combine(CodeFirstWebFramework.Config.DataPath, CodeFirstWebFramework.Config.EntryModule + "Logs");

		/// <summary>
		/// Program startup logging
		/// </summary>
		public static Log Startup = new Log(Destination.Log | Destination.StdOut);
		/// <summary>
		/// Ordinary web request logging (like access.log)
		/// </summary>
		public static Log Info = new Log(Destination.Log);
		/// <summary>
		/// Web request which failed logging
		/// </summary>
		public static Log NotFound = new Log(Destination.Null);
		/// <summary>
		/// Exception and error logging
		/// </summary>
		public static Log Error = new Log(Destination.Log | Destination.StdErr);
		/// <summary>
		/// Debug logging
		/// </summary>
		public static Log Debug = new Log(Destination.Debug);
		/// <summary>
		/// Trace logging
		/// </summary>
		public static Log Trace = new Log(Destination.Trace);
		/// <summary>
		/// Session logging
		/// </summary>
		public static Log Session = new Log(Destination.Null);
		/// <summary>
		/// Database access logging
		/// </summary>
		public static Log DatabaseRead = new Log(Destination.Null);
		/// <summary>
		/// Database write logging
		/// </summary>
		public static Log DatabaseWrite = new Log(Destination.Null);
		/// <summary>
		/// Post data logging
		/// </summary>
		public static Log PostData = new Log(Destination.Null);

		/// <summary>
		/// Strings to describe log destinations
		/// </summary>
		public class Config {
#pragma warning disable 1591
			public string Startup = "Log,StdOut";
			public string Info = "Log";
			public string NotFound = "Null";
			public string Error = "Log,StdErr";
			public string Debug = "Debug";
			public string Trace = "Trace";
			public string Session = "Null";
			public string DatabaseRead = "Null";
			public string DatabaseWrite = "Null";
			public string PostData = "Null";
#pragma warning restore 1591

			/// <summary>
			/// Update the Log destinations
			/// </summary>
			public void Update() {
				foreach(FieldInfo f in GetType().GetFields()) {
					FieldInfo l = typeof(Log).GetField(f.Name, BindingFlags.Public | BindingFlags.Static);
					if (l != null) {
						string value = f.GetValue(this).ToString();
						string cmdline = CodeFirstWebFramework.Config.CommandLineFlags[f.Name + ".log"];
						if (!string.IsNullOrWhiteSpace(cmdline))
							value = cmdline;
						l.SetValue(null, new Log(value));
					}
				}
			}
		}

		/// <summary>
		/// Whether this type of logging is outputting anywhere
		/// </summary>
		public bool On {  get { return _destination != Destination.Null;  } }

		/// <summary>
		/// Log message to console and trace
		/// </summary>
		public void WriteLine(string s) {
			if (_destination == Destination.Null)
				return;
			s = s.Trim();
			lock (_lock) {
				if ((_destination & Destination.Log) != 0) {
					open();
					_sw.WriteLine(Utils.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + s);
				}
				if ((_destination & Destination.StdOut) != 0)
					Console.WriteLine(s);
				if ((_destination & Destination.StdErr) != 0)
					Console.Error.WriteLine(s);
				if ((_destination & Destination.File) != 0 && _file != null) {
					_file.WriteLine(Utils.Now.ToString("HH:mm:ss") + " " + s);
				}
				if ((_destination & Destination.Trace) != 0)
					System.Diagnostics.Trace.WriteLine(s);
				if ((_destination & Destination.Debug) != 0)
					System.Diagnostics.Debug.WriteLine(s);
			}
		}

		/// <summary>
		/// Log message to console and trace
		/// </summary>
		public void WriteLine(string format, params object[] args) {
			try {
				WriteLine(string.Format(format, args));
			} catch (Exception ex) {
				WriteLine(string.Format("{0}:Error logging {1}", format, ex.Message));
			}
		}

		/// <summary>
		/// Close the file
		/// </summary>
		public static void Close() {
			if (_sw != null)
				_sw.Close();
			_sw = null;
		}

		/// <summary>
		/// Flush the file to disk
		/// </summary>
		public static void Flush() {
			Close();
		}

		static string fileName() {
			_lastDate = Utils.Today;
			return fileName(_lastDate);
		}

		static string fileName(DateTime date) {
			return Path.Combine(LogFolder, date.ToString("yyyy-MM-dd") + ".log");
		}

		static void open() {
			if (_sw == null || Utils.Today != _lastDate) {
				if (_sw != null) {
					_sw.Close();
				}
				Directory.CreateDirectory(LogFolder);
				_sw = new StreamWriter(new FileStream(fileName(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
				_sw.AutoFlush = true;
			}
		}
	}
}
