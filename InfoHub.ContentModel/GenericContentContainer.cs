using System;
using System.Collections;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Generic implementation of IContentContainer
	/// </summary>
	public class GenericContentContainer : AbstractContentContainer
	{
		public GenericContentContainer(IContentContainer parent) : base(parent)
		{
		}

	}
}
