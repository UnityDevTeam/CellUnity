using UnityEngine;
using System.Collections;
using CellUnity;

public class VolumeGizmo : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos() {
		
		CUE cue = CUE.GetInstance();

		Gizmos.color = new Color (0, 0, 1, 0.5f);

		float V = cue.Volume; // nanoliter
		float r = Mathf.Pow( 3*V / (4*Mathf.PI) , 1f/3f );
		Gizmos.DrawSphere(Vector3.zero, cue.ScaleNm(r));

	}
}
