using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Decorator interface indicating that a type is a persistence boundary; that is, 
	/// a marker in an object graph such that all nodes between it and another persistence
	/// boundary are loaded and unloaded as one.  
	/// </summary>
	public interface IPersistenceBoundary
	{
		bool IsActivated {get;}

		void Activate();
		void Update();
		void Refresh();
		void Delete();
		void Deactivate();
	}
}
