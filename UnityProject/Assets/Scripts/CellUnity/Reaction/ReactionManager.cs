using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Reaction;

namespace CellUnity
{
	[System.Serializable]
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
		
		private GameObject CreateProduct(MoleculeSpecies productSpecies, Vector3 position, Vector3 velocity)
		{
			GameObject product = null;
		

			
			return product;
		}

		public void PerformReaction(ReactionPrep reactionPrep)
		{
			// calculate center
			
			Vector3 center = Vector3.zero;
			float centerSum = 0;
		
			foreach (Molecule m in reactionPrep.Molecules) {
				m.ReactionPrep = null;
				
				center += m.Position * m.Species.Size;
				centerSum += m.Species.Size;
				
				GameObject.Destroy(m.gameObject);
			}

			center = center / centerSum;
			
			// momentum conservation
			
			Vector3 momentum = Vector3.zero;
			
			foreach (Molecule m in reactionPrep.Molecules) {
				momentum += m.rigidbody.velocity * m.Species.Mass;
			}
			
			float productMassSum = 0;
			float productSizeSum = 0;
			MoleculeSpecies[] productSpecies = reactionPrep.ReactionType.GetProducts();
			
			foreach (MoleculeSpecies productS in productSpecies) {
				productMassSum += productS.Mass;
				productSizeSum += productS.Size;
			}
			
			Vector3 productVelocity = momentum / productMassSum;
			
			// create products
			
			foreach (MoleculeSpecies productS in productSpecies) {
				GameObject product = (GameObject)GameObject.Instantiate(productS.GetPrefabObject(), center, Quaternion.identity);
				product.rigidbody.velocity = productVelocity;
			}			
			
			// flash
			
			GameObject flash = (GameObject)GameObject.Instantiate(LightFlash.GetPrefabObject(), Vector3.Lerp(center, Camera.main.gameObject.transform.position, 0.5f), Quaternion.identity);
			
			float intensity = 2f * (float)System.Math.Sqrt(productSizeSum);
			flash.GetComponent<LightFlash>().FinalIntensity = intensity;
		}

		public void InitiateReaction(ReactionType reaction)
		{
			CUE cue = CUE.GetInstance ();

			MoleculeSpecies[] reagents = reaction.GetReagents();
			Molecule[] molecules; 
			
			if (cue.Molecules.FindMolecuelsForReaction(reagents, out molecules))
			{
				ReactionPrep reactionPrep = new ReactionPrep(reaction, molecules);
				
				if (molecules.Length == 1)
				{
					reactionPrep.Ready(molecules[0]); // necessary if only one reagent in reaction
				}
			}
			else
			{ Debug.LogError("not sufficent Molecules for "+reaction.ToString()); }
		}
	}
}