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
			collection.Add (molecule);
			Debug.Log ("MC++ m "+molecule.Species.ToString());
		}

		public void Remove(Molecule molecule) {
			collection.Remove (molecule);
		}

		public void ResetAllReactions() {
			foreach (Molecule item in collection) {
				item.ReactionPrep = null;
			}
		}

		public Molecule FindNearestMoleculeForReaction(Molecule reference, MoleculeSpecies species)
		{
			Vector3 position = reference.transform.position;

			float minDistance = float.MaxValue;
			Molecule result = null;
			foreach (var m in collection) {

				float distance = Vector3.Distance(position, m.transform.position);
				if ((species.Equals(m.Species)) && (m.ReactionPrep == null) && (distance < minDistance) && (reference != m))
				{
					minDistance = distance;
					result = m;
				}
			}

			return result;
		}

		public Molecule FindRandomMoleculeForReaction(MoleculeSpecies species)
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
				Debug.Log("random: "+i+" /"+list.Count);
				return list[i];
			}
			else
			{ return null; }
		}
	}
}