using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Geeneric implementation of IDocument
	/// </summary>
	public class GenericDocument : AbstractDocument
	{
		public GenericDocument(IFolder parent, String name, String mimeType) : base(parent, name, mimeType)
		{
		}

	}
}
