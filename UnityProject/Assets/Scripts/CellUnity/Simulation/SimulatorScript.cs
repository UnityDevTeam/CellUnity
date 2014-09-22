using UnityEngine;
using System.Collections;

namespace CellUnity.Simulation
{
	/// <summary>
	/// Script that calls the cue.SimulationManager.Update method regularly.
	/// </summary>
	public class SimulatorScript : MonoBehaviour {

		// Use this for initialization
		void Start () {
			this.cue = CUE.GetInstance();
		}
		
		void OnDisable()
		{
			// Stop simulation when disabled, because results would not be
			// processed then anyway.
			cue.SimulationManager.Stop();
		}
		
		private CUE cue;
		
		void FixedUpdate() {
			cue.SimulationManager.Update();
		}
	}
}