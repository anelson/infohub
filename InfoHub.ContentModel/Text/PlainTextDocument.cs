using System;

using InfoHub.ContentModel;

namespace InfoHub.ContentModel.Text
{
	/// <summary>
	/// Represents a plain text document (eg, .txt), ACSII or Unicode.
	/// 
	/// This is not to say the plain text document will not have rich text elements
	/// after it has been processed by the content pipeline, however the consumer can
	/// be assured that the underlying format of the file is plain text
	/// </summary>
	public class PlainTextDocument : AbstractTextDocument
	{
		public PlainTextDocument(IFolder parent, String name, String mimeType) : base(parent, name, mimeType)
		{
		}
	}
}
