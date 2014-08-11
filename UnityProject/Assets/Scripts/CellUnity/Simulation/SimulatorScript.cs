using UnityEngine;
using System.Collections;

namespace CellUnity.Simulation
{
public class SimulatorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.cue = CUE.GetInstance();
	}
	
	private CUE cue;
	
	// Update is called once per frame
	void FixedUpdate() {
		cue.SimulationManager.Update();
	}
}
}