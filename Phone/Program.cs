using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
					if (Config.CommandLineFlags["nolaunch"] == null) {
						string url = "http://" + Config.Default.DefaultServer.ServerName + ":" + Config.Default.Port + "/";
						if (Config.CommandLineFlags["url"] != null)
							url += Config.CommandLineFlags["url"];
						if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
							Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}"));
						} else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
							Process.Start("xdg-open", "'" + url + "'");
						} else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
							Process.Start("open", "'" + url + "'");
						}
					}
					break;
			}
			new WebServer().Start();
		}
	}
}
