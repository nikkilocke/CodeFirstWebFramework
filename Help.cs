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
	/// <summary>
	/// AppModule to handle help requests
	/// </summary>
	[Handles(".md")]
	public class Help : AppModule {
		/// <summary>
		/// Whether the help has a table of contents (used in templates)
		/// </summary>
		public bool Contents;
		/// <summary>
		/// Markdown-style link to parent (if any) in table of contents
		/// </summary>
		public string Parent;
		/// <summary>
		/// Markdown-style link to next page (if any) in table of contents
		/// </summary>
		public string Next;
		/// <summary>
		/// Markdown-style link to previous page (if any) in table of contents
		/// </summary>
		public string Previous;

		/// <summary>
		/// Display a help file from the url
		/// </summary>
		public override void Default() {
			if(Request.Url.AbsolutePath.Split('/').Length < 3) {
				// Make sure the url is explicitly a file in help, so relative links work
				Redirect("/help/default.md");
				return;
			}
			FileInfo file = Server.FileInfo(Module + "/" + Method + ".md");
			if (!file.Exists) {
				// Maybe this is a templated help file
				file = Server.FileInfo(Module + "/" + Method + ".tmpl");
			}
			ReturnHelpFrom(file);
		}

		/// <summary>
		/// Override CallMethod so it also accepts unknown Method names (presuming them to be md files) and still displays them
		/// </summary>
		public override object CallMethod(out MethodInfo method) {
			method = this.GetType().GetMethod(Method, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			if (method != null)
				return base.CallMethod(out method);
			method = this.GetType().GetMethod("Default", BindingFlags.Public | BindingFlags.Instance);
			Default();
			return null;
		}

		/// <summary>
		/// Return the Markdown help text from file in a web page ready to render.
		/// If Method is not "default.md", load the table of contents information
		/// If the file is itself a template, fill it in first (output will be Markdown).
		/// Renders the md file inside the "/help/default.tmpl" template (to add the contents links)
		/// </summary>
		protected string LoadHelpFrom(FileInfo file) {
			if (Method != "default") {
				// Read the default.md file for the table of contents, to populate Next, Previous, etc.
				parseContents((Method + ".md").ToLower());
			}
			Body = Server.LoadFile(file);
			if (file.Extension == ".tmpl") {
				// Do the templating of the contents themselves
				Body = Server.TextTemplate(Body, this);
				System.Diagnostics.Debug.WriteLine(Body);
			}
			string s = LoadTemplate("/help/default", this); // Place the content in the default.tmpl wrapper
			return Markdown.ToHtml(s, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());
		}

		/// <summary>
		/// Render the Markdown help text from file in a web page.
		/// Allows caching and supports If-Modified-Since headers
		/// If Method is not "default.md", load the table of contents information
		/// If the file is itself a template, fill it in first (output will be Markdown).
		/// Renders the md file inside the "/help/default.tmpl" template (to add the contents links)
		/// </summary>
		protected void ReturnHelpFrom(FileInfo file) {
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
			CacheAllowed = true;
			Response.AddHeader("Last-Modified", file.LastWriteTimeUtc.ToString("r"));
			string s = LoadHelpFrom(file);
			WriteResponse(s, "text/html", HttpStatusCode.OK);
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
