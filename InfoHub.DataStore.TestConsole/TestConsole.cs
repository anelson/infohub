using System;
using System.Configuration;
using System.IO;
using System.Reflection;

using Spring.Context;

using InfoHub.Common;
using InfoHub.ContentModel;
using InfoHub.ContentModel.Text;
using InfoHub.DataStore;
using InfoHub.DataStore.db4o;

namespace InfoHub.DataStore.TestConsole
{
	/// <summary>
	/// Test console for testing a datastore implementation for performance, functionality, etc
	/// </summary>
	class TestConsole
	{
		IDbEngine _engine;
		IDataStore _store;
		LoggerHelper _logger;

		const String TEST_DATA_PATH=@"n:\moby";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			AppContext.InitializeAppContext();
			IDbEngine engine = (IDbEngine)AppContext.Ctx.GetObject("DbEngine", typeof(IDbEngine));
			ILoggerFactory loggerFactory = (ILoggerFactory)AppContext.Ctx.GetObject("LoggerFactory", typeof(ILoggerFactory));

			TestConsole tc = new TestConsole(engine, loggerFactory);
			tc.RunTests();
		}

		private TestConsole(IDbEngine engine, ILoggerFactory loggerFactory) {
			_engine = engine;
			_logger = new LoggerHelper(loggerFactory.GetLogger(typeof(TestConsole)), 
				Assembly.GetExecutingAssembly(),
				"StringConstants");
		}

		private void RunTests() {
			//Populate a bunch of ContentModel objects from the filesystem, then do some querying
			_logger.Info("LogMsg.CreatingDataStore");

			using (_store = _engine.Create("test.store", true)) {
				LoadRootFolder(TEST_DATA_PATH);
			}
		}

		private void LoadRootFolder(String path) {
			//Creates a IRootFolder for this folder, and populates it with all child objects
			_logger.Info("LogMsg.LoadingRootFolder", path);

			IRootFolder root = new GenericRootFolder("[root]", _store);

			Object txn = _store.BeginTransaction();

			_store.Add(root);

			LoadFolderInt(root, path);

			//Update to reflect the new contents
			_store.Update(root);

			_store.CommitTransaction(txn);
		}

		private void LoadFile(IFolder parentFolder, String path, String fileName) {
			_logger.Info("LogMsg.LoadingFile", path, fileName);

			PlainTextDocument doc = new PlainTextDocument(parentFolder, fileName, "text/plain");

			_store.Add(doc);

			using (StreamReader sr = File.OpenText(Path.Combine(path, fileName))) {
				String line = sr.ReadLine();

				if (line != null) {
					TextLine tl = new TextLine(doc);

					TextBlock tb = new TextBlock(tl);
					tb.Text = line;
				}
			}

			//Update to reflect the new contents
			_store.Update(doc);
		}

		private void LoadFolder(IFolder parentFolder, String path, String folderName) {
			_logger.Info("LogMsg.LoadingFolder", path, folderName);

			GenericFolder folder = new GenericFolder(parentFolder, folderName);

			_store.Add(folder);

			//Load child folders and files
			LoadFolderInt(folder, Path.Combine(path, folderName));

			//Update to reflect the new contents
			_store.Update(folder);
		}

		private void LoadFolderInt(IFolder folderToLoad, String folderPath) {
			//Given an alread-created folder object, loads it
			foreach (String file in Directory.GetFiles(folderPath, "*.*")) {
				LoadFile(folderToLoad, folderPath, file);
			}

			foreach (String folder in Directory.GetDirectories(folderPath)) {
				LoadFolder(folderToLoad, folderPath, folder);
			}
		}
	}
}
