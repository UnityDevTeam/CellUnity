using System;

namespace CellUnity.Simulation.Update
{
	public class SpeciesQuantityUpdate : SpeciesUpdate
	{
		public SpeciesQuantityUpdate (MoleculeSpecies species, ulong quantity) : base(species)
		{
			this.Quantity = quantity;
		}

		public ulong Quantity { get; private set; }

		protected override bool OnCanReplaceSameTypeSameSpecies (CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			newUpdate = this;
			return true;
		}
	}
}

