using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Utility;
using System.Text;

namespace CellUnity.Reaction
{
	[System.Serializable]
	public class ReactionType : ScriptableObject {

		public string Name = "";

		[SerializeField]
		private float rate = 1;

		public float Rate
		{
			get { return rate; }
			set
			{
				if (value != rate)
				{
					this.rate = value;
				}
			}
		}

		[SerializeField]
		private MoleculeSpecies[] reagents = new MoleculeSpecies[] {};

		public MoleculeSpecies[] Reagents
		{
			get { return reagents; }
			set
			{
				if (!Utils.ArrayEquals<MoleculeSpecies>(reagents, value))
				{
					this.reagents = value;
				}
			}
		}

		[SerializeField]
		private MoleculeSpecies[] products = new MoleculeSpecies[] {};
		public MoleculeSpecies[] Products
		{
			get { return products; }
			set
			{
				if (!Utils.ArrayEquals<MoleculeSpecies>(products, value))
				{
					this.products = value;
				}
			}
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
			foreach (MoleculeSpecies reagent in Reagents) {
				if (addPlus) { s.Append(" + "); }
				s.Append(SpeciesToString(reagent));
			
				addPlus = true;
			}
			
			s.Append(" \u2192 ");
			
			addPlus = false;
			foreach (MoleculeSpecies product in Products) {
				if (addPlus) { s.Append(" + "); }
				s.Append(SpeciesToString(product));
				
				addPlus = true;
			}
			
			if (string.IsNullOrEmpty(Name))
			{
				return s.ToString();
			}
			else
			{
				return Name + " ("+s.ToString()+")";
			}
			
		}
		
		public string GetAutoName()
		{
			return Name + "reaction"+Mathf.Abs(this.GetInstanceID()).ToString();	
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
						(Utils.ArrayEquals<MoleculeSpecies>(Reagents, other.Reagents)) &&
						(Utils.ArrayEquals<MoleculeSpecies>(Products, other.Products));

			} else { return false; }
		}
	}
}