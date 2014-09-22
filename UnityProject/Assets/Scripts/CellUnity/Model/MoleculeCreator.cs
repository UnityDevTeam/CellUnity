using UnityEngine;
using UnityEditor;
using System;

namespace CellUnity.Model
{
	/// <summary>
	/// Class that helps to create a new molecule species out of
	/// an array of game objects.
	/// Set gameObjects, name, mass and Diffusion fields. Then call Create method to
	/// create the species.
	/// </summary>
	public class MoleculeCreator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Model.MoleculeCreator"/> class.
		/// </summary>
		public MoleculeCreator ()
		{
		}

		/// <summary>
		/// The game objects which represent the species
		/// </summary>
		public GameObject[] gameObjects;
		/// <summary>
		/// The name of the new species
		/// </summary>
		public string name = null;
		/// <summary>
		/// The mass of the species
		/// </summary>
		public float mass = 1f;
		/// <summary>
		/// The diffusion of the new species
		/// </summary>
		public float Diffusion = 0.05f;

		public MoleculeSpecies Create()
		{
			// find a name if not specified
			findName ();

			// create main object of the species
			GameObject mol = new GameObject(name);
			mol.transform.position = Vector3.zero;

			float colliderRadius = 0;

			//
			// Find center of all gameObjects
			//

			Vector3 center = Vector3.zero;

			foreach (var obj in gameObjects) {

				// set main object as parent
				obj.transform.parent = mol.transform;

				center += obj.transform.position;

				// remove collider for performance boost
				if (obj.collider != null) {
					obj.collider.enabled = false;
					CellUnity.Utility.ScriptManager.RemoveComponent(obj.collider);
				}
			}

			center = center / gameObjects.Length;

			//
			// locate object to (0,0,0) by setting center to (0,0,0)
			// and calculate the colliderRadius by making sure every atom is inside this radius
			//
			foreach (var obj in gameObjects) {
				
				obj.transform.position -= center;

				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.min.x));
				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.min.y));
				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.min.z));
				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.max.x));
				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.max.y));
				colliderRadius = Math.Max(colliderRadius, Math.Abs(obj.renderer.bounds.max.z));
			}

			// Create a new species and add it to the CUE
			CUE cue = CUE.GetInstance ();
			MoleculeSpecies species = cue.CreateMoleculeSpecies ();
			species.Name = name;
			cue.AddSpecies (species);

			// Add the molecule script to the main object
			CellUnity.Molecule script = mol.AddComponent<CellUnity.Molecule>();
			script.Species = species;
			species.Mass = mass;
			species.Size = colliderRadius * 2;

			// Add a sphere collider to the main object
			SphereCollider sphereCollider = mol.AddComponent<SphereCollider> ();
			Rigidbody rigidbody = mol.AddComponent<Rigidbody> ();
			rigidbody.useGravity = false;
			rigidbody.mass = mass;
			sphereCollider.radius = colliderRadius;

			// Add a diffusion script to the main object
			View.Diffusion diffusion = mol.AddComponent<View.Diffusion> ();
			diffusion.Intensity = Diffusion;

			// create the prefab used as template for the species
			string assetPath = "Assets/Molecules/" + name + ".prefab";
			UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(assetPath);
			PrefabUtility.ReplacePrefab(mol, prefab);
			AssetDatabase.Refresh();
			
			species.PrefabPath = assetPath;

			EditorUtility.SetDirty (cue);

			// Delete game objects
			foreach (var obj in gameObjects) {
				GameObject.DestroyImmediate(obj);
			}
			GameObject.DestroyImmediate (mol);

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