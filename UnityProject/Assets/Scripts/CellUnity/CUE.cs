using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Reaction;
using CellUnity.Simulation;
using CellUnity.Utility;

namespace CellUnity
{
	/// <summary>
	/// CellUnity Environment
	/// Holds all environmental properties and definitions. The entire system can
	/// only contain one instance of a CUE, therefore it is implemented as singleton.
	/// </summary>
	[System.Serializable]
	public class CUE : ScriptableObject {

		private CUE() {
			instance = this;
			Debug.Log ("CUE constructor");
		}

		void OnEnable ()
		{
			//hideFlags = HideFlags.HideInHierarchy;
			Debug.Log ("CUE enabled "+ToString());

			Volume = volume; // recalculate Radius

			if (species == null) {
				species = new List<MoleculeSpecies>();
			}

			if (reactionTypes == null) {
				reactionTypes = new List<ReactionType>();
			}
		}

		/// <summary>
		/// MoleculeManager. Manages all molecules in the scene
		/// </summary>
		/// <value>MoleculeManager</value>
		public MoleculeManager Molecules { get { return molecules; } }
		private MoleculeManager molecules = new MoleculeManager();

		/// <summary>
		/// ReactionManager. Responsible for initiating and performing reactions
		/// </summary>
		/// <value>ReactionManager</value>
		public ReactionManager ReactionManager { get { return reactionManager; } }
		private ReactionManager reactionManager = new ReactionManager();

		/// <summary>
		/// SimulationManager. Responsible for simulator communication
		/// </summary>
		/// <value>SimulationManager</value>
		public SimulationManager SimulationManager { get { return simulationManager; } }
		private SimulationManager simulationManager = new SimulationManager();

		/// <summary>
		/// ScriptManager. Responsible for running Unity scripts
		/// </summary>
		/// <value>ScriptManager</value>
		public ScriptManager ScriptManager { get { return scriptManager; } }
		private ScriptManager scriptManager = new ScriptManager();

		/// <summary>
		/// List of molecule species. Must be serialized.
		/// </summary>
		[SerializeField]
		private List<MoleculeSpecies> species;

		/// <summary>
		/// Time in seconds that is simulated every step
		/// </summary>
		public float SimulationStep = 1;
		/// <summary>
		/// Duration in seconds of a simulation step in real time (in the visualization)
		/// </summary>
		public float VisualizationStep = 1;

		/// <summary>
		/// field of Volume Property
		/// </summary>
		[SerializeField]
		private float volume = 1e-14f;

		/// <summary>
		/// field of UnityRadius Property
		/// </summary>
		private float unityRadius;

		/// <summary>
		/// Radius of the compartment in unit used by Unity for 3D space.
		/// </summary>
		/// <value>The unity radius.</value>
		public float UnityRadius { get { return unityRadius; } }

		/// <summary>
		/// Gets or sets the volume of the compartment in nano liter (nl).
		/// Automatically refreshes UnityRadius when set.
		/// </summary>
		/// <value>Volume in nano liter</value>
		public float Volume
		{
			get { return volume; }
			set
			{
				// recalculate and scale radius
				this.unityRadius = Utils.ScaleFromNm(Utils.GetSphereRadius(value));

				if (value != this.volume)
				{
					this.volume = value;

					// resize compartment object
					UpdateCompartment();
				}
			}
		}

		/// <summary>
		/// field for identification. Just for debug purposes. can be removed.
		/// </summary>
		[SerializeField]
		public int ID = System.DateTime.Now.Millisecond;

		/// <summary>
		/// Gets all the species currently defined to the environment.
		/// </summary>
		/// <value>The species.</value>
		public MoleculeSpecies[] Species {
			get {
				return species.ToArray();
			}
		}

		/// <summary>
		/// Adds a new species to the environment.
		/// </summary>
		/// <param name="s">species to add</param>
		public void AddSpecies(MoleculeSpecies s) {
			species.Add (s);

			UnityEditor.AssetDatabase.AddObjectToAsset (s, this);
		}

		/// <summary>
		/// Creates a new molecule species instance. The instance is not added to
		/// the environment.
		/// </summary>
		/// <returns>new molecule species instance</returns>
		public MoleculeSpecies CreateMoleculeSpecies() {
			return ScriptableObject.CreateInstance<MoleculeSpecies> ();
		}

		/// <summary>
		/// Removes a species from the environment. All molecule instances associated
		/// with this species are removed. The species is destroyed immediatly
		/// (ScriptableObject.DestroyImmediate).
		/// </summary>
		/// <param name="s">species to remove</param>
		public void RemoveSpecies(MoleculeSpecies s) {
			s.Delete ();
			species.Remove (s);
			ScriptableObject.DestroyImmediate (s, true);
		}

		/// <summary>
		/// Removes all molecules in the environment.
		/// (Removes all GameObjects with the <see cref="Molecule"/>-Script applied)
		/// </summary>
		public void RemoveMolecules ()
		{
			RemoveMolecules(null, true);
		}

		/// <summary>
		/// Removes all molecules of a specific species.
		/// (All GameObjects with the <see cref="Molecule"/>-Script applied,
		/// associated with the species).
		/// </summary>
		/// <param name="moleculeSpecies">Species to remove</param>
		public void RemoveMolecules (MoleculeSpecies moleculeSpecies)
		{
			RemoveMolecules(moleculeSpecies, false);
		}

		/// <summary>
		/// Internal Method to removes all molecules of a specific species or all
		/// molecules.
		/// </summary>
		/// <param name="moleculeSpecies">species to remove.</param>
		/// <param name="all">If set to <c>true</c> all molecules (species does not matter) are removed.</param>
		private void RemoveMolecules (MoleculeSpecies moleculeSpecies, bool all)
		{
			Molecule[] molecules = GameObject.FindObjectsOfType<Molecule>();
			
			foreach (var m in molecules) {
				if (all || moleculeSpecies.Equals(m.Species))
				{
					GameObject.DestroyImmediate(m.gameObject);
				}
			}
		}

		/// <summary>
		/// field of ReactionTypes
		/// </summary>
		[SerializeField]
		private List<ReactionType> reactionTypes;

		/// <summary>
		/// Gets an array with all <see cref="ReactionType"/> defined to the environment
		/// </summary>
		/// <value>The reaction types.</value>
		public ReactionType[] ReactionTypes {
			get {
				return reactionTypes.ToArray();
			}
		}

		/// <summary>
		/// Creates a new reaction type instance. The instance is not added to
		/// the environment.
		/// </summary>
		/// <returns>The reaction type.</returns>
		public ReactionType CreateReactionType() {
			return ScriptableObject.CreateInstance<ReactionType> ();
		}

		/// <summary>
		/// Adds a reaction type to the environment.
		/// </summary>
		/// <param name="r">The red component.</param>
		public void AddReaction(ReactionType r) {
			reactionTypes.Add (r);

			UnityEditor.AssetDatabase.AddObjectToAsset (r, this);
		}

		/// <summary>
		/// Removes a reaction type from the environment. The reaction type
		/// is destroyed immediatly (ScriptableObject.DestroyImmediate).
		/// </summary>
		/// <param name="r">The red component.</param>
		public void RemoveReaction(ReactionType r) {
			reactionTypes.Remove (r);

			ScriptableObject.DestroyImmediate (r, true);
		}

		/// <summary>
		/// Holds the Instance of CUE
		/// </summary>
		private static CUE instance;

		/// <summary>
		/// Gets the environment instance. If not available, it is loaded from
		/// "Assets/Resources/CUE.asset". If the asset not exists, it is created.
		/// </summary>
		/// <returns>The instance.</returns>
		public static CUE GetInstance() {
			if (instance == null) {
				Debug.Log ("try load CUE...");

				Object asset = Resources.Load("CUE");
				Debug.Log("loaded: "+(asset == null ? "null" : asset.ToString()));
				CUE loadedCue = asset as CUE;
				if (loadedCue == null)
				{
					Debug.Log ("creating new CUE...");
					CUE cue = ScriptableObject.CreateInstance<CUE>();
					UnityEditor.AssetDatabase.CreateAsset(cue, "Assets/Resources/CUE.asset");
					//UnityEditor.AssetDatabase.AddObjectToAsset(cue, "Assets/Resources/CUE.asset");
					UnityEditor.AssetDatabase.SaveAssets();
				}
			}

			return instance;
		}

		public override string ToString ()
		{
			return "CUE-"+GetInstanceID().ToString()+" / ms:"+ID.ToString();
		}

		/// <summary>
		/// Resizes the Compartment GameObject according to the set volume.
		/// The GameObject must be named "Compartment". If no such GameObject
		/// exists, nothing is performed.
		/// </summary>
		private void UpdateCompartment()
		{
			GameObject obj = GameObject.Find ("Compartment");
			if (obj != null)
			{
				float s = UnityRadius * 2;
				obj.transform.position = Vector3.zero;
				obj.transform.localScale = new Vector3 (s, s, s);

				obj.collider.enabled = false;
			}
		}
	}
}