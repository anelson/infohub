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
	/// Summary description for StoredClassObjectsNode.
	/// </summary>
	public class StoredClassObjectsNode : BaseNode {
		ExtObjectContainer _store;
		StoredClass _class;

		public StoredClassObjectsNode(ExtObjectContainer store, StoredClass storedClass) {
			_store = store;
			_class = storedClass;

			Text = "Stored Instances";
		}

		protected override void LoadChildNodes() {
			//Create a node for each instance associated with this class
			foreach (long id in _class.getIDs()) {
				Nodes.Add(new StoredClassObjectNode(_store, _class, id));
			}
		}
	}
}
