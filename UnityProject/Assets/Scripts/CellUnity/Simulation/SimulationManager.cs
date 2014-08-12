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
			//this.simulator = new Copasi.CopasiSimulator();
		}
		
		private ISimulator simulator;
		private CUE cue;
		private bool running = false;
		
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
		
		public bool IsRunning { get { return running; } }
		
		public void Start()
		{
			if (!running)
			{
				running = true;
				if (simulationThread == null)
				{
					Reset();
				}
			
				simulationThread.Start();
				
				Debug.Log("Simulation start");
			}
		}
		
		public void Stop()
		{
			if (simulationThread != null)
			{
				simulationThread.Abort();
				simulationThread.Join();
				simulationThread = null;
				
				// do not set running = false. this is done in the thread
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

					SimulationStep step = simulator.Step(cue.SimulationStep);
					EnqueueStep(step);
					
					nextStep = nextStep.AddSeconds(cue.VisualizationStep);
				}
			}
			catch (ThreadAbortException)
			{
			
			}
			finally
			{
				running = false;
				Debug.Log("Simulation stop");
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
				
				System.Text.StringBuilder info = new System.Text.StringBuilder();
				
				foreach (ReactionCount item in step.Reactions)
				{
					info.Append(item.ReactionType.ToString()+": "+item.Count+";   ");
				
					for (ulong i = 0; i < item.Count; i++)
					{
						cue.ReactionManager.InitiateReaction(item.ReactionType);	
					}
				}
				
				Debug.Log(info.ToString());
			}
		}
	}
}