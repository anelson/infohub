using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Interface implemented by the underlying data store to allow content model objects to
	/// dynamically load (activate), unload (deactivate), add, update, and delete those content model objects that
	/// form persistence boundaries by implementing IPersistenceBoundary
	/// </summary>
	public interface IObjectPersistor {
		ICollectionFactory CollectionFactory {get;}

		bool IsActivated(IPersistenceBoundary obj);

		void Activate(IPersistenceBoundary obj);
		void Deactivate(IPersistenceBoundary obj);
		void Add(IPersistenceBoundary obj);
		void Update(IPersistenceBoundary obj);
		void Delete(IPersistenceBoundary obj);
		void Refresh(IPersistenceBoundary obj);

		/// Deletes an object previously stored within a persistence boundary
		void DeleteObject(Object obj);
		/// Deletes an object previously stored within a persistence boundary, 
		/// Optionally recursively deletes all objects referenced by the
		/// specified object, up to (but not including) a persistence boundary
		void DeleteObject(Object obj, bool recurse);
	}
}
