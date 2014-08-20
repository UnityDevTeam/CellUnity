using UnityEngine;
using System.Collections;

namespace CellUnity.Model.Pdb
{
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
		
		public float X { get; private set; }
		public float Y { get; private set; }
		public float Z { get; private set; }
		
		public Element Element { get; private set; }
		
		public override string ToString()
		{
			return Element.Symbol + " X: " + X + "; Y: " + Y + "; Z: " + Z;
		}
	}
}
