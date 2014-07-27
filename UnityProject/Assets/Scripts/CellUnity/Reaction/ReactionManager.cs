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
			//Debug.Log ("Collision: " + m1.ToString () + "; " + m2.ToString ());

			if ((m1.ReactionPrep != null) && (m1.ReactionPrep == m2.ReactionPrep))
			{
				Debug.Log ("Reaction "+m1.Species+" + "+m2.Species);

				GameObject.Destroy(m1.gameObject);
				GameObject.Destroy(m2.gameObject);

				ReactionPrep reactionPrep = m1.ReactionPrep;

				m1.ReactionPrep = null;
				m2.ReactionPrep = null;

				Vector3 center = (m1.transform.position * m1.Species.Size + m2.transform.position * m2.Species.Size) / (m1.Species.Size + m2.Species.Size);

				MoleculeSpecies productSpecies = reactionPrep.ReactionType.Product;
				GameObject product = (GameObject)GameObject.Instantiate(productSpecies.GetPrefabObject(), center, Quaternion.identity);
				GameObject flash = (GameObject)GameObject.Instantiate(LightFlash.GetPrefabObject(), Vector3.Lerp(center, Camera.main.gameObject.transform.position, 0.5f), Quaternion.identity);
				
				float intensity = 2f * (float)System.Math.Sqrt(productSpecies.Size);
				flash.GetComponent<LightFlash>().FinalIntensity = intensity;
				
				// momentum conservation
				
				Vector3 productVelocity = (m1.rigidbody.velocity * m1.Species.Mass + m2.rigidbody.velocity * m2.Species.Mass) / productSpecies.Mass;
				product.rigidbody.velocity = productVelocity;
				
				//flash.transform.parent = product.transform;
			}
		}

		//private List<ReactionPrep> activeReactions = new List<ReactionPrep> ();

		public void PerformReaction(ReactionType reaction)
		{
			CUE cue = CUE.GetInstance ();

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

			Debug.Log ("Reaction: "+a.name+" + "+b.name);
		
			//activeReactions.Add (reactionPrep);
		}
	}
}