using System;
using CellUnity.Utility;
using CellUnity.Reaction;

namespace CellUnity.Simulation.Update
{
	public class ReactionRemovedUpdate : ReactionUpdate
	{
		public ReactionRemovedUpdate (ReactionType reaction) : base(reaction)
		{ }

		protected override bool OnCanReplaceSameTypeSameReaction (CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			newUpdate = this;
			return true;
		}
	}
}

