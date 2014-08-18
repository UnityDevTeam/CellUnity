using UnityEngine;
using System.Collections;

namespace CellUnity.View
{
	public class CameraControl : MonoBehaviour {
	
		// Use this for initialization
		void Start () {
			/*GameObject lightGameObject = new GameObject("CamLight");

			Light light = lightGameObject.AddComponent<Light>();
			lightGameObject.transform.position = Camera.main.transform.position;
			lightGameObject.transform.parent = Camera.main.transform;

			light.intensity = 2f;
			light.range = 20f;*/

			root = new GameObject ("CameraRoot");
			root.transform.position = Vector3.zero;

			transform.parent = root.transform;
		}
		
		public float horizontalSpeed = 2.0F;
		public float verticalSpeed = 2.0F;
		
		public float moveSpeed = 5.0F;
		public float moveFastSpeed = 10.0F;

		public static GameObject Follow = null;

		private GameObject lastFollow = null;
		private GameObject root;
		private bool following = false;

		
		void Update() {

			if ((!following && Follow != null) || (following && lastFollow != Follow))
			{
				following = true;

				Vector3 p = transform.position;
				root.transform.position = Follow.transform.position;
				transform.position = p;

				lastFollow = Follow;
			}
			else if (following && Follow == null)
			{
				following = false;
				lastFollow = null;
			}

			if (following)
			{
				root.transform.position = Follow.transform.position;
			}


			Transform t = this.transform;

			if (Input.GetKey("mouse 1"))
			{
				float h = horizontalSpeed * Input.GetAxis("Mouse X");
				float v = - verticalSpeed * Input.GetAxis("Mouse Y");
				t.Rotate(v, h, 0);
			}
			
			float m = (Input.GetKey("left shift") ? moveFastSpeed : moveSpeed) * Time.deltaTime;
			
			if (Input.GetKey("w"))
			{ t.Translate(0, 0, m); }
			
			if (Input.GetKey("s"))
			{ t.Translate(0, 0, -m); }
			
			if (Input.GetKey("a"))
			{ t.Translate(-m, 0, 0); }
			
			if (Input.GetKey("d"))
			{ t.Translate(m, 0, 0); }
		}
	}
}