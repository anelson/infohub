using System;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Generic impl of IRootFolder
	/// </summary>
	public class GenericRootFolder : AbstractRootFolder
	{
		public GenericRootFolder(String source, IObjectPersistor persistor) : base(source, persistor)
		{
		}
	}
}
