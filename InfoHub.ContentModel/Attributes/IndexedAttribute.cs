using System;

namespace InfoHub.ContentModel.Attributes
{
	
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false)]
	/// <summary>
	/// Indicates that the field should be indexed for faster searching.
	/// 
	/// Can also be applied to properties, though it has no effect unless
	/// the StoredPropertyAttribute is also applied
	/// </summary>
	public class IndexedAttribute : Attribute
	{
		public IndexedAttribute()
		{
		}
	}
}
