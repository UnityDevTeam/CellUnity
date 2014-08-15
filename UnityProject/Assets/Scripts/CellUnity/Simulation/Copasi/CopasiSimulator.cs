using System.Collections;
using System.Collections.Generic;
using org.COPASI;
using System.Diagnostics;
using System;
using CellUnity.Reaction;

namespace CellUnity.Simulation.Copasi
{
	public class CopasiSimulator : ISimulator
	{
		public CopasiSimulator()
		{
			copasi = new Copasi();
		}

		private Copasi copasi;
		private CUE cue;
		private double currentTime;
		private List<CopasiReactionGroup> reactionList = new List<CopasiReactionGroup> ();
		
		public void Init(CUE cue)
		{
			this.cue = cue;

			Reload ();
		}

		public void Reload()
		{
			reactionList.Clear();

			if (copasi != null)
			{
				copasi.Dispose();
				copasi = null;
			}

			copasi = new Copasi ();

			copasi.UpdateCompartmentVolume(cue.Volume);
			
			foreach (var item in cue.Species)
			{
				copasi.AddSpecies(item);
				
				copasi.UpdateSpeciesQuantity(
					copasi.GetMetab(item),
					cue.Molecules.GetQuantity(item)
					);
			}
			
			foreach (var item in cue.ReactionTypes)
			{
				ReactionType reactionType = item;
				CReaction copasiReaction = copasi.AddReaction(reactionType);
				CModelValue modelValue = copasi.AddReactionParticleFluxValue(copasiReaction);
				
				reactionList.Add(new CopasiReactionGroup(copasiReaction, reactionType, modelValue));
			}

			copasi.InitTrajectoryTask ();

			UnityEngine.Debug.Log("copasi: compiling...");
			copasi.CompileAndUpdate();
			UnityEngine.Debug.Log("copasi: saving...");
			copasi.SaveModel("model.cps");
			UnityEngine.Debug.Log("copasi: model updated");

			currentTime = 0;
		}

		public SimulationStep Step(double stepDuration)
		{
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
		
			// clean up
			trajectoryTask.restore();
			
			return new SimulationStep(reactionCount);
		}
		
		public void Dispose()
		{
			if (copasi != null)
			{
				copasi.Dispose();
				copasi = null;
			}
		}
		
	}
}