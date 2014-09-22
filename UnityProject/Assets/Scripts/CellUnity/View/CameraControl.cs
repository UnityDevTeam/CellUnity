using UnityEngine;
using System.Collections;

namespace CellUnity.View
{
	/// <summary>
	/// Script that allows to control the camera in game mode.
	/// </summary>
	public class CameraControl : MonoBehaviour {
	
		// Use this for initialization
		void Start () {
			root = new GameObject ("CameraRoot");
			root.transform.position = Vector3.zero;

			transform.parent = root.transform;
		}

		/// <summary>
		/// The horizontal speed of the mouse cursor
		/// </summary>
		public float horizontalSpeed = 2.0F;
		/// <summary>
		/// The vertical speed of the mouse cursor
		/// </summary>
		public float verticalSpeed = 2.0F;

		/// <summary>
		/// The move speed when W, A, S or D keys are pressed
		/// </summary>
		public float moveSpeed = 5.0F;

		/// <summary>
		/// The move speed when shift key is pressed
		/// </summary>
		public float moveFastSpeed = 10.0F;

		/// <summary>
		/// GameObject which the camera should follow
		/// </summary>
		public static GameObject Follow = null;

		private GameObject lastFollow = null;

		/// <summary>
		/// Empty object which is transform parent of the camera.
		/// This allows the camera position and rotation can be set
		/// independently.
		/// </summary>
		private GameObject root;
		private bool following = false;

		
		void Update() {

			if ((!following && Follow != null) || (following && lastFollow != Follow))
			{
				// start following

				following = true;

				Vector3 p = transform.position;
				root.transform.position = Follow.transform.position;
				transform.position = p;

				lastFollow = Follow;
			}
			else if (following && Follow == null)
			{
				// stop following

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