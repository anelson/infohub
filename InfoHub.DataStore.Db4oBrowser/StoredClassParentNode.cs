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
	/// Summary description for StoredClassParentNode.
	/// </summary>
	public class StoredClassParentNode : BaseNode {
		ExtObjectContainer _store;
		StoredClass _class;

		public StoredClassParentNode(ExtObjectContainer store, StoredClass storedClass) {
			_store = store;
			_class = storedClass;

			Text = "Parent Class";
		}

		protected override void LoadChildNodes() {
			//If there is a non-null parent class, create a node for the parent class
			//else, use a plain node w/ some text
			if (_class.getParentStoredClass() != null) {
				Nodes.Add(new StoredClassNode(_store, _class.getParentStoredClass()));
			} else {
				Nodes.Add(new TreeNode("[No parent class]"));
			}
		}
	}
}
