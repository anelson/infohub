using System;
using System.Diagnostics;
using System.IO;

using InfoHub.Common;

using log4net;

namespace InfoHub.FileSystemMonitor
{
	/// <summary>
	/// Implementation of IFileSystemMonitor which uses the .NET Framework's
	/// FileSystemWatcher.
	/// </summary>
	public class FrameworkFileSystemMonitor : IDisposable, IFileSystemMonitor
	{
		/// <summary>
		/// The maximum size (in bytes) of the FileSystemWatcher's underlying change
		/// buffer
		/// </summary>
		private const int MAX_BUFFER_SIZE = 1024*1024;

		private FileSystemWatcher _watcher;
		private String _path;
		private LoggerHelper _logger;

		/// <summary>
		/// Creates a new file system monitor object using a caller-defined logger factory
		/// </summary>
		/// <param name="logger"></param>
		public FrameworkFileSystemMonitor(ILoggerFactory loggerFactory) {
			_logger = new LoggerHelper(loggerFactory.GetLogger(this.GetType()), "StringConstants");
		}

		#region IDisposable Members

		public void Dispose() {
			//If currently watching, stop
			if (_watcher != null) {
				_logger.Debug("LogMsg.Disposing");
				_watcher.Dispose();
				_watcher = null;
				_path = null;
			}
		}

		#endregion

		#region IFileSystemMonitor Members

		public String Path {
			get {
				return _path;
			}
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}

				if (_path != value) {
					//Make sure the watcher is created
					_logger.Debug("LogMsg.StartingWatch", value);
					if (_watcher == null) {
						CreateWatcher();
					}

					//Suspend change notifications during transition
					_watcher.EnableRaisingEvents = false;

					//Set the new path to monitor
					_watcher.Path = value;

					//Resume change notifications
					_watcher.EnableRaisingEvents = true;

					_path = value;
				}
			}
		}

		public event InfoHub.FileSystemMonitor.FileSystemChangeEventHandler FileSystemChanged;

		#endregion

		/// <summary>
		/// Creates a new watcher object and initializes its properties
		/// </summary>
		private void CreateWatcher() {
			Debug.Assert(_watcher == null);

			_watcher = new FileSystemWatcher();
			_watcher.NotifyFilter = NotifyFilters.DirectoryName | 
				NotifyFilters.FileName | 
				NotifyFilters.LastWrite |
				NotifyFilters.Size;
			_watcher.IncludeSubdirectories = true;

			//Register internal handlers for the watcher events
			_watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
			_watcher.Created += new FileSystemEventHandler(_watcher_Created);
			_watcher.Deleted += new FileSystemEventHandler(_watcher_Deleted);
			_watcher.Renamed += new RenamedEventHandler(_watcher_Renamed);
			_watcher.Error += new ErrorEventHandler(_watcher_Error);
		}

		private void _watcher_Changed(object sender, FileSystemEventArgs e) {
			_logger.Debug("LogMsg.ChangedEventFired", e.FullPath);

			//Report this change
			OnFileSystemChanged(new FileSystemChangeEventArgs(ChangeType.Changed, e.FullPath, e.FullPath));
		}

		private void _watcher_Created(object sender, FileSystemEventArgs e) {
			_logger.Debug("LogMsg.CreatedEventFired", e.FullPath);

			//Report this change
			OnFileSystemChanged(new FileSystemChangeEventArgs(ChangeType.Created, e.FullPath, e.FullPath));
		}

		private void _watcher_Deleted(object sender, FileSystemEventArgs e) {
			_logger.Debug("LogMsg.DeletedEventFired", e.FullPath);

			//Report this change
			OnFileSystemChanged(new FileSystemChangeEventArgs(ChangeType.Deleted, e.FullPath, e.FullPath));
		}

		private void _watcher_Renamed(object sender, RenamedEventArgs e) {
			_logger.Debug("LogMsg.RenamedEventFired", e.OldFullPath, e.FullPath);

			//Report this change
			OnFileSystemChanged(new FileSystemChangeEventArgs(ChangeType.Renamed, e.OldFullPath, e.FullPath));
		}

		private void _watcher_Error(object sender, ErrorEventArgs e) {
			_logger.Error("LogMsg.ChangedEventFired", e.GetException());

			//Ostensibly, this happens when the FileSystemWatcher's
			//buffer for reporting changes overflows.  This buffer is allocated
			//in the non-paged pool, and thus must be kept reasonably small. 
			//Double it w/ each error, up to one megabyte
			if (_watcher.InternalBufferSize < MAX_BUFFER_SIZE) {
				_watcher.InternalBufferSize *= 2;
			}
		}

		/// <summary>
		/// Fires the OnFileSystemChanged event.  Since the internal
		/// FileSystemWatcher buffer for change notifications is very limited,
		/// and it only freed when the change event is processed, report the
		/// change to our consumers on another thread, to ensure that the
		/// FileSystemWatcher receives control of this thread back as soon
		/// as possible. 
		/// </summary>
		/// <param name="e"></param>
		private void OnFileSystemChanged(FileSystemChangeEventArgs e) {
			//TODO: Use another thread
			if (FileSystemChanged != null) {
				FileSystemChanged(this, e);
			}
		}
	}
}
