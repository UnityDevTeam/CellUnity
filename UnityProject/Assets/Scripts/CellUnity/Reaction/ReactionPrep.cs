using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CellUnity.Reaction
{
	/// <summary>
	/// Reaction Preparation.
	/// Created when a reaction is initiated. Contains all necessary information
	/// for a specific reaction.
	/// </summary>
	public class ReactionPrep {

		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Reaction.ReactionPrep"/> class.
		/// </summary>
		/// <param name="t">Reaction Type</param>
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

		/// <summary>
		/// Adds a molecule to the reaction prep.
		/// AssignReactionPrep method of the molecule is called.
		/// </summary>
		/// <param name="molecule">Molecule.</param>
		public void AddMolecule(Molecule molecule)
		{
			molecules.Add (molecule);
			molecule.AssignReactionPrep(this);
		}

		/// <summary>
		/// ReleaseReactionPrep method of all molecules is called.
		/// </summary>
		public void Release()
		{
			foreach (var m in molecules)
			{
				m.ReleaseReactionPrep();
			}

			molecules.Clear ();
		}

		/// <summary>
		/// True if all necessary molecules are added and the reaction is not
		/// yet performed.
		/// </summary>
		/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
		public bool Active { get { return (molecules.Count == ready.Length) && !performed; } }

		/// <summary>
		/// Number of molecules added.
		/// </summary>
		/// <value>The molecule count.</value>
		public int MoleculeCount { get {  return molecules.Count; } }

		private ReactionType reactionType;
		private List<Molecule> molecules;
		private bool[] ready;
		
		private bool performed = false;

		/// <summary>
		/// Gets the type of the reaction.
		/// </summary>
		/// <value>The type of the reaction.</value>
		public ReactionType ReactionType { get{ return reactionType; } }
		/// <summary>
		/// Gets the molecules involved in the reaction
		/// </summary>
		/// <value>The molecules.</value>
		public Molecule[] Molecules { get { return molecules.ToArray(); } }

		/// <summary>
		/// Gets the expected location of the reaction, where all molecules collide.
		/// </summary>
		/// <returns>The expected reaction location.</returns>
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

		/// <summary>
		/// Set a molecule ready for reactiong.
		/// Called when molecules collide with each other.
		/// When all molecules are ready, cue.ReactionManager.PerformReaction is called.
		/// </summary>
		/// <param name="molecule">Molecule.</param>
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