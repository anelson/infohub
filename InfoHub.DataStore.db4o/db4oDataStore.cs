using System;
using System.Collections;
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
		db4oTypeProcessor _typeProcessor;
		ObjectRecursor _recursor;

		public db4oDataStore(db4oDbEngine engine, ExtObjectContainer container, String path, ILoggerFactory factory) {
			_engine = engine;
			_container = container;
			_path = path;

			_typeProcessor = new db4oTypeProcessor(this);

			//Here at load time, process the type information for all the types currently
			//stored in the container
			foreach (StoredClass sc in _container.storedClasses()) {
				Type storedType = Type.GetType(sc.getName());

				if (storedType != null) {
					if (!_typeProcessor.IsAssemblyProcessed(storedType.Assembly)) {
						_typeProcessor.ProcessAssembly(storedType.Assembly);
					}
				}
			}			

			_recursor = new ObjectRecursor(this);

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
				IFileSystemObject fso = (IFileSystemObject)set.next();

				fso.Activate();

				return fso;
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
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}
			
			//If this object's type hasn't been processed already, process it now
			if (!_typeProcessor.IsAssemblyProcessed(obj.GetType().Assembly)) {
				_typeProcessor.ProcessAssembly(obj.GetType().Assembly);
			}

			return _container.isActive(obj);
		}

		public void Deactivate(IPersistenceBoundary obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}
			
			//If this object's type hasn't been processed already, process it now
			if (!_typeProcessor.IsAssemblyProcessed(obj.GetType().Assembly)) {
				_typeProcessor.ProcessAssembly(obj.GetType().Assembly);
			}

			//If this object is active, deactivate it, calling its callback 
			//methods if present
			if (IsActivated(obj)) {
				_recursor.RecursivelyDo(_recursor.RecursePersistenceBoundaryGraph(obj), 
										null, 
										new RecursiveOperationDelegate(DoDeactivateObject));
			}
		}

		public void Delete(IPersistenceBoundary obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}
			
			//If this object's type hasn't been processed already, process it now
			if (!_typeProcessor.IsAssemblyProcessed(obj.GetType().Assembly)) {
				_typeProcessor.ProcessAssembly(obj.GetType().Assembly);
			}

			//If this object is stored, delete it from the store, calling
			//its callback methods if present
			if (_container.isStored(obj)) {
				_recursor.RecursivelyDo(_recursor.RecursePersistenceBoundaryGraph(obj), 
										null, 
										new RecursiveOperationDelegate(DoDeleteObject));
			}
		}

		public void Refresh(IPersistenceBoundary obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}
			
			//If this object's type hasn't been processed already, process it now
			if (!_typeProcessor.IsAssemblyProcessed(obj.GetType().Assembly)) {
				_typeProcessor.ProcessAssembly(obj.GetType().Assembly);
			}

			//If this object is active, refresh it from the store, calling
			//its callback methods if present.  If it's not active, calling
			//Refresh is bogus
			if (!IsActivated(obj)) {
				throw new InvalidOperationException();
			}
			
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			_recursor.RecursivelyDo(_recursor.RecursePersistenceBoundaryGraph(obj), 
									null, 
									new RecursiveOperationDelegate(DoRefreshObject));
		}

		public void Update(IPersistenceBoundary obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}
			
			//If this object's type hasn't been processed already, process it now
			if (!_typeProcessor.IsAssemblyProcessed(obj.GetType().Assembly)) {
				_typeProcessor.ProcessAssembly(obj.GetType().Assembly);
			}

			//If this object is active, update it in the store, calling
			//its callback methods if present.  If it's not active, calling
			//Update is bogus
			if (!IsActivated(obj)) {
				throw new InvalidOperationException();
			}

			_recursor.RecursivelyDo(_recursor.RecursePersistenceBoundaryGraph(obj), 
									null, 
									new RecursiveOperationDelegate(DoUpdateObject));
		}

		public void Add(IPersistenceBoundary obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}
			
			//If this object's type hasn't been processed already, process it now
			if (!_typeProcessor.IsAssemblyProcessed(obj.GetType().Assembly)) {
				_typeProcessor.ProcessAssembly(obj.GetType().Assembly);
			}
			
			//Add the object without regard for whether it's currently in the store/
			//in db4o, an add and an update are the same operation (set()), therefore
			//it is not important to strictly distinguish adds from updates, and
			//its possible that some Content Model slieght-of-hand caused the persistence boundary
			//to be added, without the recursive add of referenced objects that this method
			//does			
			_recursor.RecursivelyDo(_recursor.RecursePersistenceBoundaryGraph(obj), 
									null, 
									new RecursiveOperationDelegate(DoAddObject));
		}

		public void Activate(IPersistenceBoundary obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}
			
			//If this object's type hasn't been processed already, process it now
			if (!_typeProcessor.IsAssemblyProcessed(obj.GetType().Assembly)) {
				_typeProcessor.ProcessAssembly(obj.GetType().Assembly);
			}

			//If this type is stored and not activated, activate it
			//If it is activated, you can't double-activate
			if (IsActivated(obj)) {
				throw new InvalidOperationException();
			}

			_recursor.RecursivelyDo(_recursor.RecursePersistenceBoundaryGraph(obj), 
									null, 
									new RecursiveOperationDelegate(DoActivateObject));
		}
		
		
		public void DeleteObject(Object obj) {
			DeleteObject(obj, false);
		}
		
		public void DeleteObject(Object obj, bool recurse) {
			if (_container.isStored(obj)) {
				if (!recurse) {
					DoDeleteObject(null,  obj);
				} else {
					_recursor.RecursivelyDo(_recursor.RecurseObjectGraph(obj), 
											null, 
											new RecursiveOperationDelegate(DoDeleteObject));
				}
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

        /// <summary>An implementation of the RecursiveOperationDelegate that activates a single
        ///     object.</summary>
        /// 
        /// <param name="cookie"></param>
        /// <param name="obj"></param>
        /// 
        /// <returns></returns>
		private bool DoActivateObject(Object cookie, Object obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeActivate();
			}

			_container.activate(obj, 1);

			if (callback != null) {
				callback.AfterActivate();
			}
			return true;
		}

        /// <summary>An implementation of the RecursiveOperationDelegate that deactivates a single
        ///     object.</summary>
        /// 
        /// <param name="cookie"></param>
        /// <param name="obj"></param>
        /// 
        /// <returns></returns>
		private bool DoDeactivateObject(Object cookie, Object obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeDeactivate();
			}

			_container.deactivate(obj, 1);

			if (callback != null) {
				callback.AfterDeactivate();
			}
			return true;
		}

        /// <summary>An implementation of the RecursiveOperationDelegate that refreshes a single
        ///     object.</summary>
        /// 
        /// <param name="cookie"></param>
        /// <param name="obj"></param>
        /// 
        /// <returns></returns>
		private bool DoRefreshObject(Object cookie, Object obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeRefresh();
			}

			_container.refresh(obj, 1);

			if (callback != null) {
				callback.AfterRefresh();
			}
			return true;
		}

        /// <summary>An implementation of the RecursiveOperationDelegate that adds a single
        ///     object.</summary>
        /// 
        /// <param name="cookie"></param>
        /// <param name="obj"></param>
        /// 
        /// <returns></returns>
		private bool DoAddObject(Object cookie, Object obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeAdd();
			}

			_container.set(obj, 1);

			if (callback != null) {
				callback.AfterAdd();
			}
			return true;
		}

        /// <summary>An implementation of the RecursiveOperationDelegate that updates a single
        ///     object.</summary>
        /// 
        /// <param name="cookie"></param>
        /// <param name="obj"></param>
        /// 
        /// <returns></returns>
		private bool DoUpdateObject(Object cookie, Object obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeUpdate();
			}

			_container.set(obj, 1);

			if (callback != null) {
				callback.AfterUpdate();
			}
			return true;
		}

        /// <summary>An implementation of the RecursiveOperationDelegate that deletes a single
        ///     object.</summary>
        /// 
        /// <param name="cookie"></param>
        /// <param name="obj"></param>
        /// 
        /// <returns></returns>
		private bool DoDeleteObject(Object cookie, Object obj) {
			IPersistenceNotificationCallback callback = obj as IPersistenceNotificationCallback;

			if (callback != null) {
				callback.BeforeDelete();
			}

			_container.delete(obj);

			if (callback != null) {
				callback.AfterDelete();
			}
			
			return true;
		}
	}
}
