using System;

namespace CellUnity.Simulation.Update
{
	public class UpdateNotSupportedException : Exception
	{
		public UpdateNotSupportedException (CueUpdate update) : base("Update Type "+update.GetType().Name+" not supported")
		{

		}
	}
}

