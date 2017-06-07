using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Markdig;

namespace CodeFirstWebFramework {
	[Handles(".md")]
	public class Help : AppModule {
		public bool Contents;
		public string Parent;
		public string Next;
		public string Previous;

		public override void Default() {
			if(Request.Url.AbsolutePath.Split('/').Length < 3) {
				// Make sure the url is explicitly a file in help, so relative links work
				Redirect("/help/default.md");
				return;
			}
			FileInfo file = Server.FileInfo(Module + "/" + Method + ".md");
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
			if (Method != "default")
				parseContents((Method + ".md").ToLower());
			using (StreamReader r = new StreamReader(file.FullName)) {
				Body = r.ReadToEnd();
				string s = LoadTemplate("/help/default", this);
				s = Markdown.ToHtml(s);
				WriteResponse(s, "text/html", HttpStatusCode.OK);
			}
		}

		/// <summary>
		/// Override CallMethod so it accepts any Method name (presuming it to be an md file)
		/// </summary>
		public override object CallMethod(out MethodInfo method) {
			method = this.GetType().GetMethod("Default", BindingFlags.Public | BindingFlags.Instance);
			Default();
			return null;
		}

		/// <summary>
		/// Set up Next, Previous and Parent by finding the current file in the default.md table of contents
		/// </summary>
		/// <param name="current">Current file</param>
		void parseContents(string current) {
			FileInfo file = Server.FileInfo("/help/default.md");
			Regex regex = new Regex(@"^(\#+) \[(.+)\]\((.*)\)");
			int level = 0;
			bool found = false;
			string previous = null;
			List<string> links = new List<string>();
			using (StreamReader r = new StreamReader(file.FullName)) {
				string line;
				while((line = r.ReadLine()) != null) {
					Match m = regex.Match(line);
					if(m.Success) {
						int l = m.Groups[1].Length - 1;
						string name = m.Groups[2].Value;
						string link = m.Groups[3].Value;
						if (found) {
							Next = "[" + name + "](" + link + ")";
							break;
						}
						while (l >= links.Count)
							links.Add(null);
						level = l;
						if (link == current) {
							found = true;
							Previous = previous;
							while(--l >= 0) {
								if(links[l] != null) {
									Parent = links[l];
									break;
								}
							}
						}
						links[level] = previous = "[" + name + "](" + link + ")";
					}
				}
			}
			Contents = true;
		}

	}
}
