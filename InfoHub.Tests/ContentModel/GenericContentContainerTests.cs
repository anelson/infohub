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
	public class GenericContentContainerTests : TestBase {
		IFolder _parent;
		IRootFolder _root;
		IDocument _doc;

		[SetUp]
		public void CreateParent() {
			_root = new GenericRootFolder("Foo");
			_parent = new GenericFolder(_root, "Bar");
			_doc = new GenericDocument(_parent,  "Boo",  "text/plain");
		}

		[Test]
		public void GenericEmptyTest() {
			GenericContentContainer contain = new GenericContentContainer(_doc);

			Assert.AreEqual(1, _doc.Children.Count);
			Assert.AreEqual(_doc, contain.FileSystemParent);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void NullParentTest() {
			GenericContentContainer contain = new GenericContentContainer(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MoveNullTest() {
			//attempt to move to a null parent
			//That is not permitted
			GenericContentContainer contain = new GenericContentContainer(_doc);
			contain.Move(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void MoveBogusIdxTest() {
			//attempt to move to a bogus idx at a parent
			//That is not permitted
			GenericContentContainer contain = new GenericContentContainer(_doc);
			GenericDocument doc2 = new GenericDocument(_parent, "Doc1", "text/plain");
			contain.Move(doc2, 10);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void AddUnrelatedObjectTest() {
			//Add a content object to the container's Children list that
			//has a Parent other than the container
			GenericContentContainer contain = new GenericContentContainer(_doc);
			GenericContentObject notMine = new GenericContentObject(_doc);
			contain.Children.Add(notMine);
		}

		[Test]
		public void MoveTest() {
			GenericDocument doc1 = new GenericDocument(_parent, "Doc1", "text/plain");
			GenericDocument doc2 = new GenericDocument(_parent, "Doc2", "text/plain");
			GenericContentContainer contain = new GenericContentContainer(doc1);

			Assert.AreEqual(1, doc1.Children.Count);
			Assert.AreEqual(0, doc2.Children.Count);
			
			Assert.AreEqual(doc1, contain.Parent);
			Assert.AreEqual(doc1, contain.FileSystemParent);

			//Move obj from doc1 to doc2
			contain.Changed += new ContentObjectChangedEventHandler(contain_Changed);
			contain.Move(doc2);
			contain.Changed -= new ContentObjectChangedEventHandler(contain_Changed);
			Assert.AreEqual(true, contain.PropertyBag["contain_Changed.fired"]);
			Assert.AreEqual(0, doc1.Children.Count);
			Assert.AreEqual(1, doc2.Children.Count);
			
			Assert.AreEqual(doc2, contain.Parent);
			Assert.AreEqual(doc2, contain.FileSystemParent);
		}

		[Test]
		public void AncestryTest() {
			GenericDocument doc1 = new GenericDocument(_parent, "Doc1", "text/plain");
			GenericDocument doc2 = new GenericDocument(_parent, "Doc2", "text/plain");
			GenericContentContainer contain = new GenericContentContainer(doc1);

			//The root folder is the ancestor of everyone
			Assert.IsTrue(_root.IsAncestorOf(contain.FileSystemParent));

			//And everyone is the descendent of the root folder
			Assert.IsTrue(contain.FileSystemParent.IsDescendentOf(_root));
		}

		private void contain_Changed(object sender, EventArgs e) {
			((IContentContainer)sender).PropertyBag["contain_Changed.fired"] = true;
		}
	}
}
