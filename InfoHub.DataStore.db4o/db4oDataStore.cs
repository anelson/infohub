using System;
using System.IO;
using System.Reflection;

using InfoHub.Common;
using InfoHub.ContentModel;
using InfoHub.DataStore;

using com.db4o;
using com.db4o.ext;
using com.db4o.query;

namespace InfoHub.DataStore.db4o
{
	/// <summary>
	/// Implementation of IDataStore that uses a db4o ObjectContainer as the back-end
	/// </summary>
	internal class db4oDataStore : IDataStore, ICollectionFactory
	{
		db4oDbEngine _engine;
		ExtObjectContainer _container;
		String _path;
		LoggerHelper _logger;

		public db4oDataStore(db4oDbEngine engine, ExtObjectContainer container, String path, ILoggerFactory factory) {
			_engine = engine;
			_container = container;
			_path = path;

			_logger = new LoggerHelper(factory.GetLogger(typeof(db4oDataStore)), Assembly.GetExecutingAssembly(), "StringConstants");

			_logger.Debug("LogMsg.DataStoreConstructed", path);
		}

		/// <summary>
		/// Checks if a given object is contained in this data store
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool IsObjectInDataStore(IContentObject obj) {
			return _container.isStored(obj);
		}

		/// <summary>
		/// Provides access to the db4o ObjectContainer underlying the data store object, for
		/// use by other classes in this package only
		/// </summary>
		public ExtObjectContainer ObjectContainer {
			get {
				return _container;
			}
		}

		#region IDisposable Members

		public void Dispose() {
			if (_container != null) {
				_logger.Debug("LogMsg.DataStoreClosing", _path);
				_container.close();
				_container = null;

				//Inform the dbengine instance
				_engine.OnDataStoreClosed(this);
				_logger.Debug("LogMsg.DataStoreClosed", _path);
			}
		}

		#endregion

		#region IDataStore Members

		public object BeginTransaction() {
			//In db4o, transactions are automatically started, and there is no 
			//transaction object
			_logger.Debug("LogMsg.BeginningTransaction", _path);
			return new Object();
		}

		public string Path {
			get {
				return _path;
			}
		}

		public void CommitTransaction(object txn) {
			_logger.Debug("LogMsg.CommittingTransaction", _path);
			_container.commit();
			_logger.Debug("LogMsg.CommittedTransaction", _path);
		}

		public void RollbackTransaction(object txn) {
			_logger.Debug("LogMsg.RollingBackTransaction", _path);
			_container.rollback();
			_logger.Debug("LogMsg.RolledBackTransaction", _path);
		}

		public System.Collections.IEnumerable RootFolders {
			get {
				//Query all of the IRootFolders objects in the store
				_logger.Debug("LogMsg.EnumeratingRootFolders", _path);
				Query qry = _container.query();

				qry.constrain(typeof(IRootFolder));

				ObjectSet os = qry.execute();
				_logger.Debug("LogMsg.EnumeratedRootFolders", _path, os.size());
				return new ObjectSetEnumerable(qry.execute());
			}
		}
		
		public IFileSystemObject GetFileSystemObjectByPath(IRootFolder root, String path) {
			//Search for an IFileSystemObject by path
			_logger.Debug("LogMsg.GettingFsoByPath", _path, root.Source, path);
			Query qry = _container.query();

			qry.constrain(typeof(IFileSystemObject));
			qry.descend("Path").constrain(path);
			qry.descend("RootFolder").constrain(root);

			ObjectSet set = qry.execute();
			if (set.hasNext()) {
				_logger.Debug("LogMsg.FoundFsoByPath", _path);
				return (IFileSystemObject)set.next();
			} else {
				_logger.Debug("LogMsg.NotFoundFsoByPath", _path);
				return null;
			}
		}

		#endregion

		#region IObjectPersistor Members

		public ICollectionFactory CollectionFactory {
			get {
				return this;
			}
		}

		public bool IsActivated(IPersistenceBoundary obj) {
			return false;
		}

		public void Deactivate(IPersistenceBoundary obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeDeactivate();
			}

			if (callback != null) {
				callback.AfterDeactivate();
			}
		}

		public void Delete(IPersistenceBoundary obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeDelete();
			}

			if (callback != null) {
				callback.AfterDelete();
			}
		}

		public void Refresh(IPersistenceBoundary obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeRefresh();
			}

			if (callback != null) {
				callback.AfterRefresh();
			}
		}

		public void Update(IPersistenceBoundary obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeUpdate();
			}

			if (callback != null) {
				callback.AfterUpdate();
			}
		}

		public void Add(IPersistenceBoundary obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeAdd();
			}

			if (callback != null) {
				callback.AfterAdd();
			}
		}

		public void Activate(IPersistenceBoundary obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeActivate();
			}

			if (callback != null) {
				callback.AfterActivate();
			}
		}

		#endregion

		#region ICollectionFactory Members

		public System.Collections.IDictionary CreateDictionary(IContentObject owner) {
			return _container.collections().newHashMap(5);
		}

		public System.Collections.IList CreateList(IContentObject owner) {
			return _container.collections().newLinkedList();
		}

		#endregion
	}
}
