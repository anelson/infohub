using System;

namespace InfoHub.ContentModel.Text
{
	/// <summary>
	/// Represents a rich text document (eg, RTF, HTML, Word), ACSII or Unicode.
	/// 
	/// May contain rich text components from the underlying document, OR additional
	/// rich text inserted by the content processing pipeline
	/// </summary>
	public class RichTextDocument : AbstractTextDocument
	{
		public RichTextDocument(IFolder parent, String name, String mimeType) : base(parent, name, mimeType)
		{
		}
	}
}
