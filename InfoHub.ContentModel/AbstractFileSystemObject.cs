using System;
using System.Diagnostics;
using System.IO;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Abstract base implementation of IFileSystemObject
	/// </summary>
	public abstract class AbstractFileSystemObject : AbstractContentContainer, IFileSystemObject, IPersistenceNotificationCallback
	{
		String _name;

		public AbstractFileSystemObject(IFolder parent, String name) : base(parent)
		{
			//Name is not optional
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			_name = name;
		}

		#region IPersistenceBoundary members
		
		public virtual bool IsActivated {
			get {
				return RootPersistor.IsActivated(this);
			}
		}

		public virtual void Activate() {
			RootPersistor.Activate(this);
		}

		public virtual void Update() {
			RootPersistor.Update(this);
		}
		
		public virtual void Refresh() {
			RootPersistor.Refresh(this);
		}
		
		public virtual void Delete() {
			RootPersistor.Delete(this);
		}		

		public virtual void Deactivate() {
			RootPersistor.Deactivate(this);
		}

		#endregion

		#region IPersistenceNotificationCallback Members

		public virtual void BeforeActivate() {
		}

		public virtual void BeforeAdd() {
		}

		public virtual void BeforeUpdate() {
		}

		public virtual void BeforeRefresh() {
		}

		public virtual void BeforeDelete() {
		}

		public virtual void BeforeDeactivate() {
		}

		public virtual void AfterActivate() {
		}

		public virtual void AfterAdd() {
		}

		public virtual void AfterUpdate() {
		}

		public virtual void AfterRefresh() {
		}

		public virtual void AfterDelete() {
		}

		public virtual void AfterDeactivate() {
		}

		#endregion

		#region IFileSystemObject Members

		public virtual String Name {
			get {
				return _name;
			}
		}

		public virtual String Path {
			get {
				if (ParentFolder != null) {
					return System.IO.Path.Combine(ParentFolder.Path, Name);
				} else {
					return Name;
				}
			}
		}

		public virtual IFolder ParentFolder {
			get {
				return (IFolder)Parent;
			}
		} 

		public virtual bool IsDescendentOf(IFolder folder) {
			//Check if either this folder is the our immediate parent, or
			//our immediate parent is itself a descendent of this folder
			if (folder == null) {
				throw new ArgumentNullException("folder");
			}

			if (folder == ParentFolder) {
				return true;
			} else if (ParentFolder != null) {
				return ParentFolder.IsDescendentOf(folder);
			} else {
				return false;
			}
		}

		#endregion

		/// <summary>
		/// The IObjectPersistor to use for this object.  Obtains 
		/// the object persistor from the root folder of this object.
		/// </summary>
		public override IObjectPersistor RootPersistor {
			get {
				return this.ParentFolder.RootFolder.Persistor;
			}
		}
	}
}
