using System;
using System.Collections;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// A content object which contains other content objects, and has 
	/// no content of its own
	/// </summary>
	public interface IContentContainer : IContentObject
	{
		/// <summary>
		/// Gets a list of all child content objects
		/// </summary>
		IContentObjectList Children {get;}
	}
}
