using System;

namespace InfoHub.ContentModel.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	/// <summary>
	/// Indicates that a property's value should be stored in the persistence mechanism, for
	/// performance reasons.  Usually combined with the IndexedAttribute to create an index
	/// on a member.
	/// </summary>
	public class StoredPropertyAttribute : Attribute
	{
		public StoredPropertyAttribute()
		{
		}
	}
}
