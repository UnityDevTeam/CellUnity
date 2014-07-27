using UnityEngine;
using System.Collections.Generic;

namespace CellUnity.Model.Dispensing
{
	public abstract class DispenserBoxGrid
	{
		public DispenserBoxGrid()
		{
		}
		
		public long MinBoxCount { get; set; }
		public float Size { get; set; }
		
		public List<DispenserBox> Create()
		{
			List<DispenserBox> boxes = new List<DispenserBox>();
			OnCreate(boxes);
			return boxes;
		}
		
		protected abstract void OnCreate(List<DispenserBox> boxes);
	}
}