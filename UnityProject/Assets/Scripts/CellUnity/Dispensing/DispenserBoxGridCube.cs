using UnityEngine;
using System.Collections.Generic;

namespace CellUnity.Model.Dispensing
{
	public class DispenserBoxGridCube : DispenserBoxGrid
	{
		protected override void OnCreate(List<DispenserBox> boxes)
		{
			long side = (long)System.Math.Ceiling(System.Math.Pow(MinBoxCount, 1.0 / 3.0));
			
			for (long z = 0; z < side; z++)
			{
				for (long y = 0; y < side; y++)
				{
					for (long x = 0; x < side; x++)
					{
						DispenserBox box = new DispenserBox(new BoxLocation(x, y, z), Size);
						
						boxes.Add(box);
					}
				}
			}
		}
	}
}