using System;
using System.Collections;
using System.IO;

using InfoHub.ContentModel;
using InfoHub.DataStore;

using com.db4o;

namespace InfoHub.DataStore.db4o
{
	/// <summary>
	/// Wraps a db4o ObjectSet as an IEnumerable to facilitate easy enumeration
	/// </summary>
	internal class ObjectSetEnumerable : IEnumerable
	{
		ObjectSet _set;

		public ObjectSetEnumerable(ObjectSet set)
		{
			_set = set;
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator() {
			return new ObjectSetEnumerator(_set);
		}

		#endregion
	}
}
