using System;
using System.Collections;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// The interface implemented by a collection factory that produces instances
	/// of various collection types which are suitable for use containing content model
	/// objects, and will persist/unpersist correctly.
	/// </summary>
	public interface ICollectionFactory
	{
		IList CreateList(IContentObject owner);
		IDictionary CreateDictionary(IContentObject owner);
	}
}
