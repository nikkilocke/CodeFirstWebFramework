using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeFirstWebFramework {
	/// <summary>
	/// Interface to "files" the web server can access.
	/// An interface is used so a Namespace can deliver content from the database instead of the file system
	/// </summary>
	public interface IFileInfo {
		/// <summary>
		/// Text of the file
		/// </summary>
		string Content(AppModule module);

		/// <summary>
		/// Whether the file exists
		/// </summary>
		bool Exists { get; }

		/// <summary>
		///  File extension
		/// </summary>
		string Extension { get; }

		/// <summary>
		/// The modification time
		/// </summary>
		DateTime LastWriteTimeUtc { get; }

		/// <summary>
		/// File name (without extension)
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Path
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Stream containing content
		/// </summary>
		System.IO.Stream Stream(AppModule module);

	}

	/// <summary>
	/// Interface to "directories" the web server can access.
	/// An interface is used so a Namespace can deliver content from the database instead of the file system
	/// </summary>
	public interface IDirectoryInfo {
		/// <summary>
		/// List of IFileInfo items in the directory
		/// </summary>
		IEnumerable<IFileInfo> Content(string pattern);

		/// <summary>
		/// Whether the directory exists
		/// </summary>
		bool Exists { get; }

		/// <summary>
		/// Directory name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Path
		/// </summary>
		string Path { get; }

	}

	/// <summary>
	/// IFileInfo that uses the filesystem
	/// </summary>
	public class FileInfo : IFileInfo {
		System.IO.FileInfo info;

		/// <summary>
		/// Construct from a System.IO.FileInfo
		/// </summary>
		public FileInfo(string fullpath, System.IO.FileInfo info) {
			this.info = info;
			fullpath = fullpath.Replace("\\", ".");
			int pos = fullpath.LastIndexOf('/');
			Path = pos >= 0 ? fullpath.Substring(0, pos + 1) : "";
		}

		/// <summary>
		/// Whether the file exists
		/// </summary>
		public bool Exists { get { return info.Exists; } }

		/// <summary>
		/// File extension
		/// </summary>
		public string Extension { get { return info.Extension; } }

		/// <summary>
		/// The modification time
		/// </summary>
		public DateTime LastWriteTimeUtc { get { return info.LastWriteTimeUtc; } }

		/// <summary>
		/// File name without extension
		/// </summary>
		public string Name { get { return System.IO.Path.GetFileNameWithoutExtension(info.Name); } }

		/// <summary>
		/// Path
		/// </summary>
		public string Path { get; private set; }

		/// <summary>
		/// File content
		/// </summary>
		public string Content(AppModule module) {
			using (StreamReader s = new StreamReader(info.FullName, AppModule.Encoding)) {
				return s.ReadToEnd();
			}
		}

		/// <summary>
		/// Stream containing content
		/// </summary>
		public System.IO.Stream Stream(AppModule module) {
			return new FileStream(info.FullName, FileMode.Open, FileAccess.Read);
		}
	}

	/// <summary>
	/// IDirectoryInfo that uses the filesystem
	/// </summary>
	public class DirectoryInfo : IDirectoryInfo {
		System.IO.DirectoryInfo info;

		/// <summary>
		/// Construct from a System.IO.DirectoryInfo
		/// </summary>
		public DirectoryInfo(string fullpath, System.IO.DirectoryInfo info) {
			this.info = info;
			fullpath = fullpath.Replace("\\", ".");
			int pos = fullpath.LastIndexOf('/');
			Path = pos >= 0 ? fullpath.Substring(0, pos + 1) : "";
		}

		/// <summary>
		/// Whether the directory exists
		/// </summary>
		public bool Exists { get { return info.Exists; } }

		/// <summary>
		/// Directory name
		/// </summary>
		public string Name { get { return info.Name; } }

		/// <summary>
		/// Path
		/// </summary>
		public string Path { get; private set; }

		/// <summary>
		/// Search the directory for files matching the pattern
		/// </summary>
		public IEnumerable<IFileInfo> Content(string pattern) {
			foreach (System.IO.FileInfo f in info.GetFiles(pattern)) {
				yield return new FileInfo(Path, f);
			}
		}
	}

	/// <summary>
	/// Interface to the file system which converts file names into IFileInfo objects,
	/// and directory names into IDirectoryInfo objects
	/// </summary>
	public class FileSystem {

		/// <summary>
		/// Search the list of folders for a file matching the filename
		/// </summary>
		/// <param name="module">AppModule making the call</param>
		/// <param name="filename">Like a url - e.g. "admin/settings.html"</param>
		virtual public IFileInfo FileInfo(AppModule module, string filename) {
			System.IO.FileInfo f;
			string folder = Path.Combine(Config.DataPath, module.Server.ServerName);
			if (filename.StartsWith("/"))
				filename = filename.Substring(1);
			f = new System.IO.FileInfo(Path.Combine(folder, filename));
			Utils.Check(f.FullName.StartsWith(new System.IO.FileInfo(folder).FullName), "Illegal file access");
			if (!f.Exists)
				f = new System.IO.FileInfo(Path.Combine(module.Server.ServerName, filename));
			if (!f.Exists)
				f = new System.IO.FileInfo(Path.Combine(Config.DataPath, module.Server.Namespace, filename));
			if (!f.Exists)
				f = new System.IO.FileInfo(Path.Combine(module.Server.Namespace, filename));
			if (!f.Exists)
				f = new System.IO.FileInfo(Path.Combine(Config.DataPath, CodeFirstWebFramework.Config.Default.ServerName, filename));
			if (!f.Exists)
				f = new System.IO.FileInfo(Path.Combine(CodeFirstWebFramework.Config.Default.ServerName, filename));
			if (!f.Exists)
				f = new System.IO.FileInfo(Path.Combine(Config.DataPath, CodeFirstWebFramework.Config.DefaultNamespace, filename));
			if (!f.Exists)
				f = new System.IO.FileInfo(Path.Combine(CodeFirstWebFramework.Config.DefaultNamespace, filename));
			return new FileInfo(filename, f);
		}

		/// <summary>
		/// Search the list of folders for a folder matching the foldername
		/// </summary>
		/// <param name="module">AppModule making the call</param>
		/// <param name="foldername">Like a url - e.g. "admin/settings.html"</param>
		virtual public IDirectoryInfo DirectoryInfo(AppModule module, string foldername) {
			System.IO.DirectoryInfo f;
			string folder = Path.Combine(Config.DataPath, module.Server.ServerName);
			if (foldername.StartsWith("/"))
				foldername = foldername.Substring(1);
			f = new System.IO.DirectoryInfo(Path.Combine(folder, foldername));
			Utils.Check(f.FullName.StartsWith(new System.IO.FileInfo(folder).FullName), "Illegal file access");
			if (!f.Exists)
				f = new System.IO.DirectoryInfo(Path.Combine(module.Server.ServerName, foldername));
			if (!f.Exists)
				f = new System.IO.DirectoryInfo(Path.Combine(Config.DataPath, module.Server.Namespace, foldername));
			if (!f.Exists)
				f = new System.IO.DirectoryInfo(Path.Combine(module.Server.Namespace, foldername));
			if (!f.Exists)
				f = new System.IO.DirectoryInfo(Path.Combine(Config.DataPath, CodeFirstWebFramework.Config.Default.ServerName, foldername));
			if (!f.Exists)
				f = new System.IO.DirectoryInfo(Path.Combine(CodeFirstWebFramework.Config.Default.ServerName, foldername));
			if (!f.Exists)
				f = new System.IO.DirectoryInfo(Path.Combine(Config.DataPath, CodeFirstWebFramework.Config.DefaultNamespace, foldername));
			if (!f.Exists)
				f = new System.IO.DirectoryInfo(Path.Combine(CodeFirstWebFramework.Config.DefaultNamespace, foldername));
			return new DirectoryInfo(foldername, f);
		}

	}
}
