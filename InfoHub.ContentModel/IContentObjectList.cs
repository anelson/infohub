using System;
using System.Collections;

namespace InfoHub.ContentModel
{
	/// <summary>
	/// A list of IContentObject's contained by an IContentContainer.
	/// </summary>
	public interface IContentObjectList : IList {
		/// <summary>
		/// The container who is the parent of all content objects
		/// within this list
		/// </summary>
		IContentContainer Parent {get;}

		new IContentObject this[ int index ]  {
			get;
			set;
		}

		int Add( IContentObject value );
		int IndexOf( IContentObject value );
		void Insert( int index, IContentObject value );
		void Remove( IContentObject value );
		bool Contains( IContentObject value );

		/// <summary>
		/// 'Carves out' a subset of the objects in this list and places them in another list, 
		/// calling IContentObject.Move on each effected item to update its parentage.
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="destIdx"></param>
		/// <param name="srcIdx"></param>
		/// <param name="srcLength"></param>
		void CarveOut(IContentObjectList dest, int destIdx, int srcIdx, int srcLength);
	}
}
