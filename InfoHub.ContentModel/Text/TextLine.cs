using System;

using InfoHub.ContentModel;

namespace InfoHub.ContentModel.Text
{
	/// <summary>
	/// A line of text within a text document
	/// </summary>
	public class TextLine : GenericContentContainer
	{
		public TextLine(IContentContainer parent) : base(parent)
		{
		}
	}
}
