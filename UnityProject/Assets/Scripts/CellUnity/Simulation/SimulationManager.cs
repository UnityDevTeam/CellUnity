using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;
using CellUnity.Reaction;
using CellUnity.Simulation.Update;

namespace CellUnity.Simulation
{
	public class SimulationManager
	{
		public SimulationManager()
		{
			runInMainThread = true;
		}
		
		private ISimulator simulator;
		private SimulationState state;
		private readonly bool runInMainThread;
		
		public void Reset()
		{
			if (simulator == null)
			{
				this.simulator = new Copasi.CopasiSimulator();
				simulator.Init(GetNewUpdateQueue());
			}

			SimulatorScript simulatorScript = GameObject.FindObjectOfType<SimulatorScript>();
			if (simulatorScript == null)
			{
				GameObject simulatorScriptGameObject = new GameObject("SimulatorScript");
				simulatorScriptGameObject.AddComponent<CellUnity.Simulation.SimulatorScript>();
			}
			
			Stop ();
			
			simulationThread = new Thread(new ThreadStart(RunSimulation));
			simulationThread.IsBackground = true;
		}
		
		public SimulationState State { get { return state; } }
		
		public void Start()
		{
			if (state == SimulationState.Stopped)
			{
				if (simulationThread == null)
				{
					Reset();
				}

				state = SimulationState.Running;

				if (!runInMainThread)
				{
					simulationThread.Start();
				}
				
				Debug.Log("Simulation start");
			}
			else if (state == SimulationState.Paused)
			{
				state = SimulationState.Running;
			}
		}

		public void Pause()
		{
			if (state == SimulationState.Stopped)
			{
				Start ();
			}

			if (state == SimulationState.Running)
			{
				state = SimulationState.Paused;
			}
		}
		
		public void Stop()
		{
			if ((state == SimulationState.Running || state == SimulationState.Paused) && simulationThread != null)
			{
				if (!runInMainThread)
				{
					simulationThread.Abort();
					simulationThread.Join();
				}
				simulationThread = null;
				
				state = SimulationState.Stopped;
				
				Debug.Log("Simulation stop");
			}
		}

		private void SimulateStep()
		{
			CUE cue = CUE.GetInstance ();

			SimulationStep step = simulator.Step(cue.SimulationStep);
			EnqueueStep(step);
			nextStep = nextStep.AddSeconds(cue.VisualizationStep);
		}

		public void MainThreadRunSimulation()
		{
			if (state == SimulationState.Paused)
			{
				nextStep = DateTime.Now.AddSeconds(0.5);
			}
			else if (state == SimulationState.Stopped)
			{
				return;
			}
			else if (state == SimulationState.Running)
			{
				if (nextStep > DateTime.Now)
				{
					SimulateStep();
				}
			}
		}

		DateTime nextStep;

		private void RunSimulation()
		{
			nextStep = DateTime.Now;
			try
			{
				while (true)
				{
					while (nextStep > DateTime.Now) {
						int sleepTime = System.Math.Max (0, (int)((nextStep - DateTime.Now).TotalMilliseconds * 0.95));
						Thread.Sleep(sleepTime);
					}

					while (state == SimulationState.Paused)
					{
						Thread.Sleep(500);
						nextStep = DateTime.Now;
					}

					SimulateStep();
				}
			}
			catch (ThreadAbortException)
			{
				Debug.Log("Thread abort");
				// is never executed, I don't know why
				// doesn't Unity support threading?
			}
			finally
			{
				// is never executed, I don't know why
				// doesn't Unity support threading?
			}
		}

		private List<UpdateQueue> updateQueues = new List<UpdateQueue>();
		
		private UpdateQueue GetNewUpdateQueue()
		{
			CUE cue = CUE.GetInstance ();

			UpdateQueue updateQueue = new UpdateQueue ();

			updateQueue.Enqueue(new CompartmentChangedUpdate(cue));

			foreach (var item in cue.Species)
			{
				updateQueue.Enqueue(new SpeciesAddedUpdate(item));
				updateQueue.Enqueue(new SpeciesQuantityUpdate(item, cue.Molecules.GetQuantity(item)));
			}
			
			foreach (var item in cue.ReactionTypes) {
				updateQueue.Enqueue(new ReactionAddedUpdate(item));
			}
			
			updateQueues.Add (updateQueue);

			return updateQueue;
		}
		
		public void UpdateSimulator(CueUpdate update)
		{
			foreach (var updateQueue in updateQueues) {
				updateQueue.Enqueue(update);
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
			if (runInMainThread)
			{
				MainThreadRunSimulation();
			}

			while (true) {
				SimulationStep step;
				lock (stepsQueueLock)
				{
					if (stepsQueue.Count == 0) { break; }
					step = stepsQueue.Dequeue();
				}
				
				CUE cue = CUE.GetInstance();
				
				System.Text.StringBuilder info = new System.Text.StringBuilder(); // TODO: remove
				
				foreach (ReactionCount item in step.Reactions)
				{
					info.Append(item.ReactionType.ToString()+": "+item.Count+";   ");
				
					for (ulong i = 0; i < item.Count; i++)
					{
						cue.ReactionManager.InitiateReaction(item.ReactionType);	
					}
				}
				
				//Debug.Log(info.ToString());
			}
		}
	}
}