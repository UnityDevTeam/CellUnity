using System;

namespace CellUnity.Simulation.Update
{
	public abstract class CueUpdate
	{
		public CueUpdate ()
		{
		}

		public bool CanReplace(CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			return OnCanReplace (otherUpdate, out newUpdate);
		}

		protected virtual bool OnCanReplace (CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			if (GetType ().Equals (otherUpdate.GetType ()))
			{
				return OnCanReplaceSameType(otherUpdate, out newUpdate);
			}
			else
			{
				newUpdate = null;
				return false;
			}
		}

		protected virtual bool OnCanReplaceSameType(CueUpdate otherUpdate, out CueUpdate newUpdate)
		{
			newUpdate = null;
			return false;
		}
	}
}