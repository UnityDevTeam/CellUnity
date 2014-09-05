using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Utility;
using CellUnity;

namespace CellUnity
{
	/// <summary>
	/// Molecule species.
	/// </summary>
	[System.Serializable]
	public class MoleculeSpecies : ScriptableObject {

		private MoleculeSpecies() { }

		void OnEnable ()
		{
			hideFlags = HideFlags.HideInHierarchy;
		}

		/// <summary>
		/// Name of the Speices
		/// </summary>
		[SerializeField]
		public string Name = "Molecule";

		/// <summary>
		/// The diameter of the molecule in Unity units.
		/// </summary>
		public float Size;
		/// <summary>
		/// Mass of the molecule in [u]
		/// </summary>
		public float Mass;
		/// <summary>
		/// Initial Quantitiy
		/// </summary>
		public int InitialQuantity;

		/// <summary>
		/// Path to the prefab file which is used as template.
		/// Usually "Assets/Molecules/SPECIESNAME.prefab"
		/// </summary>
		[SerializeField]
		public string PrefabPath;

		/// <summary>
		/// The prefab object.
		/// </summary>
		private Object prefabObject = null;
		/// <summary>
		/// Gets the prefab object. If not already loaded, it is loaded from the PrefabPath
		/// </summary>
		/// <returns>The prefab object.</returns>
		public Object GetPrefabObject()
		{
			if (prefabObject == null)
			{
				prefabObject = Resources.LoadAssetAtPath(PrefabPath, typeof(GameObject));
			}

			return prefabObject;
		}

		/// <summary>
		/// Delete this species.
		/// All molecules associated with this species are removed. The prefab is also deleted.
		/// </summary>
		public void Delete()
		{
			CUE cue = CUE.GetInstance();
			cue.RemoveMolecules(this);
		
			if (PrefabPath != null) {
				AssetDatabase.DeleteAsset(PrefabPath);
			}
		}

		/// <summary>
		/// Creates a new molecule of this species.
		/// </summary>
		/// <returns>The created molecule</returns>
		/// <param name="location">initial location</param>
		public Molecule CreateMolecule(Vector3 location)
		{
			GameObject gameObject = (GameObject)GameObject.Instantiate(GetPrefabObject(), location, Quaternion.identity);
			return gameObject.GetComponent<Molecule>();
		}

		public override string ToString ()
		{
			return Name;
		}
		
		public override int GetHashCode ()
		{
			return GetInstanceID();
		}
		
		public override bool Equals (object o)
		{
			MoleculeSpecies other;
			if (Utils.TypeEquals<MoleculeSpecies> (this, o, out other)) {
				
				return 
					(GetInstanceID() == other.GetInstanceID()) &&
					(Name == other.Name);
				
			} else { return false; }
		}
	}
}