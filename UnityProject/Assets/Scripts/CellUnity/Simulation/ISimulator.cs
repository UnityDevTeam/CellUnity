using UnityEngine;
using System.Collections;
using CellUnity.Reaction;

namespace CellUnity.Simulation
{
	/// <summary>
	/// Simulator Interface
	/// </summary>
	public interface ISimulator : System.IDisposable
	{
		/// <summary>
		/// Init Simulator with CUE
		/// </summary>
		/// <param name="cue">CUE</param>
		void Init(CUE cue);

		/// <summary>
		/// Reload data from CUE
		/// </summary>
		void Reload();

		/// <summary>
		/// Simulate a Step and return data
		/// </summary>
		/// <param name="stepDuration">Step duration in seconds.</param>
		SimulationStep Step(double stepDuration);
	}
}