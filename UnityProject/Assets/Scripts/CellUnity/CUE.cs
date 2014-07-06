using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Reaction;

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

			if (species == null) {
				species = new List<MoleculeSpecies>();
			}

			if (reactionTypes == null) {
				reactionTypes = new List<ReactionType>();
			}
		}

		//[SerializeField]
		private MoleculeCollection molecules = new MoleculeCollection();
		public MoleculeCollection Molecules { get { return molecules; } }

		//[SerializeField]
		private ReactionManager reactionManager = new ReactionManager();
		public ReactionManager ReactionManager { get { return reactionManager; } }

		[SerializeField]
		private List<MoleculeSpecies> species;

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
	}
}