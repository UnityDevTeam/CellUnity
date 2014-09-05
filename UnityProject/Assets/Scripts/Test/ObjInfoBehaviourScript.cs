using UnityEngine;
using System.Collections;

public class ObjInfoBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MeshFilter viewedModelFilter = (MeshFilter)gameObject.GetComponent("MeshFilter");
		var viewedModel = viewedModelFilter.mesh; 
		Debug.Log("Vertex count:" + viewedModel.vertexCount);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
