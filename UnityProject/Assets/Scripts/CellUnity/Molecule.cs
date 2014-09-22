using UnityEngine;
using UnityEditor;
using System.IO;
using CellUnity.Reaction;

namespace CellUnity
{
	/// <summary>
	/// Molecule-Script that identifies a GameObject as molecule.
	/// A Molecule must be associated with a molecule Species defined to the CUE.
	/// </summary>
	[System.Serializable]
	public class Molecule : MonoBehaviour {

		/// <summary>
		/// Species of the molecuke
		/// </summary>
		public MoleculeSpecies Species;

		/// <summary>
		/// The default Drag of all molecules
		/// </summary>
		public static readonly float Drag = 1;

		void Start()
		{
			// assign default drag
			rigidbody.drag = Drag;
		}

		void OnEnable ()
		{
			CUE cue = CUE.GetInstance ();

			// Register molecule to MoleculeManager
			cue.Molecules.Add (this);
		}

		void OnDisable()
		{
			CUE cue = CUE.GetInstance ();

			// Remove molecule form MoleculeManager
			cue.Molecules.Remove (this);
		}

		void OnCollisionStay(Collision collision)
		{
			Molecule otherMolecule = collision.gameObject.GetComponent<Molecule> ();

			// Detect if collision is with molecule
			if (otherMolecule != null)
			{
				CUE cue = CUE.GetInstance ();

				// let the reaction manager handle the collision
				cue.ReactionManager.Collision(this, otherMolecule);
			}
		}
		
		void OnMouseDown() 
		{
			// select molecule
			CUE cue = CUE.GetInstance ();
			cue.ReactionManager.SelectedMolecule = this;
			cue.ScriptManager.GetOrAddScript<CellUnity.View.MoleculeSelectScript> ().enabled = true;
		}

		/// <summary>
		/// field of ReactionPrep
		/// </summary>
		[System.NonSerialized]
		private ReactionPrep reactionPrep = null;

		/// <summary>
		/// Reaction Preperation of this molecule. Is null by default. If the molecule is involved in a
		/// reaction a ReactionPrep instance is assigned.
		/// Can be set with methods AssignReactionPrep, ClearReactionPrep and ReleaseReactionPrep.
		/// </summary>
		/// <value>The reaction prep.</value>
		public ReactionPrep ReactionPrep { get { return reactionPrep; } }

		/// <summary>
		/// Assigns a reaction prep, that indicates that the molecule is involved in a reaction.
		/// The MoleculeManager is updated.
		/// </summary>
		/// <param name="reactionPrep">Reaction prep.</param>
		public void AssignReactionPrep(ReactionPrep reactionPrep)
		{
			this.reactionPrep = reactionPrep;

			CUE cue = CUE.GetInstance ();
			cue.Molecules.AssignReactionPrep (this);
		}

		/// <summary>
		/// Clears the reaction prep after the reaction was performed. The molecule is
		/// removed from the MoleculeManager. Molecule must be deleted after calling this method
		/// </summary>
		public void ClearReactionPrep()
		{
			if (this.reactionPrep != null)
			{
				CUE cue = CUE.GetInstance ();
				cue.Molecules.ClearReactionPrep (this);

				this.reactionPrep = null;
			}
		}

		/// <summary>
		/// Releases the reaction prep. Called when a planned reaction is not performed (e.g. when
		/// not enough molecules available or an reaction occurs). ReactionPrep is set to null again.
		/// </summary>
		public void ReleaseReactionPrep()
		{
			if (this.reactionPrep != null)
			{
				this.reactionPrep = null;
				
				CUE cue = CUE.GetInstance ();
				cue.Molecules.ReleaseReactionPrep (this);
			}
		}

		/// <summary>
		/// Gets or sets the position of the molecule's GameObject
		/// </summary>
		/// <value>The position.</value>
		public Vector3 Position
		{
			get { return gameObject.transform.position; }
			set { gameObject.transform.position = value; }
		}

		void FixedUpdate() {

			//
			// When in reaction, add attraction force to the location of the reaction
			//

			ReactionPrep r = ReactionPrep;

			if (r != null && r.Active) 
			{
				Vector3 destination = r.GetExpectedReactionLocation();

				Vector3 force = Vector3.Normalize(destination - Position);
			
				rigidbody.AddForce(force, ForceMode.Acceleration);
			
				//Debug.Log(force);
			}

			//
			// Collide with compartment wall
			//

			CUE cue = CUE.GetInstance ();

			cue.CheckCompartmentCollision (this);
		}

		/// <summary>
		/// Next molecule in the linked list of the molecule manager.
		/// Must only be modified by the MoleculeCollection.
		/// </summary>
		public Molecule CollectionNext = null;
		/// <summary>
		/// Previous molecule in the linked list of the molecule manager
		/// Must only be modified by the MoleculeCollection.
		/// </summary>
		public Molecule CollectionPrevious = null;
		/// <summary>
		/// The collection the molcule is assigned to in the molecule manager.
		/// Must only be modified by the MoleculeCollection.
		/// </summary>
		public MoleculeCollection Collection = null;
	}
}