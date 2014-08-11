using UnityEngine;
using System.Collections;

namespace CellUnity.Reaction
{
	public class ReactionPrep {

		public ReactionPrep(ReactionType t, Molecule[] molecules)
		{
			this.reactionType = t;
			this.molecules = molecules;
			this.ready = new bool[molecules.Length];
			for (int i = 0; i < molecules.Length; i++) {
				ready[i] = false;
				molecules[i].ReactionPrep = this;
			}
		}

		private ReactionType reactionType;
		private Molecule[] molecules;
		private bool[] ready;
		
		private bool performed = false;
		
		public ReactionType ReactionType { get{ return reactionType; } }
		public Molecule[] Molecules { get { return molecules; } }
		
		public Vector3 GetExpectedReactionLocation()
		{
			Vector3 center = Vector3.zero;
			float centerSum = 0;
			
			// calculate center
			
			foreach (Molecule m in molecules) {				
				center += m.Position * m.Species.Mass;
				centerSum += m.Species.Mass;
			}
			
			center = center / centerSum;
			
			return center;
		}
		
		public void Ready(Molecule molecule)
		{
			bool allReady = true;
		
			for (int i = 0; i < molecules.Length; i++) {
				if (molecule == molecules[i])
				{
					ready[i] = true;
					
					if (!allReady) { break; }
				}
				
				allReady = allReady & ready[i];
			}
			
			if (allReady && !performed)
			{
				performed = true;
				
				CUE cue = CUE.GetInstance();
				cue.ReactionManager.PerformReaction(this);
			}
		}
	}
}