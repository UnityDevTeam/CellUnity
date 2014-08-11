using UnityEngine;
using UnityEditor;
using System.IO;
using CellUnity.Reaction;

namespace CellUnity
{
	[System.Serializable]
	public class Molecule : MonoBehaviour {

		public MoleculeSpecies Species;

		void OnEnable ()
		{
			CUE cue = CUE.GetInstance ();
			cue.Molecules.Add (this);
		}

		void OnDisable()
		{
			CUE cue = CUE.GetInstance ();
			cue.Molecules.Remove (this);
		}

		void OnCollisionStay(Collision collision)
		{
			Molecule otherMolecule = collision.gameObject.GetComponent<Molecule> ();
			if (otherMolecule != null)
			{
				CUE cue = CUE.GetInstance ();
				cue.ReactionManager.Collision(this, otherMolecule);
			}
		}

		[System.NonSerialized]
		public ReactionPrep ReactionPrep;

		public Vector3 Position
		{
			get { return gameObject.transform.position; }
			set { gameObject.transform.position = value; }
		}

		void FixedUpdate() {
			ReactionPrep r = ReactionPrep;

			if (r != null) 
			{
				Vector3 destination = r.GetExpectedReactionLocation();

				Vector3 force = Vector3.Normalize(destination - Position);
			
				rigidbody.AddForce(force);
			
				//Debug.Log(force);
			}
		}
	}
}