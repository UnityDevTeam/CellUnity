using UnityEngine;
using System.Collections;

public class DelayedDrag : MonoBehaviour {

	// Use this for initialization
	void Start () {
		t = 0;
	}

	private float t;

	public float drag = 5;
	public float delay = 3;

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate() {
		t += Time.fixedDeltaTime;

		if (t > delay)
		{
			rigidbody.drag = drag;
		}
	}
}
