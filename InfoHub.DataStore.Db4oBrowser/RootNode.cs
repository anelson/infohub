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
	/// Summary description for RootNode.
	/// </summary>
	public class RootNode : BaseNode
	{
		ExtObjectContainer _store;

		public RootNode(ExtObjectContainer store)
		{
			_store = store;

			Text = "Store ID " + store.identity().getID(store);

			//Load child nodes immediately to seed the tree
			LoadChildNodes();
		}

		protected override void LoadChildNodes() {
			//For each type in the store, create a node
			StoredClass[] classes = _store.storedClasses();

			foreach (StoredClass storedClass in classes) {
				Nodes.Add(new StoredClassNode(_store, storedClass));
			}
		}
	}
}
