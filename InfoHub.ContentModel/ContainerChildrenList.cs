using System;
using System.Collections;
using System.Diagnostics;

using InfoHub.Common;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// An IList implementation specifically for maintaining the list of children
	/// of a content container.  Includes logic to verify children have the corrent
	/// Parent property
	/// </summary>
	internal class ContainerChildrenList : IContentObjectList
	{
		AbstractContentContainer _parent;
		IList _list;

		/// <summary>
		/// Creates a new instance which will track the children of a specified
		/// content container.  
		/// </summary>
		/// <param name="parent"></param>
		public ContainerChildrenList(AbstractContentContainer parent)
		{
			_parent = parent;
			ICollectionFactory cf = parent.RootPersistor.CollectionFactory;
			if (cf == null) {
				throw new ApplicationException();
			}

			_list = cf.CreateList(_parent);
		}

		public IContentContainer Parent {
			get {
				return _parent;
			}
		}

		public IContentObject this[ int index ]  {
			get  {
				return( (IContentObject) _list[index] );
			}
			set  {
				//Do not support replacing list items this way;
				//you must Remove() one and use Move() or create the child
				//anew to add.
				throw new InvalidOperationException();
			}
		}

		public int Add( IContentObject value )  {
			OnValidate(value);
			return( _list.Add( value ) );
		}

		public int IndexOf( IContentObject value )  {
			return( _list.IndexOf( value ) );
		}

		public void Insert( int index, IContentObject value )  {
			OnValidate(value);
			_list.Insert( index, value );
		}

		public void Remove( IContentObject value )  {
			_list.Remove( value );
		}

		public bool Contains( IContentObject value )  {
			return( _list.Contains( value ) );
		}

		public void CarveOut(IContentContainer dest, int destIdx, int srcIdx, int srcLength) {
			//Extract the child objects from srcIdx to srcIdx + srcLength - 1, and place them
			//into dest starting at destIdx
			if (dest == null) {
				throw new ArgumentNullException("dest");
			}

			if (srcIdx >= Count ||
				srcIdx + srcLength - 1 >= Count) {
				throw new ArgumentOutOfRangeException("srcIdx");
			}

			if (destIdx > dest.Children.Count) {
				throw new ArgumentOutOfRangeException("destIdx");
			}

			//For each object included in the carve-out, remove
			//it from this list, move it to the new parent, and
			//insert it into the destination list
			for (int idx = 0; idx < srcLength; idx++) {
				//Note that, each time the object at idx is removed, 
				//the object at idx+1 moves to index idx, thus even though
				//the loop counter variable idx moves from 0 on up, 
				//the index of the current object is always srcIdx
				IContentObject obj = this[srcIdx];
				obj.Move(dest, destIdx+idx);
			}
		}

		protected virtual void OnValidate( Object value )  {
			if (value == null) {
				throw new ArgumentNullException("value");
			}

			if (!(value is IContentObject)) {
				throw new ArgumentException();
			}

			//Allow the content container to validate
			_parent.OnValidateChild((IContentObject)value);
		}
		#region IList Members

		public bool IsReadOnly {
			get {
				return _list.IsReadOnly;
			}
		}

		object System.Collections.IList.this[int index] {
			get {
				return _list[index];
			}
			set {
				OnValidate(value);
				_list[index] = value;
			}
		}

		public void RemoveAt(int index) {
			_list.RemoveAt(index);
		}

		void System.Collections.IList.Insert(int index, object value) {
			OnValidate(value);
			_list.Insert(index, value);
		}

		void System.Collections.IList.Remove(object value) {
			_list.Remove(value);
		}

		bool System.Collections.IList.Contains(object value) {
			return _list.Contains(value);
		}

		public void Clear() {
			_list.Clear();
		}

		int System.Collections.IList.IndexOf(object value) {
			return _list.IndexOf(value);
		}

		int System.Collections.IList.Add(object value) {
			OnValidate(value);
			return _list.Add(value);
		}

		public bool IsFixedSize {
			get {
				return _list.IsFixedSize;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized {
			get {
				return _list.IsSynchronized;
			}
		}

		public int Count {
			get {
				return _list.Count;
			}
		}

		public void CopyTo(Array array, int index) {
			_list.CopyTo(array, index);
		}

		public object SyncRoot {
			get {
				return _list.SyncRoot;
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator() {
			return _list.GetEnumerator();
		}

		#endregion
	}
}
