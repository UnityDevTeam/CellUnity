using UnityEngine;
using System.Collections;

namespace CellUnity.Simulation
{
public class SimulatorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.cue = CUE.GetInstance();
	}
	
	void OnDisable()
	{
		cue.SimulationManager.Stop();
	}
	
	private CUE cue;
	
	void FixedUpdate() {
		cue.SimulationManager.Update();
	}
}
}