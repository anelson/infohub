using System;
using System.Collections;
using System.IO;
using System.Reflection;

using InfoHub.Common;
using InfoHub.ContentModel;
using InfoHub.ContentModel.Attributes;
using InfoHub.DataStore;

using com.db4o;
using com.db4o.config;
using com.db4o.ext;
using com.db4o.query;


namespace InfoHub.DataStore.db4o
{
	/// <summary>
	/// Helper class used by db4oDataStore to read ContentModel type information from
	/// an assembly and apply the information to the db4o database object to control
	/// the way it operates on the objects in the assembly.
	/// 
	/// By using attributes like <code>NotPersistedAttribute</code> and implementing
	/// the <code>IPersistenceBoundary</code> interface, content model objects describe
	/// how segments of their object graphs should be processed.  This class extracts 
	/// that information using reflection, and applies it to the db4o container.
	/// </summary>
	internal class db4oTypeProcessor
	{
		db4oDataStore _store;
		Hashtable _assemblyHash;

		public db4oTypeProcessor(db4oDataStore store)
		{
			_store = store;
			_assemblyHash = new Hashtable();
		}

		/// <summary>
		/// Indicates if the given assembly has been processed and used to configure
		/// the db4o container.
		/// </summary>
		/// <param name="asm"></param>
		/// <returns></returns>
		public bool IsAssemblyProcessed(Assembly asm) {
			return _assemblyHash.Contains(asm.FullName);
		}

		/// <summary>
		/// Processes an assembly's type information, using it to configure
		/// the db4o container.
		/// </summary>
		/// <param name="asm"></param>
		public void ProcessAssembly(Assembly asm) {
			//For each type that derives from IContentObject, examine its fields
			//and test it for an IPersistenceBoundary impl, and programmatically
			//configure the db4o container accordingly.
			foreach (Type type in asm.GetTypes()) {
				foreach (Type interfaceType in type.GetInterfaces()) {
					if (interfaceType == typeof(IContentObject)) {
						//This type is an IContentObject-derived type.
						//It may be another interface, or a class.
						//Either way, configure it
						ProcessType(type);
						break;
					}
				}
			}
		}

        /// 
        /// <summary>Processes a type that implements IContentObject.
        /// 
        ///     Examines fields for attributes that effect persistence or indexing, looks for
        ///     persistence boundaries as indicated by an IPersistenceBounday interface implementation,
        ///     etc.
        /// 
        ///     Using this information, populates a db4o StoredClass object accordingly, so db4o
        ///     will behave itself.</summary>
        /// <param name="type"></param>
		private void ProcessType(Type type) {
			ObjectClass typeClass = _store.ObjectContainer.configure().objectClass(type);

			//Check the fields for attributes which affect their storage/retrieval
			foreach (FieldInfo fi in type.GetFields()) {
				object[] attrs;
				
				//Check for a Indexed attribute, indicating the field should be indexed
				attrs = fi.GetCustomAttributes(typeof(IndexedAttribute), true);
				if (attrs != null) {
					//Field is indexed
					typeClass.objectField(fi.Name).indexed(true);
				}
				
			}
		}
	}
}
