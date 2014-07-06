using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Reaction;

namespace CellUnity
{
	[System.Serializable]
	public class ReactionManager {

		private CUE cue = CUE.GetInstance();

		//void OnEnable ()
		//{
		//	hideFlags = HideFlags.HideInHierarchy;
		//}

		public void Collision(Molecule a, Molecule b)
		{
			Debug.Log ("Collision: " + a.ToString () + "; " + b.ToString ());
		}

		//private List<ReactionPrep> activeReactions = new List<ReactionPrep> ();

		public void PerformReaction(ReactionType reaction)
		{
			Molecule a = cue.Molecules.FindRandomMoleculeForReaction (reaction.Reagent1);
			if (a == null) {
				Debug.LogError("no free Molecule for Reagent1 found");
				return;
			}

			Molecule b = cue.Molecules.FindNearestMoleculeForReaction (a, reaction.Reagent2);
			if (b == null) {
				Debug.LogError("no free Molecule for Reagent2 found");
				return;
			}

			ReactionPrep reactionPrep = new ReactionPrep (reaction, a, b);

			a.ReactionPrep = reactionPrep;
			b.ReactionPrep = reactionPrep;

			//activeReactions.Add (reactionPrep);
		}
	}
}