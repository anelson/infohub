using System;
using System.IO;

using InfoHub.Common;
using InfoHub.ContentModel;

using NUnit.Framework;

namespace InfoHub.Tests.ContentModel {
	[TestFixture]
	/// <summary>
	/// Test fixture which tests the implementation of GenericDocument
	/// </summary>
	public class GenericDocumentTests : TestBase {
		IFolder _parent;
		IRootFolder _root;

		[SetUp]
		public void CreateParent() {
			_root = new GenericRootFolder("Foo");
			_parent = new GenericFolder(_root, "Bar");
		}

		[Test]
		public void GenericEmptyTest() {
			GenericDocument doc = new GenericDocument(_parent, "Foo", "text/plain");

			Assert.AreEqual(1, _parent.Children.Count);
			Assert.IsTrue(doc.IsDescendentOf(_parent));
			Assert.IsTrue(doc.IsDescendentOf(_root));
			Assert.AreEqual("Foo", doc.Name);
			Assert.AreEqual("text/plain", doc.MimeType);
			Assert.AreEqual(_parent.Path + Path.DirectorySeparatorChar.ToString() + doc.Name, doc.Path);
			Assert.AreEqual(_parent, doc.Parent);
			Assert.AreEqual(_parent, doc.ParentFolder);
			Assert.AreEqual(_parent, doc.FileSystemParent);
			Assert.AreEqual(_root, doc.ParentFolder.RootFolder);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullParentTest() {
			GenericDocument doc = new GenericDocument(null, "Foo", "text/plain");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullNameTest() {
			GenericDocument doc = new GenericDocument(_parent, null, "text/plain");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullMimeTypeTest() {
			GenericDocument doc = new GenericDocument(_parent, "Foo", null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullIsDescendentTest() {
			GenericDocument doc = new GenericDocument(_parent, "Foo", "text/plain");

			//Can't pass null to IsDescendentOf
			doc.IsDescendentOf(_parent);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MoveNullTest() {
			//attempt to move to a null folder
			//That is not permitted
			GenericDocument doc = new GenericDocument(_parent, "Foo", "text/plain");
			doc.Move(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void MoveNonFolderTest() {
			//attempt to move to an object that isn't a folder
			//That is not permitted
			GenericDocument doc = new GenericDocument(_parent, "Foo", "text/plain");
			GenericDocument doc2 = new GenericDocument(_parent, "Foo", "text/plain");
			GenericContentObject notMine = new GenericContentObject(doc2);
			doc.Move(notMine);
		}

		[Test]
		public void MoveTest() {
			GenericRootFolder root = new GenericRootFolder("Foo");
			GenericFolder folder1 = new GenericFolder(root, "Folder1");
			GenericFolder folder2 = new GenericFolder(root, "Folder2");
			GenericFolder folder3 = new GenericFolder(folder2, "Folder3");
			GenericDocument doc1 = new GenericDocument(folder1, "Doc1", "text/plain");
			GenericDocument doc2 = new GenericDocument(folder3, "Doc2", "text/plain");

			Assert.AreEqual(2, root.Children.Count);
			Assert.AreEqual(1, folder1.Children.Count);
			Assert.AreEqual(1, folder2.Children.Count);
			Assert.AreEqual(1, folder3.Children.Count);
			
			Assert.AreEqual(root, folder1.ParentFolder);
			Assert.AreEqual(root, folder2.ParentFolder);
			Assert.AreEqual(folder2, folder3.ParentFolder);
			Assert.AreEqual(folder1, doc1.ParentFolder);
			Assert.AreEqual(folder3, doc2.ParentFolder);

			//Move doc2 from folder1 to folder2
			Assert.AreEqual(Path.Combine(folder1.Path, doc1.Name), doc1.Path);
			doc1.Move(folder2);
			Assert.AreEqual(Path.Combine(folder2.Path, doc1.Name), doc1.Path);
			Assert.AreEqual(0, folder1.Children.Count);
			Assert.AreEqual(2, folder2.Children.Count);
			
			Assert.AreEqual(root, folder1.ParentFolder);
			Assert.AreEqual(root, folder2.ParentFolder);
			Assert.AreEqual(folder2, folder3.ParentFolder);
			Assert.AreEqual(folder2, doc1.ParentFolder);
			Assert.AreEqual(folder3, doc2.ParentFolder);
		}

		[Test]
		public void AncestryTest() {
			GenericRootFolder root = new GenericRootFolder("Foo");
			GenericFolder folder1 = new GenericFolder(root, "Folder1");
			GenericFolder folder2 = new GenericFolder(root, "Folder2");
			GenericFolder folder3 = new GenericFolder(folder2, "Folder3");
			GenericDocument doc1 = new GenericDocument(folder1, "Doc1", "text/plain");
			GenericDocument doc2 = new GenericDocument(folder3, "Doc2", "text/plain");

			//The root folder is the ancestor of everyone
			Assert.IsTrue(root.IsAncestorOf(doc1));
			Assert.IsTrue(root.IsAncestorOf(doc2));

			//And everyone is the descendent of the root folder
			Assert.IsTrue(doc1.IsDescendentOf(root));
			Assert.IsTrue(doc2.IsDescendentOf(root));

			//doc1 is a descendent of folder1
			Assert.IsTrue(doc1.IsDescendentOf(folder1));
			Assert.IsFalse(doc1.IsDescendentOf(folder2));
			Assert.IsFalse(doc1.IsDescendentOf(folder3));

			//doc2 is a descendent of folder2 and folder3
			Assert.IsFalse(doc2.IsDescendentOf(folder1));
			Assert.IsTrue(doc2.IsDescendentOf(folder2));
			Assert.IsTrue(doc2.IsDescendentOf(folder3));
		}
	}
}
