using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// A folder which contains IDocument's and other IFolder's
	/// </summary>
	public interface IFolder : IFileSystemObject
	{
		/// <summary>
		/// The root or top-level folder in which this folder is located
		/// </summary>
		IRootFolder RootFolder {get;}

		/// <summary>
		/// Tests if this folder is an ancestor of the given file system object; 
		/// that is, the given file system object is contained within the folder or
		/// one of its subfolders
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		bool IsAncestorOf(IFileSystemObject descendent);
	}
}
