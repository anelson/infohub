using System;
using System.Diagnostics;
using System.IO;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Abstract base implementation of IFileSystemObject
	/// </summary>
	public abstract class AbstractFileSystemObject : AbstractContentContainer, IFileSystemObject
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
	}
}
