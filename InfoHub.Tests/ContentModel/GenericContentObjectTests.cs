using System;
using System.IO;

using InfoHub.Common;
using InfoHub.ContentModel;

using NUnit.Framework;

namespace InfoHub.Tests.ContentModel {
	[TestFixture]
	/// <summary>
	/// Test fixture which tests the implementation of GenericContentObject
	/// </summary>
	public class GenericContentObjectTests : TestBase {
		IFolder _parent;
		IRootFolder _root;
		IDocument _doc;

		[SetUp]
		public void CreateParent() {
			_root = new GenericRootFolder("Foo", ContentModelMockObjectFactory.CreateObjectPersistor());
			_parent = new GenericFolder(_root, "Bar");
			_doc = new GenericDocument(_parent,  "Boo",  "text/plain");
		}

		[Test]
		public void GenericEmptyTest() {
			GenericContentObject obj = new GenericContentObject(_doc);

			Assert.AreEqual(1, _doc.Children.Count);
			Assert.AreEqual(_doc, obj.FileSystemParent);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullParentTest() {
			GenericContentObject obj = new GenericContentObject(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MoveNullTest() {
			//attempt to move to a null parent
			//That is not permitted
			GenericContentObject obj = new GenericContentObject(_doc);
			obj.Move(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void MoveBogusIdxTest() {
			//attempt to move to a bogus idx at a parent
			//That is not permitted
			GenericContentObject obj = new GenericContentObject(_doc);
			GenericDocument doc2 = new GenericDocument(_parent, "Doc1", "text/plain");
			obj.Move(doc2, 10);
		}

		[Test]
		public void MoveTest() {
			GenericDocument doc1 = new GenericDocument(_parent, "Doc1", "text/plain");
			GenericDocument doc2 = new GenericDocument(_parent, "Doc2", "text/plain");
			GenericContentContainer contain = new GenericContentContainer(doc1);
			GenericContentObject obj = new GenericContentObject(contain);

			Assert.AreEqual(1, doc1.Children.Count);
			Assert.AreEqual(1, contain.Children.Count);
			Assert.AreEqual(0, doc2.Children.Count);
			
			Assert.AreEqual(contain, obj.Parent);
			Assert.AreEqual(doc1, obj.FileSystemParent);
			Assert.AreEqual(doc1, contain.Parent);

			//Move obj from doc1 to doc2
			contain.Move(doc2);
			Assert.AreEqual(0, doc1.Children.Count);
			Assert.AreEqual(1, doc2.Children.Count);
			
			Assert.AreEqual(contain, obj.Parent);
			Assert.AreEqual(doc2, obj.FileSystemParent);
			Assert.AreEqual(doc2, contain.Parent);
		}

		[Test]
		public void AncestryTest() {
			GenericDocument doc1 = new GenericDocument(_parent, "Doc1", "text/plain");
			GenericDocument doc2 = new GenericDocument(_parent, "Doc2", "text/plain");
			GenericContentContainer contain = new GenericContentContainer(doc1);
			GenericContentObject obj = new GenericContentObject(contain);

			//The root folder is the ancestor of everyone
			Assert.IsTrue(_root.IsAncestorOf(contain.FileSystemParent));
			Assert.IsTrue(_root.IsAncestorOf(obj.FileSystemParent));

			//And everyone is the descendent of the root folder
			Assert.IsTrue(contain.FileSystemParent.IsDescendentOf(_root));
			Assert.IsTrue(obj.FileSystemParent.IsDescendentOf(_root));
		}
	}
}
