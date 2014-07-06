using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Utility;

namespace CellUnity
{
	[System.Serializable]
	public class MoleculeCollection : ScriptableObject {
		
		public MoleculeCollection() { }
		
		void OnEnable ()
		{
			hideFlags = HideFlags.HideInHierarchy;
		}

		//private Dictionary<MoleculeSpecies, HashSet<Molecule>> collection = new Dictionary<MoleculeSpecies, HashSet<Molecule>>();
		private HashSet<Molecule> collection = new HashSet<Molecule>();

		public void Add(Molecule molecule) {
			collection.Add (molecule);
		}

		public void Remove(Molecule molecule) {
			collection.Remove (molecule);
		}

		public Molecule FindNearestMoleculeForReaction(Molecule reference, MoleculeSpecies species)
		{
			//reference.gameObject.transform.position

			Vector3 position = reference.transform.position;

			float minDistance = float.MaxValue;
			Molecule result = null;
			foreach (var m in collection) {

				float distance = Vector3.Distance(position, m.transform.position);
				if ((species.Equals(m.Species)) && (m.ReactionPrep == null) && (distance < minDistance))
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
				return list[(int)(Random.value * list.Count)];
			}
			else
			{ return null; }
		}
	}
}