using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Abstract base impl of IFolder
	/// </summary>
	public abstract class AbstractFolder : AbstractFileSystemObject, IFolder
	{
		public AbstractFolder(IFolder parent, String name) : base(parent, name)
		{
		}

		#region IFolder Members

		public virtual IRootFolder RootFolder {
			get {
				//If parent is null, this is the root folder, else, 
				//return the parent's root folder
				if (ParentFolder == null) {
					return (IRootFolder)this;
				} else {
					return ParentFolder.RootFolder;
				}
			}
		}

		public virtual bool IsAncestorOf(IFileSystemObject obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}

			//Check our children collection, recursively
			foreach (IFileSystemObject fso in Children) {
				if (fso == obj) {
					return true;
				} else if (fso is IFolder) {
					//This is a child folder; see if it contains obj somewhere
					if (((IFolder)fso).IsAncestorOf(obj)) {
						return true;
					}
				}
			}

			//Not an ancestor of obj
			return false;
		}

		#endregion

		/// <summary>
		/// Overrides base validation logic to ensure that only file system objects
		/// are added to the folder
		/// </summary>
		/// <param name="child"></param>
		internal override void OnValidateChild(IContentObject child) {
			base.OnValidateChild (child);

			if (!(child is IFileSystemObject)) {
				throw new ArgumentException();
			}
		}

	}
}
