using System;
using System.Collections;

namespace InfoHub.ContentModel
{
	public delegate void ContentObjectChangedEventHandler(Object sender, EventArgs e);

	/// <summary>
	/// Interface implemented by all content objects; that is, objects which
	/// may appear within the content graph of a document to represent its 
	/// content in a hierarchical, typed fashion.
	/// </summary>
	public interface IContentObject
	{
		/// <summary>
		/// Moves the object to a new parent.
		/// </summary>
		/// <param name="newParent"></param>
		void Move(IContentContainer newParent);

		/// <summary>
		/// This object's parent, or null if and only if this is the top-level
		/// content object in the tree
		/// </summary>
		IContentContainer Parent{get;}

		/// <summary>
		/// Gets the file system object containing this object.  For content objects
		/// inside a document, this will be the IDocument that contains them.  For documents
		/// and folders, this will be the IFolder that contains them.
		/// </summary>
		IFileSystemObject FileSystemParent {get;}

		/// <summary>
		/// Gets a key/value pair container for use storing and retrieving state information
		/// pertaining to this object.  Allows content pipeline processors to preserve state
		/// when manipulating content
		/// </summary>
		IDictionary PropertyBag {get;}

		/// <summary>
		/// Event fired whenever the content object changes in some way
		/// </summary>
		event ContentObjectChangedEventHandler Changed;
	}
}
