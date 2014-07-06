using UnityEngine;
using System.Collections;

namespace CellUnity.Reaction
{
	public class ReactionPrep {

		public ReactionPrep(ReactionType t, Molecule a, Molecule b)
		{
			this.ReactionType = t;
			this.A = a;
			this.B = b;
		}

		public ReactionType ReactionType;
		public Molecule A;
		public Molecule B;
	}
}