using UnityEngine;
using System.Collections;
using CellUnity.Utility;

namespace CellUnity.Reaction
{
	[System.Serializable]
	public class ReactionType : ScriptableObject {

		//public string Name = "";

		public float Rate;

		public MoleculeSpecies Reagent1;
		public MoleculeSpecies Reagent2;
		public MoleculeSpecies Product;

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
			return SpeciesToString (Reagent1) + " + " +
				SpeciesToString (Reagent2) + " \u2192 " +
				SpeciesToString (Product);
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
						(Object.Equals(Product, other.Product));

			} else { return false; }
		}
	}
}