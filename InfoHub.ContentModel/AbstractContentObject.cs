using System;
using System.Collections;
using System.Diagnostics;

using InfoHub.Common;
using InfoHub.ContentModel.Attributes;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// An abstract base implementation of IContentObject
	/// </summary>
	public abstract class AbstractContentObject : IContentObject
	{
		IContentContainer _parent;
		IDictionary _propBag;

		[NotPersisted]
		ContentObjectChangedEventHandler _changed;

		public AbstractContentObject(IContentContainer parent) {
			_parent = parent;

			if (_parent != null) {
				_parent.Children.Add(this);
			} else if (!(this is IRootFolder)) {
				//Else, a null parent and this object isn't an IRootFolder implememtation.
				//A non-null parent is required
				throw new ArgumentNullException("parent");
			}
		}

		#region IContentObject Members

		public virtual void Move(IContentContainer newParent) {
			if (newParent == null) {
				throw new ArgumentNullException("newParent");
			}
			
			Move(newParent, newParent.Children.Count);
		}
		
		public virtual void Move(IContentContainer newParent, int destIdx) {
			if (newParent == null) {
				throw new ArgumentNullException("newParent");
			}

			if (destIdx > newParent.Children.Count) {
				throw new ArgumentOutOfRangeException("destIdx");
			}

			if (newParent != _parent) {
				//Remove from old parent's Children collection, and add to new
				_parent.Children.Remove(this);
				_parent = newParent;
				_parent.Children.Insert(destIdx, this);

				OnChanged(new EventArgs());
			}
			
		}

		public virtual IContentContainer Parent {
			get {
				return _parent;
			}
		}

		public virtual IFileSystemObject FileSystemParent {
			get {
				//This is a simple content object.  If its parent
				//is a file system object, return that, else return the
				//parent's FS parent
				if (_parent is IFileSystemObject) {
					return (IFileSystemObject)_parent;
				} else if (_parent != null) {
					return _parent.FileSystemParent;
				} else {
					//No parent, so no file system parent
					return null;
				}
			}
		}

		public virtual IDictionary PropertyBag {
			get {
				if (_propBag == null) {
					//Get the collection factory from the object persistor
					ICollectionFactory cf = RootPersistor.CollectionFactory;

					if (cf == null) {
						throw new ApplicationException();
					}

					_propBag = cf.CreateDictionary(this);
				}

				return _propBag;
			}
		}

		public event ContentObjectChangedEventHandler Changed {
			add {
				_changed += value;
			}

			remove {
				_changed -= value;
			}
		}

		#endregion

		protected virtual void OnChanged(EventArgs e) {
			if (_changed != null) {
				_changed(this, e);
			}
		}

		/// <summary>
		/// The IObjectPersistor to use for this object.  Obtains 
		/// the object persistor from the root folder of this object.
		/// </summary>
		public virtual IObjectPersistor RootPersistor {
			get {
				//Navigate the ancestry hierarchy until a IFolder
				//is found, then use that to get the root folder, which exposes
				//the Persistor property
				IFileSystemObject fsParent = FileSystemParent;
				IFolder parentFolder = null;

				if (fsParent is IFolder) {
					parentFolder = (IFolder)fsParent;
				} else {
					parentFolder = fsParent.ParentFolder;
				}

				return parentFolder.RootFolder.Persistor;
			}
		}
	}
}
