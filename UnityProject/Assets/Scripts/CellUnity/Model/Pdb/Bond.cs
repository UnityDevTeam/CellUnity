using UnityEngine;
using System.Collections;

namespace CellUnity.Model.Pdb
{
	/// <summary>
	/// Represents a Bond between two atoms in a PDB file
	/// </summary>
	public class Bond
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Model.Pdb.Bond"/> class.
		/// </summary>
		/// <param name="atom">Atom.</param>
		/// <param name="bondedAtom">Bonded atom.</param>
		public Bond(Atom atom, Atom bondedAtom)
		{
			this.Atom = atom;
			this.BondedAtom = bondedAtom;
		}

		/// <summary>
		/// Gets the atom.
		/// </summary>
		/// <value>The atom.</value>
		public Atom Atom { get; private set; }
		/// <summary>
		/// Gets the bonded atom.
		/// </summary>
		/// <value>The bonded atom.</value>
		public Atom BondedAtom { get; private set; }
		
		public override string ToString()
		{
			return Atom + " -- " + BondedAtom;
		}
	}
}