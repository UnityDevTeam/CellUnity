using UnityEngine;
using System.Collections;

public class LightFlash : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.light.intensity = 0;
		gameObject.light.enabled = true;
		mode = 0;
		Debug.Log ("Explosion");
	}
	
	public float FadeInTime = 0.1f;
	public float FinalIntensity = 7;
	public float FadeOutTime = 0.5f;
	private int mode = 0;
	
	// Update is called once per frame
	void Update () {
		if (mode == 0)
		{
			gameObject.light.intensity += Time.deltaTime * FinalIntensity / FadeInTime;
			if (gameObject.light.intensity >= FinalIntensity) {
				mode = 1;
				gameObject.light.intensity = FinalIntensity;
			}
		} else if (mode == 1) {
			gameObject.light.intensity -= Time.deltaTime * FinalIntensity / FadeOutTime;
			if (gameObject.light.intensity <= 0) {
				mode = 2;
				gameObject.light.intensity = 0;
			}
		}
	}
}
