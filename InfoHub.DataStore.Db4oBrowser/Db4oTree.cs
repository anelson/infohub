using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Spring.Context;
using com.db4o;
using com.db4o.config;
using com.db4o.ext;
using com.db4o.query;

using InfoHub.Common;

namespace InfoHub.DataStore.Db4oBrowser
{
	/// <summary>
	/// Summary description for Db4oTree.
	/// </summary>
	public class Db4oTree : System.Windows.Forms.TreeView
	{
		ExtObjectContainer _store;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Db4oTree()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ExtObjectContainer Store {
			get {
				return _store;
			}

			set {
				_store = value;

				Nodes.Clear();
				Nodes.Add(new RootNode(_store));
			}
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		protected override void OnBeforeExpand(TreeViewCancelEventArgs e) {
			base.OnBeforeExpand (e);

			//If the node in question is a BaseNode-derived node, allow it to process the message
			if (e.Node is BaseNode) {
				((BaseNode)e.Node).OnBeforeExpand(e);
			}
		}


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// Db4oTree
			// 
			this.Sorted = true;

		}
		#endregion
	}
}
