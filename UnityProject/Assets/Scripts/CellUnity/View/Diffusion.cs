using UnityEngine;
using System.Collections;

namespace CellUnity.View
{
	/// <summary>
	/// Script that mimics diffusion (just for illustration, not a realistic
	/// diffusion)
	/// </summary>
	public class Diffusion : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		/// <summary>
		/// The intensity of the diffusion
		/// </summary>
		public float Intensity = 0.05f;

		private float GetRandomValue()
		{
			return Intensity * (Random.value - 0.5f);
		}

		void FixedUpdate()
		{
			// apply random force
			Vector3 force = new Vector3 (GetRandomValue (), GetRandomValue (), GetRandomValue ());
			rigidbody.AddForce(force, ForceMode.VelocityChange);
		}
	}
}