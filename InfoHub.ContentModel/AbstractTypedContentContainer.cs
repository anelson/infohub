using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// An abstract base implementation of ITypedContentContainer
	/// </summary>
	public abstract class AbstractTypedContentContainer : AbstractContentContainer, ITypedContentContainer, IPersistenceNotificationCallback
	{
		String _mimeType;

		public AbstractTypedContentContainer(IContentContainer parent, String mimeType) : base(parent) {
			if (mimeType == null) {
				throw new ArgumentNullException("mimeType");
			}

			_mimeType = mimeType;
		}

		#region ITypedContentContainer Members

		public virtual String MimeType {
			get {
				return _mimeType;
			}
		}

		#endregion

		#region IPersistenceBoundary members
		
		
		public virtual bool IsActivated {
			get {
				return RootPersistor.IsActivated(this);
			}
		}

		public virtual void Activate() {
			RootPersistor.Activate(this);
		}

		public virtual void Update() {
			RootPersistor.Update(this);
		}
		
		public virtual void Refresh() {
			RootPersistor.Refresh(this);
		}
		
		public virtual void Delete() {
			RootPersistor.Delete(this);
		}		

		public virtual void Deactivate() {
			RootPersistor.Deactivate(this);
		}

		#endregion

		#region IPersistenceNotificationCallback Members

		public virtual void BeforeActivate() {
		}

		public virtual void BeforeAdd() {
		}

		public virtual void BeforeUpdate() {
		}

		public virtual void BeforeRefresh() {
		}

		public virtual void BeforeDelete() {
		}

		public virtual void BeforeDeactivate() {
		}

		public virtual void AfterActivate() {
		}

		public virtual void AfterAdd() {
		}

		public virtual void AfterUpdate() {
		}

		public virtual void AfterRefresh() {
		}

		public virtual void AfterDelete() {
		}

		public virtual void AfterDeactivate() {
		}

		#endregion
	}
}
