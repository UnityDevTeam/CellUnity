using UnityEngine;
using System.Collections;

/// <summary>
/// Script that sets the drag of a GameObject to an initialDrag.
/// After a specified amount of time the drag is set to another value (drag-Field)
/// and this script is removed from the GameObject.
/// </summary>
public class DelayedDrag : MonoBehaviour {
	
	void Start () {
		t = 0;
	}

	/// <summary>
	/// Contains the time passed in seconds.
	/// </summary>
	private float t;

	/// <summary>
	/// The drag that is set after the time defined in the delay-Field has passed.
	/// </summary>
	public float drag = 0;
	/// <summary>
	/// Delay in seconds after which the drag is set.
	/// </summary>
	public float delay = 3;
	/// <summary>
	/// The initial drag that is set as long as the delay has not passed.
	/// </summary>
	public float initialDrag = 0;

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate() {
		t += Time.fixedDeltaTime;

		if (t > delay)
		{
			rigidbody.drag = drag;
			Destroy(this);
		}
		else
		{
			rigidbody.drag = initialDrag;
		}
	}
}
