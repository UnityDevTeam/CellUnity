using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Utility;
using System.Text;

namespace CellUnity.Reaction
{
	/// <summary>
	/// Represents a reaction type.
	/// </summary>
	[System.Serializable]
	public class ReactionType : ScriptableObject {

		/// <summary>
		/// Name of the Reaction
		/// </summary>
		public string Name = "";

		[SerializeField]
		private float rate = 1;

		/// <summary>
		/// Gets or sets the reaction rate in nl/(nmol*s)
		/// </summary>
		/// <value>The rate.</value>
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

		/// <summary>
		/// Gets or sets the reagents.
		/// Must not be null and must contain 1 or more species.
		/// </summary>
		/// <value>The reagents.</value>
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

		/// <summary>
		/// Gets or sets the reaction products.
		/// Must not be null.
		/// </summary>
		/// <value>The products.</value>
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
			
			s.Append(" \u2192 "); // arrow
			
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

		/// <summary>
		/// Gets an unique name of the reaction that is never empty or null
		/// </summary>
		/// <returns>The auto name.</returns>
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
			if (Utils.TypeEquals<ReactionType> (o, out other)) {

				return 
					(GetInstanceID() == other.GetInstanceID()) &&
						(Utils.ArrayEquals<MoleculeSpecies>(Reagents, other.Reagents)) &&
						(Utils.ArrayEquals<MoleculeSpecies>(Products, other.Products));

			} else { return false; }
		}
	}
}