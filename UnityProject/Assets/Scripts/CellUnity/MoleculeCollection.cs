using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CellUnity
{
	public class MoleculeCollection : IEnumerable<Molecule>
	{
		public MoleculeCollection ()
		{
		}

		private Molecule root = null;
		private Molecule last = null;

		private ulong count = 0;
		public ulong Count { get { return count; } }

		public void Add(Molecule molecule)
		{
			if (molecule.Collection != null)
			{ throw new System.Exception("molecule already bleongs to another collection (Add called for "+ToString()+", actually is in "+molecule.Collection.ToString()+")"); }

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

		public void Remove(Molecule molecule)
		{
			if (molecule.Collection != this)
			{ throw new System.Exception("molecule already bleongs to another collection (Remove called for "+ToString()+", actually is in "+molecule.Collection.ToString()+")"); }

			if (molecule == root && molecule == last)
			{
				root = null;
				last = null;

				molecule.CollectionNext = null;
				molecule.CollectionPrevious = null;
				molecule.Collection = null;

				count--;
			}
			else if (molecule == root)
			{
				root = molecule.CollectionNext;
				root.CollectionPrevious = null;

				molecule.CollectionNext = null;
				molecule.CollectionPrevious = null;
				molecule.Collection = null;

				count--;
			}
			else if (molecule == last)
			{
				last = molecule.CollectionPrevious;
				last.CollectionNext = null;

				molecule.CollectionNext = null;
				molecule.CollectionPrevious = null;
				molecule.Collection = null;

				count--;
			}
			else
			{
				molecule.CollectionPrevious.CollectionNext = molecule.CollectionNext;
				molecule.CollectionNext.CollectionPrevious = molecule.CollectionPrevious;

				molecule.CollectionNext = null;
				molecule.CollectionPrevious = null;
				molecule.Collection = null;

				count--;
			}
		}

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

	}
}

