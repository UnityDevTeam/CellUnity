using UnityEngine;
using System.Collections;

namespace CellUnity.Model.Pdb
{
	/// <summary>
	/// Represents an Atom in a PDB file
	/// </summary>
	public class Atom
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Model.Pdb.Atom"/> class.
		/// </summary>
		/// <param name="element">Element</param>
		/// <param name="x">The x coordinate in nm</param>
		/// <param name="y">The y coordinate in nm</param>
		/// <param name="z">The z coordinate in nm</param>
		public Atom(Element element, float x, float y, float z)
		{
			this.Element = element;
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		/// <summary>
		/// Gets the x coordinate in nm.
		/// </summary>
		/// <value>x Value.</value>
		public float X { get; private set; }
		/// <summary>
		/// Gets the y coordinate in nm.
		/// </summary>
		/// <value>y Value.</value>
		public float Y { get; private set; }
		/// <summary>
		/// Gets the z coordinate in nm.
		/// </summary>
		/// <value>z Value.</value>
		public float Z { get; private set; }

		/// <summary>
		/// Gets the element of the Atom.
		/// </summary>
		/// <value>The element.</value>
		public Element Element { get; private set; }
		
		public override string ToString()
		{
			return Element.Symbol + " X: " + X + "; Y: " + Y + "; Z: " + Z;
		}
	}
}
