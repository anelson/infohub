using System;
using System.IO;

using InfoHub.Common;
using InfoHub.ContentModel;

using NUnit.Framework;

namespace InfoHub.Tests.ContentModel {
	[TestFixture]
	/// <summary>
	/// Test fixture which tests the implementation of GenericFolder
	/// </summary>
	public class GenericFolderTests : TestBase {
		IRootFolder _root;

		[SetUp]
		public void CreateRoot() {
			_root = new GenericRootFolder("Foo");
		}

		[Test]
		public void GenericEmptyTest() {
			GenericFolder folder = new GenericFolder(_root, "Bar");

			Assert.AreEqual(1, _root.Children.Count);
			Assert.AreEqual(0, folder.Children.Count);
			Assert.IsFalse(folder.IsAncestorOf(folder));
			Assert.IsFalse(folder.IsDescendentOf(folder));
			Assert.AreEqual("Bar", folder.Name);
			Assert.AreEqual(_root.Path + folder.Name, folder.Path);
			Assert.AreEqual(_root, folder.Parent);
			Assert.AreEqual(_root, folder.ParentFolder);
			Assert.AreEqual(_root, folder.FileSystemParent);
			Assert.AreEqual(_root, folder.RootFolder);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullParentTest() {
			GenericFolder folder = new GenericFolder(null, "Bar");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullNameTest() {
			GenericFolder folder = new GenericFolder(_root, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NonFsoChildTest() {
			GenericFolder folder = new GenericFolder(_root, "Bar");

			//Attempt to add a non-IFileSystemObject child
			GenericContentObject co = new GenericContentObject(folder);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullIsAncestorTest() {
			GenericFolder folder = new GenericFolder(_root, "Bar");

			//Can't pass null to IsAncestorOf
			folder.IsAncestorOf(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MoveNullTest() {
			//attempt to move a folder a null folder
			//That is not permitted
			GenericFolder folder = new GenericFolder(_root, "Bar");

			folder.Move(null);
		}

		[Test]
		public void MoveTest() {
			GenericFolder folder1 = new GenericFolder(_root, "Bar");
			GenericFolder folder2 = new GenericFolder(_root, "Baz");
			GenericFolder folder3 = new GenericFolder(folder2, "Boo");

			Assert.AreEqual(2, _root.Children.Count);
			Assert.AreEqual(0, folder1.Children.Count);
			Assert.AreEqual(1, folder2.Children.Count);
			Assert.AreEqual(0, folder3.Children.Count);
			
			Assert.AreEqual(_root, folder1.ParentFolder);
			Assert.AreEqual(_root, folder2.ParentFolder);
			Assert.AreEqual(folder2, folder3.ParentFolder);

			Assert.AreEqual(Path.Combine(folder2.Path, folder3.Name), folder3.Path);
			folder3.Move(folder1);
			Assert.AreEqual(Path.Combine(folder1.Path, folder3.Name), folder3.Path);

			Assert.AreEqual(2, _root.Children.Count);
			Assert.AreEqual(1, folder1.Children.Count);
			Assert.AreEqual(0, folder2.Children.Count);
			Assert.AreEqual(0, folder3.Children.Count);
			
			Assert.AreEqual(_root, folder1.ParentFolder);
			Assert.AreEqual(_root, folder2.ParentFolder);
			Assert.AreEqual(folder1, folder3.ParentFolder);
		}

		[Test]
		public void AncestryTest() {
			GenericFolder folder1 = new GenericFolder(_root, "folder1");
			GenericFolder folder2 = new GenericFolder(_root, "folder2");
			GenericFolder folder3 = new GenericFolder(folder2, "folder3");

			//The root folder is the ancestor of everyone
			Assert.IsTrue(_root.IsAncestorOf(folder1));
			Assert.IsTrue(_root.IsAncestorOf(folder2));
			Assert.IsTrue(_root.IsAncestorOf(folder3));

			//And everyone is the descendent of the root folder
			Assert.IsTrue(folder1.IsDescendentOf(_root));
			Assert.IsTrue(folder2.IsDescendentOf(_root));
			Assert.IsTrue(folder3.IsDescendentOf(_root));

			//Folder1 is an ancestor of neither folder2 nor folder3
			Assert.IsFalse(folder1.IsAncestorOf(_root));
			Assert.IsFalse(folder1.IsAncestorOf(folder1));
			Assert.IsFalse(folder1.IsAncestorOf(folder2));
			Assert.IsFalse(folder1.IsAncestorOf(folder3));

			//Folder2 is an ancestor of folder3
			Assert.IsFalse(folder2.IsAncestorOf(_root));
			Assert.IsFalse(folder2.IsAncestorOf(folder1));
			Assert.IsFalse(folder2.IsAncestorOf(folder2));
			Assert.IsTrue(folder2.IsAncestorOf(folder3));

			//Folder3 is an ancestor of no one
			Assert.IsFalse(folder3.IsAncestorOf(_root));
			Assert.IsFalse(folder3.IsAncestorOf(folder1));
			Assert.IsFalse(folder3.IsAncestorOf(folder2));
			Assert.IsFalse(folder3.IsAncestorOf(folder3));

			//Folder1 is a descendent of none of the folders
			Assert.IsFalse(folder1.IsDescendentOf(folder1));
			Assert.IsFalse(folder1.IsDescendentOf(folder2));
			Assert.IsFalse(folder1.IsDescendentOf(folder3));

			//Similarly folder2
			Assert.IsFalse(folder2.IsDescendentOf(folder1));
			Assert.IsFalse(folder2.IsDescendentOf(folder2));
			Assert.IsFalse(folder2.IsDescendentOf(folder3));

			//Folder3 is a descendent of folder2
			Assert.IsFalse(folder3.IsDescendentOf(folder1));
			Assert.IsTrue(folder3.IsDescendentOf(folder2));
			Assert.IsFalse(folder3.IsDescendentOf(folder3));
		}

	}
}
