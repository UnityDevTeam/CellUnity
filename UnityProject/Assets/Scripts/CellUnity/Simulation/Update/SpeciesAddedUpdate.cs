using System;

namespace CellUnity.Simulation.Update
{
	public class SpeciesAddedUpdate : SpeciesUpdate
	{
		public SpeciesAddedUpdate (MoleculeSpecies species) : base(species)
		{
		}

		protected override bool OnCanReplaceSameTypeSameSpecies (CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			newUpdate = this;
			return true;
		}
	}
}

