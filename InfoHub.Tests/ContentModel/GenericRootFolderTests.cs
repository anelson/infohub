using System;
using System.IO;

using InfoHub.Common;
using InfoHub.ContentModel;

using NUnit.Framework;

namespace InfoHub.Tests.ContentModel
{
	[TestFixture]
	/// <summary>
	/// Test fixture which tests the implementation of GenericRootFolder
	/// </summary>
	public class GenericRootFolderTests : TestBase
	{
		[Test]
		public void EmptyTest() {
			GenericRootFolder root = new GenericRootFolder("Foo", ContentModelMockObjectFactory.CreateObjectPersistor());

			Assert.AreEqual(0, root.Children.Count);
			Assert.IsFalse(root.IsAncestorOf(root));
			Assert.IsFalse(root.IsDescendentOf(root));
			Assert.AreEqual(Path.DirectorySeparatorChar.ToString(), root.Name);
			Assert.AreEqual(Path.DirectorySeparatorChar.ToString(), root.Path);
			Assert.IsNull(root.Parent);
			Assert.IsNull(root.ParentFolder);
			Assert.IsNull(root.FileSystemParent);
			Assert.AreEqual(root, root.RootFolder);
			Assert.AreEqual("Foo", root.Source);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullSourceTest() {
			GenericRootFolder root = new GenericRootFolder(null, ContentModelMockObjectFactory.CreateObjectPersistor());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullPersistorTest() {
			GenericRootFolder root = new GenericRootFolder("Foo", null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NonFsoChildTest() {
			GenericRootFolder root = new GenericRootFolder("Foo", ContentModelMockObjectFactory.CreateObjectPersistor());

			//Attempt to add a non-IFileSystemObject child
			GenericContentObject co = new GenericContentObject(root);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void MoveTest() {
			//attempt to move a root folder to another container.
			//That is not permitted
			GenericRootFolder root = new GenericRootFolder("Foo", ContentModelMockObjectFactory.CreateObjectPersistor());
			GenericRootFolder root2 = new GenericRootFolder("Bar", ContentModelMockObjectFactory.CreateObjectPersistor());

			root.Move(root2);
		}

	}
}
