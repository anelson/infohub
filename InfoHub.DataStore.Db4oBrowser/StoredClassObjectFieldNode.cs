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
	/// Summary description for StoredClassObjectFieldNode.
	/// </summary>
	public class StoredClassObjectFieldNode: BaseNode {
		ExtObjectContainer _store;
		StoredClass _class;
		Object _obj;
		StoredField _field;

		public StoredClassObjectFieldNode(ExtObjectContainer store, StoredClass storedClass, Object obj, StoredField storedField) {
			_store = store;
			_class = storedClass;
			_obj = obj;
			_field = storedField;

			Text = _field.getStoredType().ToString();
			if (_field.isArray()) {
				Text += "[]";
			}
			Text += " " + _field.getName();
		}

		protected override void LoadChildNodes() {
			//Get the value of the field.  If it's a type w/ a StoredClass, then create
			//a StoredClassObject node.  If it's a primitive type and thus doesn't have
			//a StoredClass, then just do a ToString()

			//First, check if field is null
			Object fieldValue = _field.get(_obj);
			if (fieldValue == null) {
				Nodes.Add(new TreeNode("[Null]"));
				return;
			}

			//Not null.  Create node(s) to display the field value
			if (_field.isArray()) {
				//Field is an array, so there are potentially multiple values
				foreach (Object value in (Array)fieldValue) {
					CreateFieldValueNode(value);
				}
			} else {
				//Else, just one value
				CreateFieldValueNode(fieldValue);
			}
		}

		/// <summary>
		/// Creates a node for a discrete value of the field.
		/// </summary>
		/// <param name="value"></param>
		private void CreateFieldValueNode(object value) {
			//If this type has a StoredClass object, create a StoredClassObjectNode, else,
			//use ToString() to populate a plain tree node
			StoredClass fieldValueClass = _store.storedClass(value.GetType());

			if (fieldValueClass != null) {
				Nodes.Add(new StoredClassObjectNode(_store, fieldValueClass, _store.getID(value)));
			} else {
				Nodes.Add(new TreeNode(value.ToString()));
			}
		}
	}
}
