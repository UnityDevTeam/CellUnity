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
	}
}