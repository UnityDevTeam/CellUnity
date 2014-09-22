using UnityEngine;
using System.Collections;

namespace CellUnity.Model.Pdb
{
	/// <summary>
	/// Represents a molecule defined in a PDB file.
	/// </summary>
	public class Molecule
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Model.Pdb.Molecule"/> class.
		/// </summary>
		/// <param name="name">Name of the molecule</param>
		/// <param name="source">Source of the molecule definition. e.g. URL or Filename of the PDB file</param>
		/// <param name="atoms">Atoms of the molecule</param>
		/// <param name="bonds">Bonds between the atoms</param>
		public Molecule(string name, string source, Atom[] atoms, Bond[] bonds)
		{
			this.Name = name;
			this.Source = source;
			this.Atoms = atoms;
			this.Bonds = bonds;
		}

		/// <summary>
		/// Gets the name of the molecule
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }
		/// <summary>
		/// Gets the source of the definition. e.g. URL or Filename of the PDB file
		/// </summary>
		/// <value>The source.</value>
		public string Source { get; private set; }

		/// <summary>
		/// Gets an array with all atoms of the molecule
		/// </summary>
		/// <value>The atoms.</value>
		public Atom[] Atoms { get; private set; }
		/// <summary>
		/// Gets the bonds between the atoms
		/// </summary>
		/// <value>The bonds.</value>
		public Bond[] Bonds { get; private set; }
	}
}