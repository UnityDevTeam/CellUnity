using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CellUnity.Dispensing
{
	public class Dispenser
	{
		private MoleculeSizeQueue molecules = new MoleculeSizeQueue();
		private List<DispenserBox> boxesAvailable;
		
		public float BoxSize { get; set; }
		public float MinimumBoxSize { get; set; }
		public float MoleculeBoxDistance { get; set; }
		
		public void AddMolecules(MoleculeSpecies species, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Molecule m = species.CreateMolecule();
				molecules.Enqueue(m);
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
			DispenserBox testBox = new DispenserBox(new BoxLocation(0, 0, 0), BoxSize);
			
			foreach (var m in molecules)
			{
				float size = m.Species.Size + 2 * MoleculeBoxDistance;
				
				while (testBox.SubSize >= size && testBox.SubSize >= MinimumBoxSize)
				{
					// let's put it into a Sub Box
					
					boxesNeeded = boxesNeeded / testBox.SubBoxes.Length;
					
					testBox = testBox.SubBoxes[0];
				}
				
				minBoxCount += boxesNeeded;
			}
			
			DispenserBoxGrid grid = new DispenserBoxGridCube();
			
			grid.Size = BoxSize;
			grid.MinBoxCount = (long)System.Math.Ceiling(minBoxCount);
			
			boxesAvailable = grid.Create();
		}
		
		private void FillGrid()
		{
			System.Random random = new System.Random(1809);
			
			while (molecules.Count > 0)
			{
				Molecule m = molecules.Dequeue();
				
				int boxIndex = random.Next(0, boxesAvailable.Count);
				DispenserBox box = boxesAvailable[boxIndex];
				
				Vector3 location;
				if (box.FindSpace(m.Species.Size, MinimumBoxSize, random, out location))
				{
					m.Position = location;
					
					if (box.Occupation == BoxOccupation.Occupied)
					{
						boxesAvailable.RemoveAt(boxIndex);
					}
				}
				else
				{ throw new System.Exception("no space left, should not be possible"); }
			}
		}
		
		public static void Test()
		{
			CUE cue = CUE.GetInstance();
		
			Dispenser d = new Dispenser();
			
			d.BoxSize = 0;
			d.MinimumBoxSize = 0.1f;
			d.MoleculeBoxDistance = 0f;
			
			foreach (var species in cue.Species) {
				d.AddMolecules(species, species.InitialQuantity);
				d.BoxSize = Mathf.Max(d.BoxSize, species.Size);
			}
			
			d.Place();
		}
	}
}
