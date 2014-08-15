﻿using UnityEngine;
using System.Collections;

namespace CellUnity.View
{
	public class CameraControl : MonoBehaviour {
	
		// Use this for initialization
		void Start () {
			GameObject lightGameObject = new GameObject("CamLight");
			Light light = lightGameObject.AddComponent<Light>();
			lightGameObject.transform.position = Camera.main.transform.position;
			lightGameObject.transform.parent = Camera.main.transform;

			light.intensity = 2f;
			light.range = 20f;
		}
		
		private float horizontalSpeed = 2.0F;
		private float verticalSpeed = 2.0F;
		
		private float moveSpeed = 5.0F;
		private float moveFastSpeed = 10.0F;
		
		void Update() {
			if (Input.GetKey("mouse 1"))
			{
				float h = horizontalSpeed * Input.GetAxis("Mouse X");
				float v = - verticalSpeed * Input.GetAxis("Mouse Y");
				transform.Rotate(v, h, 0);
			}
			
			float m = (Input.GetKey("left shift") ? moveFastSpeed : moveSpeed) * Time.deltaTime;
			
			if (Input.GetKey("w"))
			{ transform.Translate(0, 0, m); }
			
			if (Input.GetKey("s"))
			{ transform.Translate(0, 0, -m); }
			
			if (Input.GetKey("a"))
			{ transform.Translate(-m, 0, 0); }
			
			if (Input.GetKey("d"))
			{ transform.Translate(m, 0, 0); }
		}
	}
}