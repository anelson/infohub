using System;
using System.Collections;
using System.Reflection;

using InfoHub.ContentModel;
using InfoHub.ContentModel.Attributes;

namespace InfoHub.DataStore.db4o
{
	/// <summary>
	/// Delegate called by ObjectRecursor once for each object in the object graph it
	/// traverses.
	/// </summary>
	internal delegate bool ProcessObjectFilterDelegate(Object cookie, FieldInfo field, Object obj, Object parent);

	
    /// <summary>Delegate called by RecursivelyDo once for each object in the collection.
    /// 
    ///     Returns false to cancel doing.</summary>
	internal delegate bool RecursiveOperationDelegate(Object cookie, Object obj);

	/// <summary>
	/// A utility class that provides a generic recursion function that
	/// recursively traverses an object graph performing a specific operation
	/// on each object.
	/// </summary>
	internal class ObjectRecursor
	{
		db4oDataStore _store;

		public ObjectRecursor(db4oDataStore dataStore)
		{
			_store = dataStore;
		}

        /// <summary>Traverses the object graph rooted at <c>obj</c> by reflecting over the objects in the object's fields,
        ///     their fields, etc.  For each object visited in the graph, calls objFilter.  If objFilter
        ///     returns false, the object passed to objFilter isn't recursed, else it is.
        /// 
        ///     If a non-null value is provided for <c>parent</c>, any references to the <c>parent</c> object are
        ///     ignored and are neither traversed nor passed to objFilter.
        /// 
        ///     <c>cookie</c> is a caller-defined state object that is passed to objFilter directly.
        /// </summary>
        /// 
        /// <param name="cookie"></param>
        /// <param name="obj"></param>
        /// <param name="parent"></param>
        /// <param name="objFilter"></param>
        /// 
        /// <returns>A list of all the objects in the graph, not including those that were excluded
        ///     by objFilter.  This list will not contain duplicates, even if an object
        ///     was encountered multiple times during the recursion.</returns>
		public ICollection RecurseObjectGraph(Object cookie, Object obj, Object parent, ProcessObjectFilterDelegate objFilter) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}

			if (objFilter == null) {
				throw new ArgumentNullException("obj");
			}

			//Start the list of objects
			ArrayList objList = new ArrayList();

			RecurseObjectGraphInternal(objList, cookie, null, obj, parent, objFilter);

			return objList;
		}

        /// <summary>Recurses the objects within a persistence boundary, excluding objects
        ///     decorated with NotPersisted and the persistence boundary object's parent
        ///     object.</summary>
        /// 
        /// <param name="obj"></param>
        /// 
        /// <returns></returns>
		public ICollection RecursePersistenceBoundaryGraph(IPersistenceBoundary obj) {
			return RecurseObjectGraph(obj, 
									  obj, 
									  null, 
									  new ProcessObjectFilterDelegate(PersistenceBoundaryObjectFilter));
		}

        /// <summary>Does a boundless recursion of an arbitrary object.  Stops at persistence boundaries
        ///     and NotPersisted fields just like RecursePersistenceBoundaryGraph.</summary>
        /// 
        /// <param name="obj"></param>
        /// 
        /// <returns></returns>
		public ICollection RecurseObjectGraph(Object obj) {
			return RecurseObjectGraph(null, 
									  obj, 
									  null, 
									  new ProcessObjectFilterDelegate(PersistenceBoundaryObjectFilter));
		}

        /// <summary>Using a collection obtained from one of the Recurse* methods, calls a delegate
        ///     once for each object in the collection, stopping if the delegate returns false
        ///     or the entire collection is processed.</summary>
        /// 
        /// <param name="coll"></param>
        /// <param name="cookie"></param>
        /// <param name="doer"></param>
		public void RecursivelyDo(ICollection coll, Object cookie, RecursiveOperationDelegate doer) {
			if (coll == null) {
				throw new ArgumentNullException("coll");
			}
			
			if (doer == null) {
				throw new ArgumentNullException("doer");
			}

			foreach (Object obj in coll) {
				if (!doer(cookie, obj)) {
					break;
				}
			}
		}

		
        /// <summary>Internal recursive function that actually does the recursion</summary>
        /// 
        /// <param name="cookie"></param>
        /// <param name="field"></param>
        /// <param name="obj"></param>
        /// <param name="parent"></param>
        /// <param name="objFilter"></param>
		private void RecurseObjectGraphInternal(IList objList, Object cookie, FieldInfo field, Object obj, Object parent, ProcessObjectFilterDelegate objFilter) {
			//First, pass this object through the object processor.  If it indicates
			//not to recurse further, stop now
			if (!objFilter(cookie, field, obj, parent)) {
				return;
			}

			//Add this object to the list of objects that have been processed
			objList.Add(obj);

			//Get all the fields for this type, and for each one, process its object graph
			foreach (FieldInfo fi in GetTypeFields(obj.GetType())) {
				//Process the value of the field
				RecurseFieldValue(objList, 
										   cookie, 
										   fi, 
										   fi.GetValue(obj), 
										   obj, 
										   parent,
										   objFilter);
			}

			if (obj is Array) {
				//An array object.  Process each of its elements as well
				foreach (Object arrayObj in (Array)obj) {
				RecurseFieldValue(objList, 
										   cookie, 
										   field, 
										   arrayObj, 
										   obj, 
										   parent,
										   objFilter);
				}
			}
		}

        /// <summary>Given a type, returns a collection of FieldInfo objects for the fields contained
        ///     by the type and all base types.</summary>
        /// 
        /// <param name="type"></param>
        /// 
        /// <returns></returns>
		private ICollection GetTypeFields(Type type) {
			ArrayList al = new ArrayList();

			al.AddRange(type.GetFields(BindingFlags.FlattenHierarchy | 
									   BindingFlags.GetField | 
									   BindingFlags.Public | 
									   BindingFlags.NonPublic | 
									   BindingFlags.Instance));

			return al;
		}

		/// Encapsulates the logic to recurse the (potentially null) value of a field
		private void RecurseFieldValue(IList objList, Object cookie, FieldInfo field, Object fieldValue, Object obj, Object parent, ProcessObjectFilterDelegate objFilter) {
			//If the field value is null, ignore it
			if (fieldValue == null) {
				return;
			}

			//If this is the current object's parent, ignore it, since it will
			//already have been processed
			if (fieldValue == parent) {
				return;
			}

			//If this is already in the list of objects in the graph, don't process
			//it again
			if (objList.Contains(fieldValue)) {
				return;
			}

			//Process this object's graph
			RecurseObjectGraphInternal(objList, 
									   cookie, 
									   field, 
									   fieldValue, 
									   obj, 
									   objFilter);
		}

        /// <summary>A ProcessObjectFilterDelegate implementation that filters out other persistence
        ///     boundaries (apart from the first one), fields marked with the NotPersisted attribute,
        ///     and the parent of the persistence boundary object.</summary>
        /// 
        /// <param name="cookie">The delegate cookie.  In this case, it is the persistence boundary object being
        ///     recursed.</param>
        /// <param name="field"></param>
        /// <param name="obj"></param>
        /// <param name="parent"></param>
        /// 
        /// <returns>true to continue recursing at this object; false to ignore it</returns>
		private bool PersistenceBoundaryObjectFilter(Object cookie, FieldInfo field, Object obj, Object parent) {
			//Is obj a persistence boundary, and not the persistence boundary we're traversing?
			if (obj is IPersistenceBoundary &&
				obj != cookie) {
				return false;
			}

			if (field != null) {
				//If the attributes on the field the object came from include NotPersisted, skip
				Object[] attrs = field.GetCustomAttributes(typeof(NotPersistedAttribute), true);

				if (attrs != null) {
					return false;
				}
			}

			//Else, seems ok
			return true;
		}
	}
}
