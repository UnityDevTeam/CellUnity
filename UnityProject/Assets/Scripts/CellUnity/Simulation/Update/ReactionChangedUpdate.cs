using System;
using CellUnity.Reaction;

namespace CellUnity.Simulation.Update
{
	public class ReactionChangedUpdate : ReactionUpdate
	{
		public ReactionChangedUpdate (ReactionType reaction) : base(reaction)
		{
		}

		protected override bool OnCanReplaceSameTypeSameReaction (CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			newUpdate = this;
			return true;
		}
	}
}

