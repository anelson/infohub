using System;

namespace InfoHub.ContentModel.Attributes {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple=false,Inherited=true)]
	/// <summary>
	/// Attribute which controls how adding, updating, loading, and deleting of object(s)
	/// referenced by a field are to be performed.
	/// 
	/// Using this attribute, specific fields can have cascading fully disabled, fully enabled, or
	/// partially enabl3ed
	/// </summary>
	public class CascadeAttribute : Attribute
	{
		bool _cascadeAdd, _cascadeUpdate, _cascadeDelete, _cascadeLoad;

		public CascadeAttribute(bool cascade)
		{
			_cascadeAdd = _cascadeUpdate = _cascadeDelete = _cascadeLoad = cascade;
		}

		public CascadeAttribute(bool cascadeAdd, bool cascadeUpdate, bool cascadeDelete, bool cascadeLoad) {
			_cascadeAdd = cascadeAdd; 
			_cascadeUpdate = cascadeUpdate; 
			_cascadeDelete = cascadeDelete; 
			_cascadeLoad = cascadeLoad; 
		}

		public bool CascadeAdd { get { return _cascadeAdd; } }
		public bool CascadeUpdate { get { return _cascadeUpdate; } }
		public bool CascadeDelete { get { return _cascadeDelete; } }
		public bool CascadeLoad { get { return _cascadeLoad; } }
	}
}
