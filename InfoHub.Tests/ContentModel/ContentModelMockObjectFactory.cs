using System;
using System.Collections;
using System.IO;

using DotNetMock.Dynamic;

using NUnit.Framework;

using InfoHub.Common;
using InfoHub.ContentModel;
using InfoHub.DataStore;

namespace InfoHub.Tests.ContentModel {
	/// <summary>
	/// Static factory class which produces mock objects that emulate real-world
	/// implementations of DataStore interfaces
	/// </summary>
	public class ContentModelMockObjectFactory {
		private ContentModelMockObjectFactory() {
			//Static singleton
		}

		public static IObjectPersistor CreateObjectPersistor() {
			DynamicMock mock = new DynamicMock(typeof(IObjectPersistor));

			mock.SetValue("CollectionFactory", CreateCollectionFactory());

			return (IObjectPersistor)mock.Object;
		}

		public static ICollectionFactory CreateCollectionFactory() {
			return new CollectionFactoryMock();
		}

		private class CollectionFactoryMock : ICollectionFactory {
			#region ICollectionFactory Members

			public IDictionary CreateDictionary(IContentObject owner) {
				return new Hashtable();
			}

			public IList CreateList(IContentObject owner) {
				return new ArrayList();
			}

			#endregion

		}
	}

}
