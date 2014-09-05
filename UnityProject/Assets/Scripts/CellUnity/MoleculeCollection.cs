using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CellUnity
{
	/// <summary>
	/// Doubly linked List of Molecules.
	/// Uses the Molecule's CollectionNext, CollectionPrevious and Collection fields to link
	/// each Molecule can only be assigned to one MoleculeCollection at a time. MoleculeCollections
	/// are used in the MoleculeManager.
	/// </summary>
	public class MoleculeCollection : IEnumerable<Molecule>
	{
		public MoleculeCollection (string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Name of the collection, for debug reasons
		/// </summary>
		private string name;

		/// <summary>
		/// first item of the list, null when no item is assigned
		/// </summary>
		private Molecule root = null;
		/// <summary>
		/// last item of the list, null when no item is assigned
		/// </summary>
		private Molecule last = null;

		/// <summary>
		/// field of Property Count
		/// </summary>
		private ulong count = 0;
		/// <summary>
		/// Gets the count of the items in the list.
		/// </summary>
		/// <value>The count.</value>
		public ulong Count { get { return count; } }

		/// <summary>
		/// Add a molecule to the collection. Molecule must not be already in another
		/// collection.
		/// </summary>
		/// <param name="molecule">Molecule to add</param>
		public void Add(Molecule molecule)
		{
			if (molecule.Collection != null)
			{
				throw new System.Exception("molecule already bleongs to another collection (Add called for "+ToString()+", actually is in "+molecule.Collection.ToString()+")");
			}

			if (root == null)
			{
				root = molecule;
				last = molecule;

				molecule.CollectionNext = null;
				molecule.CollectionPrevious = null;
			}
			else
			{
				last.CollectionNext = molecule;

				molecule.CollectionPrevious = last;
				molecule.CollectionNext = null;

				last = molecule;
			}

			molecule.Collection = this;

			count++;
		}

		/// <summary>
		/// Removes a molecule from the collection.
		/// </summary>
		/// <param name="molecule">Molecule to remove. Molecule must be contained in this collection</param>
		public void Remove(Molecule molecule)
		{
			if (molecule.Collection != this)
			{
				throw new System.Exception("molecule already bleongs to another collection (Remove called for "+ToString()+", actually is in "+molecule.Collection.ToString()+")");
			}

			if (molecule == root && molecule == last)
			{
				// count == 1

				root = null;
				last = null;

				molecule.CollectionNext = null;
				molecule.CollectionPrevious = null;
				molecule.Collection = null;

				count--;
			}
			else if (molecule == root)
			{
				// item is first item and count > 1

				root = molecule.CollectionNext;
				root.CollectionPrevious = null;

				molecule.CollectionNext = null;
				molecule.CollectionPrevious = null;
				molecule.Collection = null;

				count--;
			}
			else if (molecule == last)
			{
				// item is last item and count > 1

				last = molecule.CollectionPrevious;
				last.CollectionNext = null;

				molecule.CollectionNext = null;
				molecule.CollectionPrevious = null;
				molecule.Collection = null;

				count--;
			}
			else
			{
				// item is not first and not last, therefore count > 2

				molecule.CollectionPrevious.CollectionNext = molecule.CollectionNext;
				molecule.CollectionNext.CollectionPrevious = molecule.CollectionPrevious;

				molecule.CollectionNext = null;
				molecule.CollectionPrevious = null;
				molecule.Collection = null;

				count--;
			}
		}

		/// <summary>
		/// Enumertor of a molecule collection
		/// </summary>
		private class Enumerator : IEnumerator<Molecule>
		{
			public Enumerator(MoleculeCollection collection)
			{
				this.collection = collection;
			}

			private MoleculeCollection collection;
			private Molecule current = null;
			private bool first = true;

			public bool MoveNext ()
			{
				if (first)
				{
					first = false;
					current = collection.root;
				}
				else
				{
					current = current.CollectionNext;
				}
				return (current != null);
			}
			public void Reset ()
			{
				throw new System.NotImplementedException ();
			}

			public void Dispose ()
			{
				current = null;
				collection = null;
			}

			public Molecule Current { get { return current; } }

			#region IEnumerator implementation

			object IEnumerator.Current  { get { return current; } }

			#endregion
		}

		public IEnumerator<Molecule> GetEnumerator ()
		{
			return new Enumerator (this);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return new Enumerator (this);
		}

		public override string ToString ()
		{
			return name;
		}
	}
}

