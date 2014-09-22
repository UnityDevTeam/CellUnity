using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;
using CellUnity.Reaction;

namespace CellUnity.Simulation
{
	/// <summary>
	/// Simulation manager.
	/// The simulation is started and administered by the simulation manager.
	/// The manager is also responsible for the data transfer with the simulator
	/// as well as the utilization of the simulation results.
	/// </summary>
	public class SimulationManager
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Simulation.SimulationManager"/> class.
		/// </summary>
		public SimulationManager()
		{
			runInMainThread = true;
		}

		/// <summary>
		/// Simulator used by CellUnity
		/// </summary>
		private ISimulator simulator;
		/// <summary>
		/// Current state of the simulator
		/// </summary>
		private SimulationState state;
		/// <summary>
		/// Can only be set in constructor.
		/// If true, the simulation is run in the main thread,
		/// if false, a new thread is started for the simulation.
		/// </summary>
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
			{ state = SimulationState.Stopped; }
		}

		/// <summary>
		/// Gets the current state of the simulator.
		/// </summary>
		/// <value>Simulator state</value>
		public SimulationState State { get { return state; } }

		/// <summary>
		/// Starts the simulation.
		/// Can only be called in Play Mode
		/// </summary>
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

		/// <summary>
		/// Pauses the Simulation.
		/// </summary>
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

		/// <summary>
		/// Stops the Simulation.
		/// </summary>
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

		/// <summary>
		/// Simulates one step.
		/// </summary>
		private void SimulateStep()
		{
			Debug.Log ("Step");

			CUE cue = CUE.GetInstance ();

			SimulationStep step = simulator.Step(cue.SimulationStep);
			EnqueueStep(step);
			nextStep = nextStep.AddSeconds(cue.VisualizationStep);
		}

		/// <summary>
		/// Can be called every frame. Runs a Simulation Step if necessary.
		/// </summary>
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

		/// <summary>
		/// Time that indicates when the next simulation step is simulated.
		/// </summary>
		DateTime nextStep;

		/// <summary>
		/// Simulation Thread Method.
		/// </summary>
		private void RunSimulation()
		{
			try
			{
				while (true)
				{
					// Free CPU by call Thread.Sleep
					while (nextStep > DateTime.Now) {
						int sleepTime = System.Math.Max (0, (int)((nextStep - DateTime.Now).TotalMilliseconds * 0.95));
						Thread.Sleep(sleepTime);
					}

					// Wait while Simulation is paused
					while (state == SimulationState.Paused)
					{
						Thread.Sleep(500);
						nextStep = DateTime.Now;
					}

					// Simulate Step
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

		/// <summary>
		/// Simulation thread.
		/// </summary>
		private Thread simulationThread;

		/// <summary>
		/// Queue which holds simulation results
		/// </summary>
		private Queue<SimulationStep> stepsQueue = new Queue<SimulationStep>();
		/// <summary>
		/// Thread lock object of the stepsQueue.
		/// </summary>
		private readonly object stepsQueueLock = new object();

		/// <summary>
		/// Enqueue a Simulation Result. Thread-safe.
		/// </summary>
		/// <param name="step">Simulation Result.</param>
		private void EnqueueStep(SimulationStep step)
		{
			lock (stepsQueueLock)
			{
				stepsQueue.Enqueue(step);
			}
		}

		/// <summary>
		/// Should be called every frame.
		/// Processes Simulation results and initiates reactions.
		/// </summary>
		public void Update()
		{
			if (runInMainThread)
			{
				MainThreadRunSimulation();
			}

			// Process Simulation Results
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