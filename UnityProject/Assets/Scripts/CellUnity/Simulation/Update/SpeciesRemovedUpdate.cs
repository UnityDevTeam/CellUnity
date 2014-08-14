using System;

namespace CellUnity.Simulation.Update
{
	public class SpeciesRemovedUpdate : SpeciesUpdate
	{
		public SpeciesRemovedUpdate (MoleculeSpecies species) : base(species)
		{
		}

		protected override bool OnCanReplaceSameTypeSameSpecies (CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			newUpdate = this;
			return true;
		}
	}
}

