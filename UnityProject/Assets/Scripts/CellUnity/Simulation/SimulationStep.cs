using UnityEngine;
using System.Collections;

namespace CellUnity.Simulation
{
	public class SimulationStep
	{
		public SimulationStep(ReactionCount[] reactions)
		{
			if (reactions == null) { reactions = new ReactionCount[]{}; }
		
			this.reactions = reactions;
		}
	
		private ReactionCount[] reactions;
		public ReactionCount[] Reactions { get { return reactions; } }
	}
}