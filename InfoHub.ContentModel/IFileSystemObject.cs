using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Base interface implemented by both documents and folders
	/// </summary>
	public interface IFileSystemObject : IContentContainer{
		String Name {get;}
		String Path {get;}

		/// <summary>
		/// The parent object of file system objects is always
		/// an IFolder
		/// </summary>
		IFolder ParentFolder {get;}

		/// <summary>
		/// Tests if this file system object is a descendent of a given folder; that is,
		/// if this file system object is contained within a given folder or one of its subfolders
		/// </summary>
		/// <param name="folder"></param>
		/// <returns></returns>
		bool IsDescendentOf(IFolder folder);
	}
}
