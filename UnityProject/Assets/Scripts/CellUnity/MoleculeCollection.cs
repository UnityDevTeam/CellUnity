using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Utility;

namespace CellUnity
{
	public class MoleculeCollection {
		
		public MoleculeCollection() { }
		
		//void OnEnable ()
		//{
		//	hideFlags = HideFlags.HideInHierarchy;
		//}

		//private Dictionary<MoleculeSpecies, HashSet<Molecule>> collection = new Dictionary<MoleculeSpecies, HashSet<Molecule>>();
		private HashSet<Molecule> collection = new HashSet<Molecule>();

		public void Add(Molecule molecule) {
			if (molecule == null) { throw new System.ArgumentException("molecule must not be null"); }
			collection.Add (molecule);
		}

		public void Remove(Molecule molecule) {
			collection.Remove (molecule);
		}

		public void ResetAllReactions() {
			foreach (Molecule item in collection) {
				item.ReactionPrep = null;
			}
		}

		public bool FindMolecuelsForReaction(MoleculeSpecies[] species, out Molecule[] molecules)
		{
			molecules = new Molecule[species.Length];
			
			if (species.Length > 0)
			{
				if (!FindRandomMoleculeForReaction(species[0], out molecules[0])) { return false; }
				for (int i = 1; i < molecules.Length; i++) {
					if (!FindRandomMoleculeForReaction(species[i], out molecules[i])) { return false; }
				}
			}
			
			return true;
		}

		private bool FindNearestMoleculeForReaction(Molecule reference, MoleculeSpecies species, out Molecule molecule)
		{
			Vector3 position = reference.transform.position;

			float minDistance = float.MaxValue;
			foreach (var m in collection) {

				float distance = Vector3.Distance(position, m.transform.position);
				if ((species.Equals(m.Species)) && (m.ReactionPrep == null) && (distance < minDistance) && (reference != m))
				{
					minDistance = distance;
					molecule = m;
					return true;
				}
			}

			molecule = null;
			return false;
		}

		private bool FindRandomMoleculeForReaction(MoleculeSpecies species, out Molecule molecule)
		{
			List<Molecule> list = new List<Molecule> ();
			foreach (var m in collection)
			{
				if (species.Equals(m.Species) && (m.ReactionPrep == null))
				{
					list.Add(m);
				}
			}

			if (list.Count > 0)
			{
				int i = (int)(Random.value * list.Count);
				molecule = list[i];
				return true;
			}
			else
			{
				molecule = null;
				return false;
		  	}
		}
	}
}