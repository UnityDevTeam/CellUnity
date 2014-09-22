using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CellUnity.Dispensing
{
	/// <summary>
	/// Dispenser. Used to place a defined initial quantity of molecules automatically
	/// and randomly. The initial quantity can be set in the species editor, and
	/// placed in the placing menu.
	/// </summary>
	public class Dispenser
	{
		public Dispenser()
		{

		}

		/// <summary>
		/// The center point where the molecules are placed.
		/// </summary>
		public Vector3 Location = Vector3.zero;
		/// <summary>
		/// The radius in nm defining a sphere around the center point (defined in Location)
		/// in which the molecules are placed.
		/// </summary>
		public float Radius = 2;

		/// <summary>
		/// Delay in seconds after which the molecule's drag is reset to
		/// its default value.
		/// </summary>
		public float DragDelay = 2;
		/// <summary>
		/// The initial drag that is set when a molecule is created. After the
		/// amount of time defined in DragDelay, the drag is reset to its
		/// default value
		/// </summary>
		public float InitialDrag = 1000;

		/// <summary>
		/// Queue Containing the species of each molecule that should be
		/// created and placed.
		/// </summary>
		private Queue<MoleculeSpecies> molecules = new Queue<MoleculeSpecies> ();

		/// <summary>
		/// Adds a specific amount of molecules to the placement queue.
		/// </summary>
		/// <param name="species">Species to place</param>
		/// <param name="count">Quantity of that species to place</param>
		public void AddMolecules(MoleculeSpecies species, int count)
		{
			for (int i = 0; i < count; i++)
			{
				molecules.Enqueue(species);
			}
		}

		/// <summary>
		/// Place all the molecules in the placement queue.
		/// </summary>
		public void Place()
		{
			while (molecules.Count > 0)
			{
				// Get next species to add
				MoleculeSpecies s = molecules.Dequeue();

				// Get random location
				// s.Size is subtracted so never half of the molecule reaches outside the defined radius 
				Vector3 l = Location + Random.insideUnitSphere * (Utility.Utils.ScaleFromNm(Radius)-s.Size);

				// create molecule
				Molecule m = s.CreateMolecule(l);

				// Add DelayedDrag component that resets the drag after a
				// specified amount of time
				DelayedDrag delayedDrag = m.gameObject.AddComponent<DelayedDrag>();
				delayedDrag.delay = DragDelay;
				delayedDrag.initialDrag = InitialDrag;
				delayedDrag.drag = Molecule.Drag;
			}
		}
	}
}
