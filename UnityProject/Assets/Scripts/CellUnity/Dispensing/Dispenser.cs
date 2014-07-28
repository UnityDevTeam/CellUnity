using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CellUnity.Dispensing
{
	public class Dispenser
	{
		public Dispenser()
		{
			BoxSize = 0;
			MinimumBoxSize = 0.5f;
			MoleculeBoxDistance = 0.05f;
		}
	
		private MoleculeSizeQueue molecules = new MoleculeSizeQueue();
		private List<DispenserBox> boxesAvailable;
		
		public float BoxSize { get; set; }
		public float MinimumBoxSize { get; set; }
		public float MoleculeBoxDistance { get; set; }
		
		public void AddMolecules(MoleculeSpecies species, int count)
		{
			for (int i = 0; i < count; i++)
			{
				molecules.Enqueue(species);
			}
		}
		
		public void Place()
		{
			CreateGrid();
			FillGrid();
		}
		
		private void CreateGrid()
		{
			double minBoxCount = 0;
			double boxesNeeded = 1;
			
			BoxSize = Mathf.Max(BoxSize, MinimumBoxSize);
			
			DispenserBox testBox = new DispenserBox(new Vector3(0, 0, 0), BoxSize);
			
			foreach (var m in molecules)
			{
				float size = m.Size + 2 * MoleculeBoxDistance;
				
				while (testBox.SubSize >= size && testBox.SubSize >= MinimumBoxSize)
				{
					// let's put it into a Sub Box
					
					boxesNeeded = boxesNeeded / testBox.SubBoxes.Length;
					
					testBox = testBox.SubBoxes[0];
				}
				
				minBoxCount += boxesNeeded;
			}
			
			DispenserBoxGrid grid = new DispenserBoxGridSphere();
			
			grid.Size = BoxSize;
			grid.MinBoxCount = (long)System.Math.Ceiling(minBoxCount);
			
			Debug.Log("Boxes: " + grid.MinBoxCount.ToString());
			
			boxesAvailable = grid.Create();
		}
		
		private void FillGrid()
		{
			System.Random random = new System.Random(1809);
			
			while (molecules.Count > 0)
			{
				MoleculeSpecies s = molecules.Dequeue();
				
				int boxIndex = random.Next(0, boxesAvailable.Count);
				DispenserBox box = boxesAvailable[boxIndex];
				
				Vector3 location;
				if (box.FindSpace(s.Size, MinimumBoxSize, random, out location))
				{
					s.CreateMolecule(location);
					
					if (box.Occupation == BoxOccupation.Occupied)
					{
						boxesAvailable.RemoveAt(boxIndex);
					}
				}
				else
				{ throw new System.Exception("no space left, should not be possible"); }
			}
		}
	}
}
