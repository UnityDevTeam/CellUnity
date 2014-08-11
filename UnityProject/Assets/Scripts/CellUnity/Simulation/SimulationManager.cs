using UnityEngine;
using System.Collections.Generic;
using CellUnity.Reaction;
using System.Threading;
using System;

namespace CellUnity.Simulation
{
	public class SimulationManager
	{
		public SimulationManager()
		{
			this.simulator = new SimpleSimulator();
			
		}
		
		private ISimulator simulator;
		private CUE cue;
		
		public void Reset()
		{
			this.cue = CUE.GetInstance();
			
			SimulatorScript simulatorScript = GameObject.FindObjectOfType<SimulatorScript>();
			if (simulatorScript == null)
			{
				GameObject simulatorScriptGameObject = new GameObject("SimulatorScript");
				simulatorScriptGameObject.AddComponent<CellUnity.Simulation.SimulatorScript>();
			}
		
			Stop ();
			
			simulationThread = new Thread(new ThreadStart(RunSimulation));
			simulationThread.IsBackground = true;
		
			CUE cue = CUE.GetInstance();
			simulator.Init(cue.Species, cue.ReactionTypes);
		}
		
		public void Start()
		{
			if (simulationThread == null)
			{
				Reset();
			}
		
			simulationThread.Start();
		}
		
		public void Stop()
		{
			if (simulationThread != null)
			{
				simulationThread.Abort();
				simulationThread.Join();
				simulationThread = null;
			}
		}
		
		private void RunSimulation()
		{
			DateTime nextStep = DateTime.Now;
		
			try
			{
				while (true)
				{
					while (nextStep > DateTime.Now) {
						int sleepTime = System.Math.Max (0, (int)((nextStep - DateTime.Now).TotalMilliseconds * 0.95));
						Thread.Sleep(sleepTime);
					}
				
					Debug.Log("Step "+nextStep.ToLongTimeString());
					SimulationStep step = simulator.Step(cue.SimulationStep);
					EnqueueStep(step);
					
					nextStep = nextStep.AddSeconds(cue.VisualizationStep);
				}
			}
			catch (ThreadAbortException)
			{
			
			}
		}
		
		private Thread simulationThread;
		
		private Queue<SimulationStep> stepsQueue = new Queue<SimulationStep>();
		private readonly object stepsQueueLock = new object();
		
		private void EnqueueStep(SimulationStep step)
		{
			lock (stepsQueueLock)
			{
				stepsQueue.Enqueue(step);
			}
		}
		
		public void Update()
		{
			while (true) {
				SimulationStep step;
				lock (stepsQueueLock)
				{
					if (stepsQueue.Count == 0) { break; }
					step = stepsQueue.Dequeue();
				}
				
				CUE cue = CUE.GetInstance();
				
				foreach (ReactionCount item in step.Reactions)
				{
					for (int i = 0; i < item.Count; i++)
					{
						cue.ReactionManager.InitiateReaction(item.ReactionType);	
					}
				}
			}
		}
	}
}