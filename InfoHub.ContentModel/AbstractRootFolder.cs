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
		IObjectPersistor _persistor;

		public AbstractRootFolder(String source, IObjectPersistor persistor) : base(null, System.IO.Path.DirectorySeparatorChar.ToString())
		{
			if (source == null) {
				throw new ArgumentNullException("source");
			}

			if (persistor == null) {
				throw new ArgumentNullException("persistor");
			}

			_source = source;
			_persistor = persistor;
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

		public virtual IObjectPersistor Persistor {
			get {
				return _persistor;
			}
		}

			#endregion

			public override IObjectPersistor RootPersistor {
			get {
				//The root folder knows about its own root persistor
				return _persistor;
			}
		}

	}
}
