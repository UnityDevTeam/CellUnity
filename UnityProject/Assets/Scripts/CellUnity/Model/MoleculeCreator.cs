using UnityEngine;
using UnityEditor;

namespace CellUnity.Model
{
	public class MoleculeCreator
	{
		public MoleculeCreator ()
		{
		}

		public GameObject[] gameObjects;
		public string name = null;

		public MoleculeSpecies Create()
		{
			findName ();

			GameObject mol = new GameObject(name);
			mol.transform.position = Vector3.zero;

			Vector3 center = Vector3.zero;

			foreach (var obj in gameObjects) {

				obj.transform.parent = mol.transform;

				center += obj.transform.position;

				if (obj.collider != null) {
					obj.collider.enabled = false;
				}
			}

			center = center / gameObjects.Length;

			foreach (var obj in gameObjects) {
				
				obj.transform.position -= center;
			}
			
			CUE cue = CUE.GetInstance ();
			MoleculeSpecies species = cue.CreateMoleculeSpecies ();
			species.Name = name;
			cue.AddSpecies (species);

			mol.AddComponent<CellUnity.Molecule>();
			CellUnity.Molecule script = mol.GetComponent<CellUnity.Molecule> ();
			script.Species = species;

			mol.AddComponent<SphereCollider> ();
			Rigidbody rigidbody = mol.AddComponent<Rigidbody> ();
			rigidbody.useGravity = false;

			string assetPath = "Assets/Molecules/" + name + ".prefab";
			Object prefab = PrefabUtility.CreateEmptyPrefab(assetPath);
			PrefabUtility.ReplacePrefab(mol, prefab);
			AssetDatabase.Refresh();
			
			species.PrefabPath = assetPath;

			EditorUtility.SetDirty (cue);

			/*foreach (var obj in gameObjects) {
				GameObject.DestroyImmediate(obj);
			}
			GameObject.DestroyImmediate (mol);*/

			return species;
		}

		private void findName()
		{
			if (name == null) {

				if (gameObjects!= null && gameObjects.Length>0) {
					name = gameObjects[0].name;
				}
				else
				{
					name = "unknown";
				}

			}
		}
	}
}