using System;
using CellUnity.Reaction;

namespace CellUnity.Simulation.Update
{
	public abstract class ReactionUpdate : CueUpdate
	{
		public ReactionUpdate (ReactionType reaction)
		{
			this.Reaction = reaction;
		}

		protected override bool OnCanReplaceSameType (CueUpdate otherUpdate, out CueUpdate newUpdate)
		{

			if (((ReactionUpdate)otherUpdate).Reaction == Reaction)
			{
				return OnCanReplaceSameTypeSameReaction(otherUpdate, out newUpdate);
			}
			else
			{
				newUpdate = null;
				return false;
			}
		}

		protected virtual bool OnCanReplaceSameTypeSameReaction(CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			newUpdate = null;
			return false;
		}

		public ReactionType Reaction { get; private set; }
	}
}

