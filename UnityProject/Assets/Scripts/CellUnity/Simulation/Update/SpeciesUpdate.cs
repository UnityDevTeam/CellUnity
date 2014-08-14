using System;

namespace CellUnity.Simulation.Update
{
	public abstract class SpeciesUpdate : CueUpdate
	{
		public SpeciesUpdate (MoleculeSpecies species)
		{
			this.Species = species;
		}

		protected override bool OnCanReplaceSameType (CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			if (((SpeciesUpdate)otherUpdate).Species == Species)
			{
				return OnCanReplaceSameTypeSameSpecies(otherUpdate, out newUpdate);
			}
			else
			{
				newUpdate = null;
				return false;
			}
		}
		
		protected virtual bool OnCanReplaceSameTypeSameSpecies(CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			newUpdate = null;
			return false;
		}

		public MoleculeSpecies Species { get; private set; }
	}
}

