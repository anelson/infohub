using System;
using System.Collections;
using System.IO;
using System.Reflection;

using InfoHub.Common;
using InfoHub.ContentModel;
using InfoHub.ContentModel.Attributes;
using InfoHub.DataStore;

using com.db4o;

using Spring.Context;

namespace InfoHub.DataStore.db4o
{
	/// <summary>
	/// Implementation of IDbEngine using the db4o object database
	/// </summary>
	public class db4oDbEngine : IDbEngine
	{
		ILoggerFactory _logFactory;
		LoggerHelper _logger;
		IList _dataStores;

		public db4oDbEngine(ILoggerFactory factory)
		{
			_logFactory = factory;
			_logger = new LoggerHelper(factory.GetLogger(typeof(db4oDbEngine)), 
				Assembly.GetExecutingAssembly(),
				"StringConstants");
			_dataStores = new ArrayList();

			_logger.Info("LogMsg.Initializing", Db4o.version());

			//Set initial container configuration
			//activation depth is 1 (meaining primitive types only), since
			//this assembly handles activation of all objects in a persistence
			//boundary manually.
			Db4o.configure().activationDepth(1);

			//The content model objects do not know about db4o, therefore
			//they can't possibly implement the callback interface.  Thus, save
			//some compute cycles and don't even check
			Db4o.configure().callbacks(false);

			//Don't call constructors when instantiating objects
			Db4o.configure().callConstructors(false);

			//Throw an exception when storing a type that is not storable
			//Usually this is due to a missing default ctor, but attempting to
			//save event objects also causes this problem
			Db4o.configure().exceptionsOnNotStorable(true);

			//Tell db4o that the NotPersisted attribute decorates those
			//fields that shouldn't be stored
			Db4o.configure().markTransient(typeof(NotPersistedAttribute).FullName);

			//Create a logger object for db4o's log messages, then use 
			//a db4oPrintStreamLogger to adapt the logger object's interface
			//to the PrintStream that db4o expects.

			ILogger db4oLogger = factory.GetLogger(typeof(db4oPrintStreamLogger));
			db4oPrintStreamLogger ps = new db4oPrintStreamLogger(db4oLogger);
			Db4o.configure().setOut(ps);
			
			//Based on the log levels of the logger object, set db4o's log level
			if (db4oLogger.IsDebugEnabled) {
				//Debug level, so enable all the log messages
				Db4o.configure().messageLevel(3);				
			} else if (db4oLogger.IsInfoEnabled) {
				//Include new/update/delete msgs, but no activation details
				Db4o.configure().messageLevel(2);				
			} else {
				//No extra logging.  Just report open and close
				Db4o.configure().messageLevel(1);
			}

			_logger.Info("LogMsg.Initialized");
		}

		#region IDbEngine Members

		public void Destroy(string fileName) {
			if (fileName == null) {
				throw new ArgumentNullException("fileName");
			}

			_logger.Info("LogMsg.Destroying", fileName);

			if (File.Exists(fileName)) {
				File.Delete(fileName);
			}
		}

		public IDataStore Create(string fileName) {
			return Create(fileName, false);
		}

		public IDataStore Create(String fileName, bool overwrite) {
			//Does the data store already exixt?
			if (File.Exists(fileName)) {
				//Delete if overwrite is true, else throw an excepttion
				if (overwrite) {
					File.Delete(fileName);
				} else {
					throw new InvalidOperationException();
				}
			}

			//In db4o, opening a non-existant file creates it
			return Open(fileName);
		}

		public IDataStore Open(string fileName) {
			_logger.Info("LogMsg.Opening", fileName);
			db4oDataStore dataStore = new db4oDataStore(this, Db4o.openFile(fileName).ext(), fileName, _logFactory);

			//Keep track of this data store in the list of active stores
			_dataStores.Add(dataStore);

			return dataStore;
		}

		#endregion

		/// <summary>
		/// Given a content object stored on one of the active db4o data stores, determines which
		/// data store.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>The data store containing obj, or null if obj is not in any active data store</returns>
		public IDataStore GetDataStoreForObject(IContentObject obj) {
			return GetDataStoreForObject(obj, false);
		}

		/// <summary>
		/// Given a content object stored on one of the active db4o data stores, determines which
		/// data store.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="recursive">true if the object's parent should be consulted recursively
		/// until an object stored in one of the active data stores is found; default is false</param>
		/// <returns>The data store containing obj, or null if obj is not in any active data store</returns>
		public IDataStore GetDataStoreForObject(IContentObject obj, bool recursive) {
			db4oDataStore ds = null;

			do {
				//Check all active data stores for one containing obj
				foreach (db4oDataStore tempDs in _dataStores) {
					if (tempDs.IsObjectInDataStore(obj)) {
						ds = ds;
						break;
					}
				}

				if (ds == null) {
					//None found; check obj's parent
					obj = obj.Parent;
				}
			} while (ds == null && obj != null);

			//Walked the entire ancestry line up to the root, and still couldn't find
			//an active data store containing the object
			return null;
		}

		/// <summary>
		/// Callback called by a db4oDataStore after it closes the db4o database file.
		/// 
		/// Removes the data store from the list of active stores.
		/// </summary>
		/// <param name="dataStore"></param>
		internal void OnDataStoreClosed(db4oDataStore dataStore) {
			_dataStores.Remove(dataStore);
		}
	}
}
