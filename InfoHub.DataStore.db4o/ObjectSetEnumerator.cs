using System;
using System.Collections;
using System.IO;

using InfoHub.ContentModel;
using InfoHub.DataStore;

using com.db4o;

namespace InfoHub.DataStore.db4o
{
	/// <summary>
	/// Wraps a db4o ObjectSet as an IEnumerator
	/// </summary>
	internal class ObjectSetEnumerator : IEnumerator
	{
		ObjectSet _set;
		Object _current;

		public ObjectSetEnumerator(ObjectSet set)
		{
			_set = set;
			_current = null;
		}

		#region IEnumerator Members

		public void Reset() {
			_set.reset();
		}

		public object Current {
			get {
				return _current;
			}
		}

		public bool MoveNext() {
			_current = _set.next();
			if (_current == null) {
				return false;
			} else {
				return true;
			}
		}

		#endregion
	}
}
