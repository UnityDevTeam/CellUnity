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

		private MoleculeManager molecules = new MoleculeManager();
		public MoleculeManager Molecules { get { return molecules; } }

		private ReactionManager reactionManager = new ReactionManager();
		public ReactionManager ReactionManager { get { return reactionManager; } }
		
		private SimulationManager simulationManager = new SimulationManager();
		public SimulationManager SimulationManager { get { return simulationManager; } }

		private ScriptManager scriptManager = new ScriptManager();
		public ScriptManager ScriptManager { get { return scriptManager; } }

		[SerializeField]
		private List<MoleculeSpecies> species;
		
		public float SimulationStep = 1;
		public float VisualizationStep = 1;

		[SerializeField]
		private float volume = 1e-16f;
		private float unityRadius;

		public float UnityRadius { get { return unityRadius; } }

		public float Volume
		{
			get { return volume; }
			set
			{
				this.unityRadius = Utils.ScaleFromNm(Utils.GetSphereRadius(value));

				if (value != this.volume)
				{
					this.volume = value;
					MakeCompartment();
				}
			}
		}


		[SerializeField]
		public int ID = System.DateTime.Now.Millisecond;

		public MoleculeSpecies[] Species {
			get {
				return species.ToArray();
			}
		}

		public void AddSpecies(MoleculeSpecies s) {
			species.Add (s);

			UnityEditor.AssetDatabase.AddObjectToAsset (s, this);
		}

		public MoleculeSpecies CreateMoleculeSpecies() {
			return ScriptableObject.CreateInstance<MoleculeSpecies> ();
		}

		public void RemoveSpecies(MoleculeSpecies s) {
			s.Delete ();
			species.Remove (s);
			ScriptableObject.DestroyImmediate (s, true);
		}

		public void RemoveMolecules ()
		{
			RemoveMolecules(null, true);
		}

		public void RemoveMolecules (MoleculeSpecies moleculeSpecies)
		{
			RemoveMolecules(moleculeSpecies, false);
		}

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

		[SerializeField]
		private List<ReactionType> reactionTypes;

		public ReactionType[] ReactionTypes {
			get {
				return reactionTypes.ToArray();
			}
		}

		public ReactionType CreateReactionType() {
			return ScriptableObject.CreateInstance<ReactionType> ();
		}

		public void AddReaction(ReactionType r) {
			reactionTypes.Add (r);

			UnityEditor.AssetDatabase.AddObjectToAsset (r, this);
		}
		
		public void RemoveReaction(ReactionType r) {
			reactionTypes.Remove (r);

			ScriptableObject.DestroyImmediate (r, true);
		}

		private static CUE instance;
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
			return GetInstanceID().ToString()+" / ms:"+ID.ToString();
		}

		public void ResetData() {
			reactionTypes.Clear ();
			species.Clear ();
		}

		public void MakeCompartment()
		{
			//GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			//obj.name = "Compartment";
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