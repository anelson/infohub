using System;

using InfoHub.ContentModel;

namespace InfoHub.ContentModel.Text
{
	/// <summary>
	/// Abstract base class for documents of a primarily textual nature, such as
	/// a text file, an RTF, HTML, etc.
	/// </summary>
	public abstract class AbstractTextDocument : GenericDocument
	{
		public AbstractTextDocument(IFolder parent, String name, String mimeType) : base(parent, name, mimeType)
		{
		}
	}
}
