using UnityEngine;
using UnityEditor;
using System.IO;
using CellUnity.Reaction;

namespace CellUnity
{
	[System.Serializable]
	public class Molecule : MonoBehaviour {

		public MoleculeSpecies Species;

		public static readonly float Drag = 1;

		void Start()
		{
			rigidbody.drag = Drag;
		}

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
		
		void OnMouseDown() {
			Camera.main.transform.parent = gameObject.transform;
		}

		[System.NonSerialized]
		private ReactionPrep reactionPrep = null;

		public ReactionPrep ReactionPrep { get { return reactionPrep; } }

		public void AssignReactionPrep(ReactionPrep reactionPrep)
		{
			this.reactionPrep = reactionPrep;

			CUE cue = CUE.GetInstance ();
			cue.Molecules.AssignReactionPrep (this);
		}

		// must be removed afterwards
		public void ClearReactionPrep()
		{
			if (this.reactionPrep != null)
			{
				this.reactionPrep = null;

				CUE cue = CUE.GetInstance ();
				cue.Molecules.ClearReactionPrep (this);
			}
		}

		public void ReleaseReactionPrep()
		{
			if (this.reactionPrep != null)
			{
				this.reactionPrep = null;
				
				CUE cue = CUE.GetInstance ();
				cue.Molecules.ReleaseReactionPrep (this);
			}
		}

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

		public Molecule CollectionNext = null;
		public Molecule CollectionPrevious = null;
		public MoleculeCollection Collection = null;
	}
}