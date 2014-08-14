using System;
using CellUnity.Reaction;

namespace CellUnity.Simulation.Update
{
	public class ReactionChangedUpdate : ReactionUpdate
	{
		public ReactionChangedUpdate (ReactionType reaction) : base(reaction)
		{
			this.Reagents = reaction.Reagents;
			this.Products = reaction.Products;
			this.Rate = reaction.Rate;
		}

		public MoleculeSpecies[] Reagents { get; private set; }
		public MoleculeSpecies[] Products { get; private set; }
		public double Rate { get; private set; }

		protected override bool OnCanReplaceSameTypeSameReaction (CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			newUpdate = this;
			return true;
		}
	}
}

