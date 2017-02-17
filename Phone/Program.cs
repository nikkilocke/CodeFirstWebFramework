using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFirstWebFramework;

namespace Phone {
	class Program {
		public static void Main(string[] args) {
			Config.Load(args);
			switch (Environment.OSVersion.Platform) {
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
					string startPage = "";
					if (Config.CommandLineFlags["url"] != null)
						startPage = Config.CommandLineFlags["url"];
					if (Config.CommandLineFlags["nolaunch"] == null)
						System.Diagnostics.Process.Start("http://" + Config.Default.DefaultServer.ServerName + ":" + Config.Default.Port + "/" + startPage);
					break;
			}
			new WebServer().Start();
		}
	}
}
