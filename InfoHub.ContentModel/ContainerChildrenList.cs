using System;
using System.Collections;
using System.Diagnostics;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// An IList implementation specifically for maintaining the list of children
	/// of a content container.  Includes logic to verify children have the corrent
	/// Parent property
	/// </summary>
	internal class ContainerChildrenList : CollectionBase, IContentObjectList
	{
		AbstractContentContainer _parent;

		/// <summary>
		/// Creates a new instance which will track the children of a specified
		/// content container.  
		/// </summary>
		/// <param name="parent"></param>
		public ContainerChildrenList(AbstractContentContainer parent)
		{
			_parent = parent;
		}

		public IContentContainer Parent {
			get {
				return _parent;
			}
		}

		public IContentObject this[ int index ]  {
			get  {
				return( (IContentObject) List[index] );
			}
			set  {
				//Do not support replacing list items this way;
				//you must Remove() one and use Move() or create the child
				//anew to add.
				throw new InvalidOperationException();
			}
		}

		public int Add( IContentObject value )  {
			return( List.Add( value ) );
		}

		public int IndexOf( IContentObject value )  {
			return( List.IndexOf( value ) );
		}

		public void Insert( int index, IContentObject value )  {
			List.Insert( index, value );
		}

		public void Remove( IContentObject value )  {
			List.Remove( value );
		}

		public bool Contains( IContentObject value )  {
			return( List.Contains( value ) );
		}

		public void CarveOut(IContentObjectList dest, int destIdx, int srcIdx, int srcLength) {
			//Extract the child objects from srcIdx to srcIdx + srcLength - 1, and place them
			//into dest starting at destIdx
			if (dest == null) {
				throw new ArgumentNullException("dest");
			}

			if (srcIdx >= Count ||
				srcIdx + srcLength - 1 >= Count) {
				throw new IndexOutOfRangeException();
			}

			if (destIdx > dest.Count) {
				throw new IndexOutOfRangeException();
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
				RemoveAt(srcIdx);

				obj.Move(dest.Parent);
				Insert(destIdx+idx, obj);
			}
		}

		protected override void OnValidate( Object value )  {
			base.OnValidate(value);

			if (!(value is IContentObject)) {
				throw new ArgumentException();
			}

			//Allow the content container to validate
			_parent.OnValidateChild((IContentObject)value);
		}
	}
}
