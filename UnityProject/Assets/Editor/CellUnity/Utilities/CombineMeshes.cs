using UnityEngine;
using System;
using System.Collections.Generic;

public static class CombineMeshes
{
	public static GameObject Combine(GameObject[] gameObjects, string name)
	{
		
		GameObject result = new GameObject ();
	
		List<CombineInstance> combine = new List<CombineInstance> ();
		
		// Find center
		
		Vector3 center = Vector3.zero;
		
		foreach (var item in gameObjects) {
			Vector3 position = item.transform.position;
			center += position;
		}
		
		center = center / gameObjects.Length;
		
		// Reposition to origin and Collect Meshes
		
		result.transform.position = Vector3.zero; // result to origin
		
		MeshFilter meshFilter = result.AddComponent<MeshFilter> ();
		MeshRenderer meshRenderer = result.AddComponent<MeshRenderer> ();
		
		foreach (var item in gameObjects) {
			item.transform.position -= center;
			
			meshRenderer.sharedMaterial = item.renderer.sharedMaterial;
			
			// Collect mesh
			MeshFilter itemMeshFilter = item.GetComponent<MeshFilter>();
			CombineInstance combineInstance = new CombineInstance();
			
			combineInstance.mesh = itemMeshFilter.sharedMesh;
			combineInstance.transform = itemMeshFilter.transform.localToWorldMatrix;
			
			combine.Add(combineInstance);
		}
		
		// Combine
		
		meshFilter.sharedMesh = new Mesh ();
		meshFilter.sharedMesh.CombineMeshes(combine.ToArray());
		
		foreach (var item in gameObjects) {
			//GameObject.DestroyImmediate(item);
		}
		
		
		//result.AddComponent<MeshCollider> ();

		result.name = name;
		
		//result.transform.position = center;

		return result;
	}
}

