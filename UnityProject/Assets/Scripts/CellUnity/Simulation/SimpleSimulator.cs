using UnityEngine;
using System.Collections;
using CellUnity.Reaction;

namespace CellUnity.Simulation
{
	public class SimpleSimulator : ISimulator
	{
		public void Init(MoleculeSpecies[] species, ReactionType[] reactions)
		{
			this.reactions = reactions;
			
			lastReactionQuantity = new long[reactions.Length];
			for (int i = 0; i < reactions.Length; i++) {
				lastReactionQuantity[i] = 0;
			}
			
			time = 0;
		}
		
		private ReactionType[] reactions;
		private long[] lastReactionQuantity;
		private	double time;
		
		public SimulationStep Step(double stepDuration)
		{
			time += stepDuration;
			
			ReactionCount[] rc = new ReactionCount[reactions.Length];
			
			for (int i = 0; i < reactions.Length; i++) {
				ReactionType reaction = reactions[i];
			
				long q = (long)System.Math.Floor(reaction.Rate * time);
				long lastQ = lastReactionQuantity[i];
				
				long count = q - lastQ;
				
				lastReactionQuantity[i] = q;
				
				rc[i] = new ReactionCount(reaction, count); 
			}
			
			return new SimulationStep(rc);
		}
		
		public void Dispose() { }
	}
}