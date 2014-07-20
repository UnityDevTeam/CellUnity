using UnityEngine;
using UnityEditor;
using System;

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

			float colliderRadius = 0;

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

				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.min.x));
				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.min.y));
				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.min.z));
				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.max.x));
				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.max.y));
				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.max.z));
			}
			
			CUE cue = CUE.GetInstance ();
			MoleculeSpecies species = cue.CreateMoleculeSpecies ();
			species.Name = name;
			cue.AddSpecies (species);

			mol.AddComponent<CellUnity.Molecule>();
			CellUnity.Molecule script = mol.GetComponent<CellUnity.Molecule> ();
			script.Species = species;

			SphereCollider sphereCollider = mol.AddComponent<SphereCollider> ();
			Rigidbody rigidbody = mol.AddComponent<Rigidbody> ();
			rigidbody.useGravity = false;
			sphereCollider.radius = colliderRadius;

			string assetPath = "Assets/Molecules/" + name + ".prefab";
			UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(assetPath);
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