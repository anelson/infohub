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
	/// Summary description for StoredClassFieldsNode.
	/// </summary>
	public class StoredClassFieldsNode : BaseNode {
		ExtObjectContainer _store;
		StoredClass _class;

		public StoredClassFieldsNode(ExtObjectContainer store, StoredClass storedClass) {
			_store = store;
			_class = storedClass;

			Text = "Stored Fields";
		}

		protected override void LoadChildNodes() {
			//Create a node for each stored field associated with this class

			//Lamely, db4o throws an exception when this is invoked for primitive types.
			//Thus, if an exception is thrown, assume a primitive type
			StoredField[] fields = null;
			try {
				fields = _class.getStoredFields();
			} catch (Exception) {
			}

			if (fields == null) {
				Nodes.Add(new TreeNode("[Primitive type]"));
			} else {
				foreach (StoredField field in fields) {
					Nodes.Add(new StoredClassFieldNode(_store, _class, field));
				}
				}
		}
	}
}
