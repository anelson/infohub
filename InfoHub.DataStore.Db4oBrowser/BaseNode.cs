using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace InfoHub.DataStore.Db4oBrowser
{
	/// <summary>
	/// Summary description for BaseNode.
	/// </summary>
	public abstract class BaseNode : TreeNode
	{	
		public BaseNode() {
		}

		public virtual void OnBeforeExpand(TreeViewCancelEventArgs e) {
			//Before expanding, load the child nodes of 
			//all child BaseNode-derived nodes, so they'll show up as expandable 
			//if they have any children
			foreach (TreeNode node in Nodes) {
				if (node is BaseNode) {
					node.Nodes.Clear();
					((BaseNode)node).LoadChildNodes();
				}
			}
		}

		protected abstract void LoadChildNodes();
	}
}
