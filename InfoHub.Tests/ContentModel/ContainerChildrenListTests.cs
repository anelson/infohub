using System;
using System.IO;

using InfoHub.Common;
using InfoHub.ContentModel;

using NUnit.Framework;

namespace InfoHub.Tests.ContentModel {
	[TestFixture]
	/// <summary>
	/// Test fixture which tests the implementation of IContentObjectList
	/// as provided by ContainerChildrenList.  THis is an internal class, however, 
	/// therefore it can only be accessed indirectly via AbstractContentContainer
	/// and its subclasses.
	/// </summary>
	public class ContainerChildrenListTests : TestBase {
		
		IFolder _parent;
		IRootFolder _root;
		IDocument _doc;
		IContentContainer _cont;

		[SetUp]
		public void CreateParent() {
			_root = new GenericRootFolder("Foo");
			_parent = new GenericFolder(_root, "Bar");
			_doc = new GenericDocument(_parent,  "Boo",  "text/plain");
			_cont = new GenericContentContainer(_doc);
		}

		[Test]
		public void EmptyListTest() {
			Assert.AreEqual(0, _cont.Children.Count);
			Assert.IsFalse(_cont.Children.Contains(_cont));
			Assert.AreEqual(-1, _cont.Children.IndexOf(_cont));
		}

		[Test]
		public void ParentTest() {
			Assert.AreEqual(_cont.Children.Parent);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void AddNullObjectTest() {
			//Null children aren't allowed either
			_cont.Children.Add(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void AddNonContentObjectTest() {
			//Add something to the Children list that isn't an IContentObject-derivative
			_cont.Children.Add("Give me a reason");
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void SetWithIndexerTest() {
			//Using the indexer to set an element is not allowed
			_cont.Children[0] = new GenericContentObject(_cont);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void GetWithInvalidIndexTest() {
			//The indexer is ok for retreiving elements, but only if the index is valid
			Assert.IsNotNull(_cont.Children[100]);
		}

		[Test]
		public void AddItemTest() {
			GenericContentObject obj = new GenericContentObject(_cont);

			Assert.AreEqual(1, _cont.Children.Count);
			Assert.IsTrue(_cont.Children.Contains(obj));
			Assert.AreEqual(obj,  _cont.Children[0]);
			Assert.AreEqual(0, _cont.Children.IndexOf(obj));
		}

		[Test]
		public void RemoveItemTest() {
			GenericContentObject obj = new GenericContentObject(_cont);
			_cont.Children.Remove(obj);

			Assert.AreEqual(0, _cont.Children.Count);
			Assert.IsFalse(_cont.Children.Contains(obj));
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void CarveOutEmptyListTest() {
			//When the list is empty, any attempt to carve out of it with a non-zero
			//length will fail
			GenericDocument doc2 = new GenericDocument(_parent,  "Baz",  "text/plain");

			_cont.Children.CarveOut(doc2.Children,  
									0, 
									0, 
									1);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void CarveOutBadDestIdxTest() {
			//Pass an invalid index into the dest list
			GenericDocument doc2 = new GenericDocument(_parent,  "Baz",  "text/plain");
			GenericContentObject obj1 = GenericContentObject(_cont);
			GenericContentObject obj2 = GenericContentObject(_cont);
			GenericContentObject obj3 = GenericContentObject(_cont);
			GenericContentObject obj4 = GenericContentObject(_cont);

			_cont.Children.CarveOut(doc2.Children,  
									10, 
									0, 
									1);
		}

		[Test]
		public void CarveOutEntireListTest() {
			//Carve out the entire contents of the source list into the destination
			GenericDocument doc2 = new GenericDocument(_parent,  "Baz",  "text/plain");
			GenericContentObject obj1 = GenericContentObject(_cont);
			GenericContentObject obj2 = GenericContentObject(_cont);
			GenericContentObject obj3 = GenericContentObject(_cont);
			GenericContentObject obj4 = GenericContentObject(_cont);
			
			_cont.Children.CarveOut(doc2.Children,  
									0, 
									0, 
									_cont.Children.Count);

			Assert.AreEqual(0,  _cont.Children.Count);
			Assert.AreEqual(4,  doc2.Children.Count);
			foreach (IContentObject obj in doc2.Children) {
				Assert.AreEqual(doc2,  obj.Parent);
			}
		}

		[Test]
		public void CarveOutPartialListTest() {
			//Carve out the part of the source list into the destination
			GenericDocument doc2 = new GenericDocument(_parent,  "Baz",  "text/plain");
			GenericContentObject obj1 = GenericContentObject(_cont);
			GenericContentObject obj2 = GenericContentObject(_cont);
			GenericContentObject obj3 = GenericContentObject(_cont);
			GenericContentObject obj4 = GenericContentObject(_cont);
			
			_cont.Children.CarveOut(doc2.Children,  
									0, 
									0, 
									2);

			Assert.AreEqual(2,  _cont.Children.Count);
			Assert.AreEqual(2,  doc2.Children.Count);
			
			foreach (IContentObject obj in _cont.Children) {
				Assert.AreEqual(_cont,  obj.Parent);
			}
			
			foreach (IContentObject obj in doc2.Children) {
				Assert.AreEqual(doc2,  obj.Parent);
			}
		}
	}
}
