using System;
using System.Diagnostics;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// Abstract base impl of IRootFolder
	/// </summary>
	public class AbstractRootFolder : AbstractFolder, IRootFolder
	{
		String _source;

		public AbstractRootFolder(String source) : base(null, System.IO.Path.DirectorySeparatorChar.ToString())
		{
			if (source == null) {
				throw new ArgumentNullException("source");
			}

			_source = source;
		}

		public override void Move(IContentContainer newParent) {
			//Root folders cannot be moved, as they are roots
			throw new InvalidOperationException();
		}


		#region IRootFolder Members

		public virtual String Source {
			get {
				return _source;
			}
		}

		#endregion
	}
}
