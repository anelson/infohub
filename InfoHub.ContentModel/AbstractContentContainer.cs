using System;
using System.Collections;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Abstract base implementation of IContentContainer
	/// </summary>
	public abstract class AbstractContentContainer : AbstractContentObject, IContentContainer
	{
		ContainerChildrenList _children;
		
		public AbstractContentContainer(IContentContainer parent) : base(parent)
		{
			_children = null;
		}

		#region IContentContainer Members

		public virtual IContentObjectList Children {
			get {
				//Just-in-time construction of the children list
				if (_children == null) {
					_children = new ContainerChildrenList(this);
				}
				return _children;
			}
		}

		#endregion

		/// <summary>
		/// Internal method called by ContainerChildrenList whenever a child 
		/// is about to be placed in the collection of children.  Allows
		/// specific content containers to apply additional validation 
		/// rules to their children
		/// </summary>
		/// <param name="child"></param>
		internal virtual void OnValidateChild(IContentObject child) {
			//Make sure this object has a parent equal to the parent for this collection
			if (((IContentObject)child).Parent != this) {
				throw new ArgumentException();
			}
		}
	}
}
