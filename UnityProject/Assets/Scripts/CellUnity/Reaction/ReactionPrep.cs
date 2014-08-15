using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CellUnity.Reaction
{
	public class ReactionPrep {

		public ReactionPrep(ReactionType t)
		{
			this.reactionType = t;

			int reagentsCount = t.Reagents.Length;
			this.ready = new bool[reagentsCount];
			this.molecules = new List<Molecule> (reagentsCount);

			for (int i = 0; i < ready.Length; i++)
			{
				ready[i] = false;	
			}
		}

		public void AddMolecule(Molecule molecule)
		{
			molecules.Add (molecule);
			molecule.AssignReactionPrep(this);
		}

		public void Release()
		{
			foreach (var m in molecules)
			{
				m.ClearReactionPrep();
			}

			molecules.Clear ();
		}

		public int MoleculeCount { get {  return molecules.Count - 1; } }

		private ReactionType reactionType;
		private List<Molecule> molecules;
		private bool[] ready;
		
		private bool performed = false;
		
		public ReactionType ReactionType { get{ return reactionType; } }
		public Molecule[] Molecules { get { return molecules.ToArray(); } }
		
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
		
			for (int i = 0; i < molecules.Count; i++) {
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