using System;
using System.Collections;
using System.Diagnostics;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Generic implementation of IContentObject
	/// </summary>
	public class GenericContentObject : AbstractContentObject
	{
		public GenericContentObject(IContentContainer parent) : base(parent) {
		}
	}
}
