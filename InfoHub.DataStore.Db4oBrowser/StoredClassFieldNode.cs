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
	/// Summary description for StoredClassFieldNode.
	/// </summary>
	public class StoredClassFieldNode : BaseNode {
		ExtObjectContainer _store;
		StoredClass _class;
		StoredField _field;

		public StoredClassFieldNode(ExtObjectContainer store, StoredClass storedClass, StoredField storedField) {
			_store = store;
			_class = storedClass;
			_field = storedField;

			Text = _field.getStoredType().ToString();
			if (_field.isArray()) {
				Text += "[]";
			}
			Text += " " + _field.getName();
		}

		protected override void LoadChildNodes() {
			//If there is a StoredClass object for the type of this field, 
			//create a child node of that class type
			Type fieldType = (Type)_field.getStoredType();
			if (!fieldType.IsPrimitive) {
				//The field isn't a primitive type, so try to get the StoredClass for
				//the field
				StoredClass fieldClass = null;
				try {
					fieldClass = _store.storedClass(_field.getStoredType());
				} catch (Exception) {
				}

				//Though undocumented, it appears that the StoredClass objects
				//for primitives and intrinsics like String are somewhat broken, 
				//and return a null class name.  Thus, don't do anything 
				//with such classes
				if (fieldClass != null &&
					fieldClass.getName() != null) {
					Nodes.Add(new StoredClassNode(_store, fieldClass));
				}
			}
		}
	}
}
