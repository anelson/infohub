using System;

using InfoHub.ContentModel;

namespace InfoHub.ContentModel.Text
{
	/// <summary>
	/// A block of text within a line.
	/// </summary>
	public class TextBlock : GenericContentObject
	{
		String _text;

		public TextBlock(IContentContainer parent) : base(parent)
		{
		}

		public String Text {
			get {
				return _text;
			}

			set {
				if (_text != value) {
					_text = value;
					OnChanged(new EventArgs());
				}
			}
		}
	}
}
