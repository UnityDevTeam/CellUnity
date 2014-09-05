using UnityEngine;
using System.Collections;

namespace CellUnity.View
{
	public class Diffusion : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public float Intensity = 0.05f;

		private float GetRandomValue()
		{
			return Intensity * (Random.value - 0.5f);
		}

		void FixedUpdate()
		{
			Vector3 force = new Vector3 (GetRandomValue (), GetRandomValue (), GetRandomValue ());
			rigidbody.AddForce(force, ForceMode.VelocityChange);
		}
	}
}