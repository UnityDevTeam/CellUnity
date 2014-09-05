using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CellUnity.Dispensing
{
	public class Dispenser
	{
		public Dispenser()
		{

		}

		public Vector3 Location = Vector3.zero;
		public float Radius = 2;

		public float DragDelay = 2;
		public float InitialDrag = 1000;

		private Queue<MoleculeSpecies> molecules = new Queue<MoleculeSpecies> ();
		
		public void AddMolecules(MoleculeSpecies species, int count)
		{
			for (int i = 0; i < count; i++)
			{
				molecules.Enqueue(species);
			}
		}
		
		public void Place()
		{
			while (molecules.Count > 0)
			{
				MoleculeSpecies s = molecules.Dequeue();

				Vector3 l = Location + Random.insideUnitSphere * Utility.Utils.ScaleFromNm((Radius - s.Size));

				Molecule m = s.CreateMolecule(l);
				DelayedDrag delayedDrag = m.gameObject.AddComponent<DelayedDrag>();
				delayedDrag.delay = DragDelay;
				delayedDrag.initialDrag = InitialDrag;
				delayedDrag.drag = Molecule.Drag;
			}
		}
	}
}
