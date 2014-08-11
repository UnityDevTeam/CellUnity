using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Utility;
using System.Text;

namespace CellUnity.Reaction
{
	[System.Serializable]
	public class ReactionType : ScriptableObject {

		//public string Name = "";

		public float Rate;

		public MoleculeSpecies Reagent1;
		public MoleculeSpecies Reagent2;
		public MoleculeSpecies Reagent3;
		public MoleculeSpecies Product1;
		public MoleculeSpecies Product2;
		
		public MoleculeSpecies[] GetReagents() {
			if (Reagent2 == null)
			{ return new MoleculeSpecies[]{ Reagent1 }; }
			else if (Reagent3 == null)
			{ return new MoleculeSpecies[]{ Reagent1, Reagent2 }; }
			else
			{ return new MoleculeSpecies[]{ Reagent1, Reagent2, Reagent3 }; }
		}
		
		public MoleculeSpecies[] GetProducts() {
			if (Product2 == null)
			{ return new MoleculeSpecies[]{ Product1 }; }
			else
			{ return new MoleculeSpecies[]{ Product1, Product2 }; }
		}

		void OnEnable ()
		{
			hideFlags = HideFlags.HideInHierarchy;
		}

		private string SpeciesToString(MoleculeSpecies species)
		{
			if (species == null) { return "?"; }
			return species.Name;
		}

		public override string ToString ()
		{
			StringBuilder s = new StringBuilder();
			
			bool addPlus = false;
			foreach (MoleculeSpecies reagent in GetReagents()) {
				if (addPlus) { s.Append(" + "); }
				s.Append(SpeciesToString(reagent));
			
				addPlus = true;
			}
			
			s.Append(" \u2192 ");
			
			addPlus = false;
			foreach (MoleculeSpecies product in GetProducts()) {
				if (addPlus) { s.Append(" + "); }
				s.Append(SpeciesToString(product));
				
				addPlus = true;
			}
			
			return s.ToString();
		}

		public override int GetHashCode ()
		{
			return GetInstanceID ();
		}

		public override bool Equals (object o)
		{
			ReactionType other;
			if (Utils.TypeEquals<ReactionType> (this, o, out other)) {

				return 
					(GetInstanceID() == other.GetInstanceID()) &&
						(Object.Equals(Reagent1, other.Reagent1)) &&
						(Object.Equals(Reagent2, other.Reagent2)) &&
						(Object.Equals(Product1, other.Product1)) &&
						(Object.Equals(Product2, other.Product2));

			} else { return false; }
		}
	}
}