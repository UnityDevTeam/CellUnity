using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Utility;

namespace CellUnity
{
	[System.Serializable]
	public class MoleculeSpecies : ScriptableObject {

		private MoleculeSpecies() { }

		void OnEnable ()
		{
			hideFlags = HideFlags.HideInHierarchy;
		}

		[SerializeField]
		public string Name = "Molecule";
		
		public float Size;
		public float Mass;

		[SerializeField]
		public string PrefabPath;

		private Object prefabObject = null;
		public Object GetPrefabObject()
		{
			if (prefabObject == null)
			{
				prefabObject = Resources.LoadAssetAtPath(PrefabPath, typeof(GameObject));
			}

			return prefabObject;
		}

		public void Delete()
		{
			if (PrefabPath != null) {
				AssetDatabase.DeleteAsset(PrefabPath);
			}
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