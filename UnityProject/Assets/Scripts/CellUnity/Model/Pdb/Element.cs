using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CellUnity.Model.Pdb
{
	/// <summary>
	/// Represents the Element of an Atom.
	/// There should always be only one instance of each Element.
	/// Therefore, once a element is created, use the Element.BySmbol(...) method
	/// to get a element
	/// </summary>
	public class Element
	{
		/// <summary>
		/// Creates a new Element and inserts it in the elements-Dictionary
		/// </summary>
		/// <param name="symbol">Symbol</param>
		/// <param name="radius">Radius of the Atom in nm</param>
		private Element(string symbol, float radius, float vdwRadius, float mass) {
			this.Symbol = symbol;
			this.Radius = radius;
			this.VdWRadius = vdwRadius;
			this.Mass = mass;

			// add element to static dictionary
			elements.Add(symbol, this);
		}

		/// <summary>
		/// Gets the symbol of the element as defined in the periodic table
		/// </summary>
		/// <value>The symbol.</value>
		public string Symbol { get; private set; }
		/// <summary>
		/// Gets the theoretical radius of an atom of this element in nm.
		/// </summary>
		/// <value>The radius.</value>
		public float Radius { get; private set; }
		/// <summary>
		/// Gets the Van der Waals radius of an atom of this element in nm.
		/// </summary>
		/// <value>The Van der Waals radius.</value>
		public float VdWRadius { get; private set; }
		/// <summary>
		/// Gets the mass of an atom of this element in u (unified atomic mass unit).
		/// </summary>
		/// <value>The mass.</value>
		public float Mass { get; private set; }
		
		public override string ToString()
		{
			return Symbol+" (radius: "+Radius+"nm, vdwRadius: "+VdWRadius+"nm)";
		}

		/// <summary>
		/// Dictionary containing all elements by their symbol.
		/// </summary>
		private static Dictionary<string, Element> elements = new Dictionary<string,Element>();

		/// <summary>
		/// Returns a element by its symbol. Every element instance created can be found that way.
		/// If the symbol is unknown, Element.Default is returned.
		/// </summary>
		/// <returns>Element with defined Symbol.</returns>
		/// <param name="symbol">Symbol.</param>
		public static Element BySymbol(string symbol) {
			Element element;
			if (elements.TryGetValue(symbol, out element))
			{
				return element;
			}
			else
			{
				return Element.Default;
			}
		}

		/// <summary>
		/// Element that is used when the desired element could not be found.
		/// </summary>
		public static readonly Element Default = new Element("?", 60/1000f, 152/1000f, 15.999f);
		/// <summary>
		/// Hydrogen
		/// </summary>
		public static readonly Element H = new Element("H", 25/1000f, 120/1000f, 1.008f);
		/// <summary>
		/// Oxygen
		/// </summary>
		public static readonly Element O = new Element("O", 60/1000f, 152/1000f, 15.999f);
		/// <summary>
		/// Carbon
		/// </summary>
		public static readonly Element C = new Element("C", 70/1000f, 170/1000f, 12.011f);
		/// <summary>
		/// Nitrogen
		/// </summary>
		public static readonly Element N = new Element("N", 65/1000f, 155/1000f, 14.007f);
		/// <summary>
		/// Sulfur
		/// </summary>
		public static readonly Element S = new Element("S", 100/1000f, 102.5f/1000f, 32.06f);
		/// <summary>
		/// Phosphorus
		/// </summary>
		public static readonly Element P = new Element("P", 100/1000f, 180/1000f, 30.974f);
	}
}