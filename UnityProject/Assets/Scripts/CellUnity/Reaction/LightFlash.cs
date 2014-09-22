using UnityEngine;
using System.Collections;

namespace CellUnity.Reaction
{
	/// <summary>
	/// Light flash.
	/// Makes a light brighter until a defined FinalIntensity and then
	/// lets it fade out.
	/// </summary>
	public class LightFlash : MonoBehaviour {
	
		// Use this for initialization
		void Start () {
			gameObject.light.intensity = 0;
			gameObject.light.enabled = true;
			mode = 0;
		}

		/// <summary>
		/// Duration till full brightness in seconds
		/// </summary>
		public float FadeInTime = 0.1f;
		/// <summary>
		/// The final intensity of the light
		/// </summary>
		public float FinalIntensity = 7;
		/// <summary>
		/// Duration of fade out in seconds
		/// </summary>
		public float FadeOutTime = 0.5f;
		/// <summary>
		/// Current status of the flash.
		/// 0: fadeIn;
		/// 1: fadeOut;
		/// 2: light is dark -> destroy;
		/// 3: object destroyed
		/// </summary>
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
			} else if (mode == 2)
			{
				mode = 3;
				GameObject.Destroy(gameObject);
			}
		}
		
		private static GameObject prefabObject = null;

		/// <summary>
		/// Returns the FusionFlash prefab.
		/// </summary>
		/// <returns>The prefab object.</returns>
		public static GameObject GetPrefabObject()
		{
			if (prefabObject == null)
			{
				prefabObject = Resources.LoadAssetAtPath<GameObject>("Assets/FusionFlash.prefab");
			}
			
			return prefabObject;
		}
	}
}
