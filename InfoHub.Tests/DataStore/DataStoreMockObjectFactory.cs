using System;
using System.IO;

using DotNetMock.Dynamic;

using NUnit.Framework;

using InfoHub.Common;
using InfoHub.ContentModel;
using InfoHub.DataStore;

namespace InfoHub.Tests.DataStore
{
	/// <summary>
	/// Static factory class which produces mock objects that emulate real-world
	/// implementations of DataStore interfaces
	/// </summary>
	public class DataStoreMockObjectFactory
	{
		private DataStoreMockObjectFactory()
		{
			//Static singleton
		}

		public static IDataStore CreateDataStore() {
			return null;
		}
	}
}
