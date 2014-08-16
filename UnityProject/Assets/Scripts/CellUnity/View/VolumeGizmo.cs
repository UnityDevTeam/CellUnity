using UnityEngine;
using System.Collections;
using CellUnity;
using CellUnity.Utility;

namespace CellUnity.View
{
	public class VolumeGizmo : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		void OnDrawGizmos() {

			Gizmos.color = new Color (0, 0, 1, 0.5f);

			CUE cue = CUE.GetInstance();

			float r = Utils.GetSphereRadius (cue.Volume);

			Gizmos.DrawSphere(Vector3.zero, Utils.ScaleFromNm(r));

		}
	}
}