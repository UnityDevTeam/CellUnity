using UnityEngine;
using System.Collections;
using CellUnity.Reaction;
using CellUnity.Simulation.Update;

namespace CellUnity.Simulation
{
	public class SimpleSimulator : ISimulator
	{
		private UpdateQueue updateQueue;

		public void Init(UpdateQueue updateQueue)
		{
			this.updateQueue = updateQueue;

			ReactionType[] reactions = new ReactionType[]{}; //cue.ReactionTypes;
		
			this.reactions = reactions;
			
			lastReactionQuantity = new ulong[reactions.Length];
			for (int i = 0; i < reactions.Length; i++) {
				lastReactionQuantity[i] = 0;
			}

			Sync ();
			
			time = 0;
		}
		
		private ReactionType[] reactions;
		private ulong[] lastReactionQuantity;
		private	double time;

		private void Sync()
		{ 
			CueUpdate update;
			while (updateQueue.Dequeue(out update))
			{
				if (update is CompartmentChangedUpdate)
				{
				}
				else if (update is ReactionAddedUpdate)
				{
				}
				else if (update is ReactionChangedUpdate)
				{
				}
				else if (update is ReactionRemovedUpdate)
				{
				}
				else if (update is SpeciesAddedUpdate)
				{
				}
				else if (update is SpeciesQuantityUpdate)
				{
				}
				else if (update is SpeciesRemovedUpdate)
				{
				}
				else
				{ throw new UpdateNotSupportedException(update); }
			}
		}
		
		public SimulationStep Step(double stepDuration)
		{
			Sync ();

			time += stepDuration;
			
			ReactionCount[] rc = new ReactionCount[reactions.Length];
			
			for (int i = 0; i < reactions.Length; i++) {
				ReactionType reaction = reactions[i];
			
				ulong q = (ulong)System.Math.Floor(reaction.Rate * time);
				ulong lastQ = lastReactionQuantity[i];
				
				ulong count = q - lastQ;
				
				lastReactionQuantity[i] = q;
				
				rc[i] = new ReactionCount(reaction, count); 
			}
			
			return new SimulationStep(rc);
		}
		
		public void Dispose() { }
	}
}