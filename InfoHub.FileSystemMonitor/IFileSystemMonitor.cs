using System;

namespace InfoHub.FileSystemMonitor
{
	/// <summary>
	/// Enumerates the types of file system changes that can be detected
	/// </summary>
	public enum ChangeType {
		Created,
		Changed,
		Deleted,
		Renamed
	}

	/// <summary>
	/// Provides data for the IFileSystemMonitor.FileSystemChanged event.
	/// </summary>
	public class FileSystemChangeEventArgs : EventArgs {
		public FileSystemChangeEventArgs(ChangeType change, String oldPath, String newPath) {
			this.ChangeType = change;
			this.OldPath = oldPath;
			this.NewPath = newPath;
		}

		public readonly ChangeType ChangeType;
		public readonly String OldPath;
		public readonly String NewPath;
	}

	/// <summary>
	/// A method that handles the IFileSystemMonitor.FileSystemChanged event.
	/// </summary>
	public delegate void FileSystemChangeEventHandler(Object sender, FileSystemChangeEventArgs e);

	/// <summary>
	/// Interface for a file system monitor implementation, which monitors
	/// path(s) on the local file system for changes, reporting them via an
	/// event.
	/// 
	/// The monitor will detect changes in the contents of the file such that
	/// the file's modification date, size, or name are modified.  Changes to 
	/// file ACLs or other extended attributes of the file may not be reported, and
	/// should not be expected.
	/// </summary>
	public interface IFileSystemMonitor {
		/// <summary>
		/// The path to the directory to monitor.  When monitoring 
		/// a directory, changes to the directory contents or any subdirectories
		/// are reported.  If a file is specified, or a path to a non-existent 
		/// location is provided, an exception is thrown.
		/// </summary>
		String Path {get; set;}

		/// <summary>
		/// Event which fires when the area of the file system specified in the 
		/// Path property changes in some way.
		/// </summary>
		event FileSystemChangeEventHandler FileSystemChanged;
	}
}
