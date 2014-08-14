using System.Collections;
using System.Collections.Generic;
using org.COPASI;
using System.Diagnostics;
using System;
using CellUnity.Reaction;
using CellUnity.Simulation.Update;

namespace CellUnity.Simulation.Copasi
{
	public class CopasiSimulator : ISimulator
	{
		public CopasiSimulator()
		{
			copasi = new Copasi();
		}

		private Copasi copasi;
		private UpdateQueue updateQueue;
		private double currentTime;
		private List<CopasiReactionGroup> reactionList;
		
		public void Init(UpdateQueue updateQueue)
		{
			this.updateQueue = updateQueue;

			reactionList = new List<CopasiReactionGroup> ();
		
			Sync ();

			copasi.InitTrajectoryTask ();

			currentTime = 0;
		}

		private void Sync()
		{
			CueUpdate update;
			bool updateCopasi = false;
			while (updateQueue.Dequeue(out update))
			{
				if (update is CompartmentChangedUpdate)
				{
					copasi.UpdateCompartmentVolume(((CompartmentChangedUpdate)update).Volume);
				}
				else if (update is ReactionAddedUpdate)
				{
					copasi.AddReaction(((ReactionAddedUpdate)update).Reaction);
				}
				else if (update is ReactionChangedUpdate)
				{
					ReactionChangedUpdate reactionUpdate = (ReactionChangedUpdate)update;

					copasi.UpdateReaction(
						copasi.GetReaction(reactionUpdate.Reaction),
						reactionUpdate.Reagents,
						reactionUpdate.Products,
						reactionUpdate.Rate
						);
				}
				else if (update is ReactionRemovedUpdate)
				{
					copasi.RemoveReaction(((ReactionRemovedUpdate)update).Reaction);
				}
				else if (update is SpeciesAddedUpdate)
				{
					copasi.AddSpecies(((SpeciesAddedUpdate)update).Species);
				}
				else if (update is SpeciesQuantityUpdate)
				{
					SpeciesQuantityUpdate speciesUpdate = (SpeciesQuantityUpdate)update;

					copasi.UpdateSpeciesQuantity(
						copasi.GetMetab(speciesUpdate.Species),
						speciesUpdate.Quantity
						);
				}
				else if (update is SpeciesRemovedUpdate)
				{
					copasi.RemoveSpecies(((SpeciesRemovedUpdate)update).Species);
				}
				else
				{ throw new UpdateNotSupportedException(update); }

				updateCopasi = true;
			}

			if (updateCopasi)
			{
				UnityEngine.Debug.Log("copasi: compiling...");
				copasi.CompileAndUpdate();
				UnityEngine.Debug.Log("copasi: saving...");
				copasi.SaveModel("model.cps");
				UnityEngine.Debug.Log("copasi: model updated");
			}
		}

		public SimulationStep Step(double stepDuration)
		{
			Sync ();

			CTrajectoryTask trajectoryTask = copasi.TrajectoryTask;

			CTrajectoryProblem problem = (CTrajectoryProblem)trajectoryTask.getProblem();
			
			problem.setDuration(stepDuration);
			
			currentTime += stepDuration;
			
			try
			{
				// now we run the actual trajectory
				trajectoryTask.processWithOutputFlags(true, (int)CCopasiTask.NO_OUTPUT);
			}
			catch
			{
				if (CCopasiMessage.size() > 0)
				{
					throw new System.Exception("Running the time course simulation failed: " + CCopasiMessage.getAllMessageText(true));
				}
				
				throw new System.Exception("Running the time course simulation failed");
			}
			
			// Update the species properties that have changed
			ReactionCount[] reactionCount = new ReactionCount[reactionList.Count];
			
			for (int i = 0; i < reactionList.Count; i++)
			{
				CopasiReactionGroup r = reactionList[i];
				
				reactionCount[i] = r.CalcParticleFlux();
			}
			
			//UpdateSimulation();
			
			// clean up
			trajectoryTask.restore();
			
			return new SimulationStep(reactionCount);
		}
		
		public void Dispose()
		{

		}
		
	}
}