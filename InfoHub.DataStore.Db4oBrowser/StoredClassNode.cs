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
	/// Summary description for StoredClassNode.
	/// </summary>
	public class StoredClassNode : BaseNode
	{
		ExtObjectContainer _store;
		StoredClass _class;

		public StoredClassNode(ExtObjectContainer store, StoredClass storedClass)
		{
			_store = store;
			_class = storedClass;

			Text = _class.getName();
		}

		protected override void LoadChildNodes() {
			//Create one child node that will point back to the parent class
			//Create another to store the collection of stored fields for this class
			//Create a third for the list of objects of this type stored in the collection
			Nodes.Add(new StoredClassParentNode(_store, _class));
			Nodes.Add(new StoredClassFieldsNode(_store, _class));
			Nodes.Add(new StoredClassObjectsNode(_store, _class));
		}
	}
}
