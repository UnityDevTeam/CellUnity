using System;

namespace CellUnity.Simulation.Update
{
	public class CompartmentChangedUpdate : CueUpdate
	{
		public CompartmentChangedUpdate (CUE cue)
		{
		}

		public double Volume { get; set; }
		
		protected override bool OnCanReplaceSameType (CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			newUpdate = this;
			return true;
		}
	}
}

