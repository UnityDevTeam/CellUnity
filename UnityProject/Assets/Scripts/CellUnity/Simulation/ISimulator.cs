using UnityEngine;
using System.Collections;
using CellUnity.Reaction;

namespace CellUnity.Simulation
{
	public interface ISimulator : System.IDisposable
	{
		void Init(UpdateQueue updateQueue);
		SimulationStep Step(double stepDuration);
	}
}