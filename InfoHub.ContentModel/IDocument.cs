using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Represents a single document containing content objects, and having
	/// a name.  Note that a document is a typed content container, and thus has
	/// a mime type.  A document could conceivably contain additional embedded
	/// typed content containers (a multi-part mail message) or even multiple documents
	/// (a zip archive).
	/// </summary>
	public interface IDocument : ITypedContentContainer, IFileSystemObject
	{
	}
}
