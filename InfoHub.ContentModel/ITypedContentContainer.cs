using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// A content collection that is identified by a MIME type
	/// </summary>
	public interface ITypedContentContainer : IContentContainer, IPersistenceBoundary
	{
		String MimeType {get;}
	}
}
