using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CellUnity.Model.Pdb
{
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
			
			elements.Add(symbol, this);
		}
		
		public string Symbol { get; private set; }
		public float Radius { get; private set; }
		public float VdWRadius { get; private set; }
		public float Mass { get; private set; }
		
		public override string ToString()
		{
			return Symbol+" (radius: "+Radius+"nm, vdwRadius: "+VdWRadius+"nm)";
		}
		
		private static Dictionary<string, Element> elements = new Dictionary<string,Element>();
		
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
		
		public static readonly Element Default = new Element("?", 60/1000f, 152/1000f, 15.999f);
		public static readonly Element H = new Element("H", 25/1000f, 120/1000f, 1.008f);
		public static readonly Element O = new Element("O", 60/1000f, 152/1000f, 15.999f);
		public static readonly Element C = new Element("C", 70/1000f, 170/1000f, 12.011f);
		public static readonly Element N = new Element("N", 65/1000f, 155/1000f, 14.007f);
		public static readonly Element S = new Element("S", 100/1000f, 102.5f/1000f, 32.06f);
		public static readonly Element P = new Element("P", 100/1000f, 180/1000f, 30.974f);
	}
}