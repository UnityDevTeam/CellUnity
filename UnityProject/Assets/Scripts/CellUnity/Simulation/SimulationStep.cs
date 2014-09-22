using UnityEngine;
using System.Collections;

namespace CellUnity.Simulation
{
	/// <summary>
	/// Simulation step.
	/// Contains the simulation results of one simulated step
	/// </summary>
	public class SimulationStep
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Simulation.SimulationStep"/> class.
		/// </summary>
		/// <param name="reactions">Number of reactions occured of each type</param>
		public SimulationStep(ReactionCount[] reactions)
		{
			if (reactions == null) { reactions = new ReactionCount[]{}; }
		
			this.reactions = reactions;
		}
	
		private ReactionCount[] reactions;
		/// <summary>
		/// Array that contains the number of reactions occured of each type
		/// </summary>
		public ReactionCount[] Reactions { get { return reactions; } }
	}
}