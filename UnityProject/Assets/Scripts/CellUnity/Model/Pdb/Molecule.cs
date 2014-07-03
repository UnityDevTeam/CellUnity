using UnityEngine;
using System.Collections;

namespace CellUnity.Model.Pdb
{
	public class Molecule
	{
		public Molecule(string name, string source, Atom[] atoms, Bond[] bonds)
		{
			this.Name = name;
			this.Source = source;
			this.Atoms = atoms;
			this.Bonds = bonds;
		}
		
		public string Name { get; private set; }
		public string Source { get; private set; }
		
		public Atom[] Atoms { get; private set; }
		public Bond[] Bonds { get; private set; }
	}
}