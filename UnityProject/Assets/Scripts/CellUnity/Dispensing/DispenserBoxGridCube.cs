using UnityEngine;
using System.Collections.Generic;

namespace CellUnity.Dispensing
{
	public class DispenserBoxGridCube : DispenserBoxGrid
	{
		protected override void OnCreate(List<DispenserBox> boxes)
		{
			long side = (long)System.Math.Ceiling(System.Math.Pow(MinBoxCount, 1.0 / 3.0));
			float a = Size * (side-1) / 2f;
			
			for (long z = 0; z < side; z++)
			{
				for (long y = 0; y < side; y++)
				{
					for (long x = 0; x < side; x++)
					{
						DispenserBox box = new DispenserBox(new Vector3(x*Size - a, y*Size - a, z*Size - a), Size);
						
						boxes.Add(box);
					}
				}
			}
		}
	}
}