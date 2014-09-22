using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Reaction;
using CellUnity.Utility;

namespace CellUnity.Reaction
{
	/// <summary>
	/// The assignment of the ReactionManager is to initiate reactions and to perform
	/// them when all reagents collide. Reactions are usually initiated by the
	/// simulation manager.
	/// </summary>
	public class ReactionManager {

		/// <summary>
		/// Handle the collision of two molecules
		/// </summary>
		/// <param name="m1">molecule 1</param>
		/// <param name="m2">molecule 2</param>
		public void Collision(Molecule m1, Molecule m2)
		{
			// check if ReactionPrep of both molecules is not null and if the ReactionPrep equals
			if ((m1.ReactionPrep != null) && (m1.ReactionPrep == m2.ReactionPrep))
			{
				// both molecules have the same ReactionPrep assigned and therefore
				// belong to the same reaction

				ReactionPrep reactionPrep = m1.ReactionPrep;

				// make the molecules ready for reacting
				reactionPrep.Ready(m1);
				reactionPrep.Ready(m2);
			}
		}

		private Molecule selectedMolecule;
		private ReactionType selectedReaction;

		/// <summary>
		/// Gets or sets the selected molecule.
		/// A selected molecule is prefrerred for reactions.
		/// When set SelectedReaction is set to null.
		/// </summary>
		/// <value>The selected molecule.</value>
		public Molecule SelectedMolecule
		{
			get 
			{
				if (selectedMolecule != null && selectedMolecule.Collection == null) // means that the Molecule has been deleted
				{ SelectedMolecule = null; }

				return selectedMolecule;
			}
			set
			{
				if (value != selectedMolecule)
				{
					selectedMolecule = value;
					selectedReaction = null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the selected reaction.
		/// A selected molecule and a selected reaction is defined,
		/// the selected molecule is prefered for a this type of reaction
		/// </summary>
		/// <value>The selected reaction.</value>
		public ReactionType SelectedReaction
		{
			get { return selectedReaction; }
			set
			{
				if (value != selectedReaction)
				{
					selectedReaction = value;
				}
			}
		}

		/// <summary>
		/// Queues reactions that could not be executed when initiated.
		/// </summary>
		private ShortKeyDict<ReactionType, int> openReactions = new ShortKeyDict<ReactionType, int>();

		/// <summary>
		/// Performs a reaction.
		/// Called by the ReactionPrep when all molecules are ready.
		/// </summary>
		/// <param name="reactionPrep">Reaction prep.</param>
		public void PerformReaction(ReactionPrep reactionPrep)
		{
			bool selectedInvolved = false;

			// calculate center
			
			Vector3 center = Vector3.zero;
			float centerSum = 0;
		
			foreach (Molecule m in reactionPrep.Molecules) {

				selectedInvolved |= (m == SelectedMolecule);

				m.ClearReactionPrep();
				
				center += m.Position * m.Species.Size;
				centerSum += m.Species.Size;

				GameObject.Destroy(m.gameObject);
			}

			if (centerSum > float.Epsilon) // avoid division by 0
			{
				center = center / centerSum;
			} 
			
			// momentum conservation
			
			Vector3 momentum = Vector3.zero;
			
			foreach (Molecule m in reactionPrep.Molecules) {
				momentum += m.rigidbody.velocity; //* m.Species.Mass;  // ignore mass (because it doesnt work properly when a very big and a very small molecule collide
			}
			
			float productMassSum = 0;
			float productSizeSum = 0;
			MoleculeSpecies[] productSpecies = reactionPrep.ReactionType.Products;
			
			foreach (MoleculeSpecies productS in productSpecies) {
				productMassSum += productS.Mass;
				productSizeSum += productS.Size;
			}
			
			Vector3 productVelocity = momentum; // / productMassSum; // ignore mass (because it doesnt work properly when a very big and a very small molecule collide
			
			// create products

			Vector3 productPosition = center;
			foreach (MoleculeSpecies productS in productSpecies) {
				Molecule product = productS.CreateMolecule(productPosition);
				product.rigidbody.velocity = productVelocity;

				productPosition += new Vector3(product.Species.Size * 1.2f, 0, 0); // so products don't touch

				if (selectedInvolved)
				{
					selectedInvolved = false;
					
					SelectedMolecule = product;
				}
			}			

			if (selectedInvolved)
			{
				selectedInvolved = false;
				SelectedMolecule = null;
			}

			// flash
			
			GameObject flash = (GameObject)GameObject.Instantiate(LightFlash.GetPrefabObject(), Vector3.Lerp(center, Camera.main.gameObject.transform.position, 0.5f), Quaternion.identity);
			
			float intensity = 2f * (float)System.Math.Sqrt(productSizeSum);
			flash.GetComponent<LightFlash>().FinalIntensity = intensity;

			TryInitiateOpenReactions ();
		}

		/// <summary>
		/// Initiates a reaction.
		/// </summary>
		/// <returns><c>true</c>, if the reaction was initiated, <c>false</c> otherwise.</returns>
		/// <param name="reaction">Reaction.</param>
		/// <param name="queueIfNotPossible">If set to <c>true</c>, the reaction is queued and performed later if the reaction is not possible at the moment.</param>
		public bool InitiateReaction(ReactionType reaction, bool queueIfNotPossible)
		{
			CUE cue = CUE.GetInstance ();

			ReactionPrep reactionPrep = new ReactionPrep(reaction);

			bool moleculesFound;

			// Select molecules

			// Prefer selected molecule if suitable
			if (
				selectedMolecule != null && selectedReaction != null && // molecule and reaction must be selected
				selectedReaction == reaction && 						// selected reaction must match current reaction
				selectedMolecule.ReactionPrep == null					// selected molecule must not be already involved in a reaction
				) 
			{
				moleculesFound = cue.Molecules.FindMolecuelsForReaction(reactionPrep, selectedMolecule);
			}
			else
			{
				// select random molecules
				moleculesFound = cue.Molecules.FindMolecuelsForReaction(reactionPrep);
			}

			if (moleculesFound)
			{
				// Ready reactions for 0 to 1 reagents because Collision is never called for these molecules

				if (reactionPrep.MoleculeCount == 0)
				{
					reactionPrep.Ready(null);
				}
				if (reactionPrep.MoleculeCount == 1)
				{
					reactionPrep.Ready(reactionPrep.Molecules[0]); // necessary if only one reagent in reaction
				}

				return true;
			}
			else
			{ 
				// not sufficient molecules for reaction

				reactionPrep.Release(); // remove ReactonPrep from Molecules

				if (queueIfNotPossible)
				{
					// --> queue Reaction for later

					ShortKeyDict<ReactionType, int>.Entry entry;
					if (!openReactions.Find (reaction, out entry))
					{
						entry = openReactions.Set(reaction, 0);
					}

					entry.Value++;
				}

				return false;
			}
		}

		/// <summary>
		/// Tries to initiate queued reactions.
		/// </summary>
		private void TryInitiateOpenReactions()
		{
			ShortKeyDict<ReactionType, int>.Entry entry = null;
			while (openReactions.GetNext(entry, out entry))
			{
				while (entry.Value > 0)
				{
					if (InitiateReaction(entry.Key, false))
					{
						entry.Value--;
						if (entry.Value == 0)
						{
							openReactions.Remove(entry.Key);
							break;
						}
					}
					else
					{ break; }
				}
			}
		}
	}
}