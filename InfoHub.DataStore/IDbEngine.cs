using System;

namespace InfoHub.DataStore
{
	/// <summary>
	/// Singleton interface representing an OODBMS engine
	/// </summary>
	public interface IDbEngine
	{
		/// <summary>
		/// Opens an existing data store and returns an IDataStore instance to operate
		/// on the data store.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		IDataStore Open(String fileName);

		/// <summary>
		/// Creates a new data store, failing if the data store exists
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		IDataStore Create(String fileName);

		/// <summary>
		/// Creates a new data store, optionally replacing an existing data store if present
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		IDataStore Create(String fileName, bool overwrite);

		/// <summary>
		/// Destroys a previously-created data store (assuming it's not open)
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		void Destroy(String fileName);
	}
}
