using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Reaction;
using CellUnity.Utility;

namespace CellUnity.Reaction
{
	public class ReactionManager {

		//void OnEnable ()
		//{
		//	hideFlags = HideFlags.HideInHierarchy;
		//}

		public void Collision(Molecule m1, Molecule m2)
		{
			if ((m1.ReactionPrep != null) && (m1.ReactionPrep == m2.ReactionPrep))
			{
				ReactionPrep reactionPrep = m1.ReactionPrep;
				
				reactionPrep.Ready(m1);
				reactionPrep.Ready(m2);
			}
		}

		private ShortKeyDict<ReactionType, int> openReactions = new ShortKeyDict<ReactionType, int>();

		public void PerformReaction(ReactionPrep reactionPrep)
		{
			// calculate center
			
			Vector3 center = Vector3.zero;
			float centerSum = 0;
		
			foreach (Molecule m in reactionPrep.Molecules) {
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
				momentum += m.rigidbody.velocity * m.Species.Mass;
			}
			
			float productMassSum = 0;
			float productSizeSum = 0;
			MoleculeSpecies[] productSpecies = reactionPrep.ReactionType.Products;
			
			foreach (MoleculeSpecies productS in productSpecies) {
				productMassSum += productS.Mass;
				productSizeSum += productS.Size;
			}
			
			Vector3 productVelocity = momentum / productMassSum;
			
			// create products

			Vector3 productPosition = center;
			foreach (MoleculeSpecies productS in productSpecies) {
				Molecule product = productS.CreateMolecule(productPosition);
				product.rigidbody.velocity = productVelocity;

				productPosition += new Vector3(product.Species.Size * 1.2f, 0, 0); // so products don't touch
			}			
			
			// flash
			
			GameObject flash = (GameObject)GameObject.Instantiate(LightFlash.GetPrefabObject(), Vector3.Lerp(center, Camera.main.gameObject.transform.position, 0.5f), Quaternion.identity);
			
			float intensity = 2f * (float)System.Math.Sqrt(productSizeSum);
			flash.GetComponent<LightFlash>().FinalIntensity = intensity;

			TryPerformOpenReaction ();
		}

		public bool InitiateReaction(ReactionType reaction, bool queueIfNotPossible)
		{
			CUE cue = CUE.GetInstance ();

			ReactionPrep reactionPrep = new ReactionPrep(reaction);
			if (cue.Molecules.FindMolecuelsForReaction(reactionPrep))
			{
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
				reactionPrep.Release();

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

		private void TryPerformOpenReaction()
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