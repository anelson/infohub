using System;

namespace InfoHub.ContentModel.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	/// <summary>
	/// Attribute used to mark a class or field as not persisted, which will cause
	/// the class/field to be ignored when saving a content model to a persistent storage 
	/// medium
	/// </summary>
	public class NotPersistedAttribute : Attribute
	{
	}
}
