using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// A special folder which is the ancestor of all folders, documents, and objects
	/// within a collection.
	/// </summary>
	public interface IRootFolder : IFolder
	{
		String Source {get;}
	}
}
