using System;
using System.Collections;

using InfoHub.ContentModel;

namespace InfoHub.DataStore
{
	/// <summary>
	/// Represents an instance of an object data store
	/// </summary>
	public interface IDataStore : IDisposable, IObjectPersistor
	{
		/// <summary>
		/// The path and file name of the data store
		/// </summary>
		String Path {get;}

		IEnumerable RootFolders {get;}

		IFileSystemObject GetFileSystemObjectByPath(IRootFolder root, String path);

		Object BeginTransaction();
		void CommitTransaction(Object txn);
		void RollbackTransaction(Object txn);
	}
}
