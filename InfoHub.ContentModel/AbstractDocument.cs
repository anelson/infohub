using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Abstract base implementation of IDocument
	/// </summary>
	public abstract class AbstractDocument : AbstractFileSystemObject, IDocument
	{
		String _mimeType;

		public AbstractDocument(IFolder parent, String name, String mimeType) : base(parent, name)
		{
			//If parent is null, bogus
			if (mimeType == null) {
				throw new ArgumentNullException("mimeType");
			}

			_mimeType = mimeType;
		}

		public override void Move(IContentContainer newParent) {
			if (newParent == null) {
				throw new ArgumentNullException("newParent");
			}

			//Documents can only be moved to folders
			if (!(newParent is IFolder)) {
				throw new ArgumentException();
			}

			base.Move (newParent);
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
