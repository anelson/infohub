using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// An abstract base implementation of ITypedContentContainer
	/// </summary>
	public abstract class AbstractTypedContentContainer : AbstractContentContainer, ITypedContentContainer
	{
		String _mimeType;

		public AbstractTypedContentContainer(IContentContainer parent, String mimeType) : base(parent)
		{
			_mimeType = mimeType;
		}

		#region ITypedContentContainer Members

		public virtual String MimeType {
			get {
				return _mimeType;
			}
		}

		#endregion
	}
}
