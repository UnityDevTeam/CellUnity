using UnityEngine;
using System.Collections.Generic;

namespace CellUnity.Dispensing
{
	public class DispenserBoxGridSphere: DispenserBoxGrid
	{
		protected override void OnCreate(List<DispenserBox> boxes)
		{
			float volumeCubes = MinBoxCount * Mathf.Pow(Size, 3);
		
			// V_kugel = 4/3*pi*r^3  --> r = (3/4 * V/pi)^(1/3)
			// diameter = 2*r
			long diameter = (long) Mathf.Ceil( (2 * Mathf.Pow(3f/4f * volumeCubes/Mathf.PI, 1f/3f))/Size );
			
			while (boxes.Count < MinBoxCount)
			{
				boxes.Clear();
					
				float radiusSquared = (diameter / 2f) * (diameter / 2f);
				
				long min = (long)Mathf.Round(-(diameter / 2f));
				long max = diameter + min;
				
				for (long z = min; z <=max; z++)
				{
					for (long y = min; y <=max; y++)
					{
						for (long x = min; x <=max; x++)
						{
							// check if cube is in Sphere
							if (x*x + y*y +z*z <= radiusSquared)
							{
								DispenserBox box = new DispenserBox(new Vector3(x*Size, y*Size, z*Size), Size);
							
								boxes.Add(box);
							}
						}
					}
				}	
				
				diameter++;
			}
		}
	}
}