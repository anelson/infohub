using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// An interface which, if implemented by an IPersistenceBoundary instance, will
	/// be called by the persistence framework to allow the implementation to 
	/// perform whatever logic it requires at key points in the persistence process.
	/// </summary>
	public interface IPersistenceNotificationCallback {
		void BeforeActivate();
		void BeforeAdd();
		void BeforeUpdate();
		void BeforeRefresh();
		void BeforeDelete();
		void BeforeDeactivate();

		void AfterActivate();
		void AfterAdd();
		void AfterUpdate();
		void AfterRefresh();
		void AfterDelete();
		void AfterDeactivate();
	}
}
