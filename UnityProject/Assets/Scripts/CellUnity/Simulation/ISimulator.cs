using UnityEngine;
using System.Collections;
using CellUnity.Reaction;

namespace CellUnity.Simulation
{
	public interface ISimulator : System.IDisposable
	{
		void Init(CUE cue);
		void Reload();
		SimulationStep Step(double stepDuration);
	}
}