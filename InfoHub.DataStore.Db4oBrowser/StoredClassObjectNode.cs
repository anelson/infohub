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
	/// Summary description for StoredClassInstanceNode.
	/// </summary>
	public class StoredClassObjectNode: BaseNode {
		ExtObjectContainer _store;
		StoredClass _class;
		long _id;
		Object _obj;

		public StoredClassObjectNode(ExtObjectContainer store, StoredClass storedClass, long id) {
			_store = store;
			_class = storedClass;
			_id = id;

			_obj = _store.getByID(id);
			_store.activate(_obj, 5);

			Text = "Instance #" + _id + " - " + _obj.ToString();
		}

		protected override void LoadChildNodes() {
			//Create a node for each stored field stored for this class.
			//Unlike StoredClassFieldNode, this node will display the actual value of
			//the field for this specific instance
			foreach (StoredField field in _class.getStoredFields()) {
				Nodes.Add(new StoredClassObjectFieldNode(_store, _class, _obj, field));
			}
		}
	}
}
