using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;
using CellUnity.Reaction;

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
		
		public void Reload()
		{
			CUE cue = CUE.GetInstance ();

			SimulationState oldState = state;
			if (state != SimulationState.Stopped)
			{
				Stop();
			}

			if (simulator == null)
			{
				this.simulator = new Copasi.CopasiSimulator();
				simulator.Init(cue);
			}
			else
			{
				simulator.Reload();
			}

			cue.ScriptManager.GetOrAddScript<CellUnity.Simulation.SimulatorScript>();
			
			simulationThread = new Thread(new ThreadStart(RunSimulation));
			simulationThread.IsBackground = true;

			// restore old state

			if (oldState == SimulationState.Running)
			{ Start(); }
			else if (oldState == SimulationState.Paused)
			{ Pause(); }
			else if (oldState == SimulationState.Stopped)
			{state = SimulationState.Stopped; }
		}
		
		public SimulationState State { get { return state; } }
		
		public void Start()
		{
			if (!Application.isPlaying) 
			{
				throw new System.Exception("can only simulate in Play Mode");
			}

			if (state == SimulationState.Stopped)
			{
				Reload();

				state = SimulationState.Running;

				nextStep = DateTime.Now.AddSeconds(0.5);

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
				Debug.Log("Simulation pause");
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
			Debug.Log ("Step");

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
				if (nextStep <= DateTime.Now)
				{
					SimulateStep();
				}
			}
		}

		DateTime nextStep;

		private void RunSimulation()
		{
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
						cue.ReactionManager.InitiateReaction(item.ReactionType, true);	
					}
				}
				
				Debug.Log(info.ToString());
			}
		}
	}
}