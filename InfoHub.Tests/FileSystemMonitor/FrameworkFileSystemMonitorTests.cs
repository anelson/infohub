using System;
using System.IO;

using InfoHub.Common;
using InfoHub.FileSystemMonitor;

using NUnit.Framework;

namespace InfoHub.Tests.FileSystemMonitor
{
	[TestFixture]
	/// <summary>
	/// Test fixture containing unit tests for the FrameworkFileSystemMonitor
	/// </summary>
	public class FrameworkFileSystemMonitorTests : TestBase
	{
		private FrameworkFileSystemMonitor _monitor;
		private bool _fsChangedFired;
		private int _fsChangedFireCount;
		private ChangeType _lastChangeType;
		private String _lastNewPath;
		private String _lastOldPath;
		private String _scratchPath;

		public FrameworkFileSystemMonitorTests()
		{
		}

		[TestFixtureSetUp]
		public void CreateScratchPath() {
			//Create a scratch area in the Temporary folder in which
			//to perform these filesystem tests
			_scratchPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FSMonTest");

			//delete this folder if it exists
			if (Directory.Exists(_scratchPath)) {
				Directory.Delete(_scratchPath, true);
			}

			//And create it
			Directory.CreateDirectory(_scratchPath);
		}

		[TestFixtureTearDown]
		public void ClearScratchPath() {
			//Delete the scratch path folder
			if (Directory.Exists(_scratchPath)) {
				Directory.Delete(_scratchPath, true);
			}
		}

		[SetUp]
		public void CreateMonitor() {
			_monitor = new FrameworkFileSystemMonitor((ILoggerFactory)AppContext.GetObject("LoggerFactory", typeof(ILoggerFactory)));
			Assert.IsNotNull(_monitor);
			
			_monitor.FileSystemChanged += new FileSystemChangeEventHandler(_monitor_FileSystemChanged);

			_fsChangedFired = false;
			_fsChangedFireCount = 0;
			_lastChangeType = ChangeType.Changed;
			_lastNewPath = null;
			_lastOldPath = null;
		}

		[TearDown]
		public void DisposeMonitor() {
			_monitor.Dispose();
			_monitor = null;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullPathTest() {
			//Setting a null path should throw an argument exception
			_monitor.Path = null;
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void WatchNonexistentFileForCreation() {
			//Path must be the name of a directory that exists.
			_monitor.Path = Path.Combine(_scratchPath, "idontexist");
		}

		[Test]
		public void DetectFileCreation() {
			//Watch a folder, and report the creation of a file therein
			String filePath = Path.Combine(_scratchPath, "created.txt");

			_monitor.Path = _scratchPath;

			Assert.AreEqual(_scratchPath, _monitor.Path);
			Assert.IsFalse(_fsChangedFired);

			//Create an empty file
			File.CreateText(filePath).Close();

			//Change notification comes on another thread; depending upon system load,
			//wait as long as a few seconds
			Assert.IsTrue(WaitForFsChangedFired());
			Assert.AreEqual(1, _fsChangedFireCount);
			Assert.AreEqual(ChangeType.Created, _lastChangeType);
			Assert.AreEqual(filePath, _lastNewPath);
			Assert.AreEqual(filePath, _lastOldPath);
		}

		[Test]
		public void DetectFileModification() {
			//Watch a folder, and report the modification of a file therin
			String filePath = Path.Combine(_scratchPath, "changed.txt");

			//Create a file before starting the monitor, so it doesn't pick up 
			//the creation
			File.CreateText(filePath).Close();

			_monitor.Path = _scratchPath;

			Assert.IsFalse(_fsChangedFired);

			//Append something to the file
			using (FileStream fs = File.OpenWrite(filePath)) {
				byte[] data = System.Text.Encoding.ASCII.GetBytes("foo bar baz\r\n");
				fs.Write(data, 0, data.Length);
			}

			//Change notification comes on another thread; depending upon system load,
			//wait as long as a few seconds
			Assert.IsTrue(WaitForFsChangedFired());
			Assert.AreEqual(1, _fsChangedFireCount);
			Assert.AreEqual(ChangeType.Changed, _lastChangeType);
			Assert.AreEqual(filePath, _lastNewPath);
			Assert.AreEqual(filePath, _lastOldPath);
		}

		[Test]
		public void DetectFileDeletion() {
			//Watch a folder, and report the deletion of a file therin
			String filePath = Path.Combine(_scratchPath, "deleted.txt");

			//Create a file before starting the monitor, so it doesn't pick up 
			//the creation
			File.CreateText(filePath).Close();

			_monitor.Path = _scratchPath;

			Assert.IsFalse(_fsChangedFired);

			//Delete the file
			File.Delete(filePath);

			//Change notification comes on another thread; depending upon system load,
			//wait as long as a few seconds
			Assert.IsTrue(WaitForFsChangedFired());
			Assert.AreEqual(1, _fsChangedFireCount);
			Assert.AreEqual(ChangeType.Deleted, _lastChangeType);
			Assert.AreEqual(filePath, _lastNewPath);
			Assert.AreEqual(filePath, _lastOldPath);
		}

		[Test]
		public void DetectFileRename() {
			//Watch a folder, and report the deletion of a file therin
			String filePath = Path.Combine(_scratchPath, "oldname.txt");
			String newFilePath = Path.Combine(_scratchPath, "newname.txt");

			//Create a file before starting the monitor, so it doesn't pick up 
			//the creation
			File.CreateText(filePath).Close();

			_monitor.Path = _scratchPath;

			Assert.IsFalse(_fsChangedFired);

			//Rename the file
			File.Move(filePath, newFilePath);

			//Change notification comes on another thread; depending upon system load,
			//wait as long as a few seconds
			Assert.IsTrue(WaitForFsChangedFired());
			Assert.AreEqual(1, _fsChangedFireCount);
			Assert.AreEqual(ChangeType.Renamed, _lastChangeType);
			Assert.AreEqual(newFilePath, _lastNewPath);
			Assert.AreEqual(filePath, _lastOldPath);
		}

		/// <summary>
		/// Waits for a few seconds for _fsChangedFired to go to true.
		/// </summary>
		/// <returns>true if _fsChangedFired goes true within a few seconds, else false</returns>
		private bool WaitForFsChangedFired() {
			int patience = 10*1000;

			while (patience > 0) {
				if (_fsChangedFired) {
					return true;
				}

				System.Threading.Thread.Sleep(100);
				patience -= 100;
			}

			return false;
		}

		private void _monitor_FileSystemChanged(object sender, FileSystemChangeEventArgs e) {
			_fsChangedFired = true;
			_fsChangedFireCount++;
			_lastChangeType = e.ChangeType;
			_lastNewPath = e.NewPath;
			_lastOldPath = e.OldPath;
		}
	}
}
