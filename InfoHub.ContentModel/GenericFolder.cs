using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Generic impl of IFolder
	/// </summary>
	public class GenericFolder : AbstractFolder
	{
		public GenericFolder(IFolder parent, String name) : base(parent, name)
		{
		}
	}
}
