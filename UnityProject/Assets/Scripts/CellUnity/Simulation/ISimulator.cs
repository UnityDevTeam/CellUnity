using UnityEngine;
using System.Collections;
using CellUnity.Reaction;

namespace CellUnity.Simulation
{
	public interface ISimulator : System.IDisposable
	{
		void Init(MoleculeSpecies[] species, ReactionType[] reactions);
		SimulationStep Step(double stepDuration);
	}
}