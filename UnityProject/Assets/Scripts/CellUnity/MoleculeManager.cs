using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Reaction;
using CellUnity.Utility;

namespace CellUnity
{
	public class MoleculeManager {
		
		public MoleculeManager() { }

		private ShortKeyDict<MoleculeSpecies, MoleculeSet> collection = new ShortKeyDict<MoleculeSpecies, MoleculeSet>();

		private class MoleculeSet
		{
			public MoleculeSet()
			{
				Free = new MoleculeCollection();
				Reacting = new MoleculeCollection();
			}

			public MoleculeCollection Free { get; private set; }
			public MoleculeCollection Reacting { get; private set; }
		}

		public void Add(Molecule molecule) {
			if (molecule == null) { throw new System.ArgumentException("molecule must not be null"); }

			ShortKeyDict<MoleculeSpecies, MoleculeSet>.Entry entry;
			if (!collection.Find (molecule.Species, out entry))
			{
				entry = collection.Set(molecule.Species, new MoleculeSet());
			}

			entry.Value.Free.Add (molecule);
		}

		public void Remove(Molecule molecule)
		{
			if (molecule.Collection != null)
			{
				molecule.Collection.Remove(molecule);
			}
		}

		public void AssignReactionPrep(Molecule molecule)
		{
			ShortKeyDict<MoleculeSpecies, MoleculeSet>.Entry entry;
			if (collection.Find (molecule.Species, out entry))
			{
				MoleculeSet ms = entry.Value;

				ms.Free.Remove(molecule);
				ms.Reacting.Add(molecule);
			}
			else
			{ 
				throw new System.Exception("unknown species "+molecule.Species); 
			}
		}

		// molecule must be deleted afterwards. it is removed from all lists
		public void ClearReactionPrep(Molecule molecule)
		{
			ShortKeyDict<MoleculeSpecies, MoleculeSet>.Entry entry;
			if (collection.Find (molecule.Species, out entry))
			{
				MoleculeSet ms = entry.Value;
				
				ms.Reacting.Remove(molecule);
			}
			else { throw new System.Exception("unknown species"); }
		}

		public void ReleaseReactionPrep(Molecule molecule)
		{
			ShortKeyDict<MoleculeSpecies, MoleculeSet>.Entry entry;
			if (collection.Find (molecule.Species, out entry))
			{
				MoleculeSet ms = entry.Value;
				
				ms.Reacting.Remove(molecule);
				ms.Free.Add(molecule);
			}
			else { throw new System.Exception("unknown species"); }
		}

		public ulong GetQuantity(MoleculeSpecies species)
		{
			ShortKeyDict<MoleculeSpecies, MoleculeSet>.Entry entry;
			if (collection.Find (species, out entry))
			{
				return entry.Value.Free.Count;
			}
			else { return 0; }
		}

		public bool FindMolecuelsForReaction(ReactionPrep reactionPrep)
		{
			MoleculeSpecies[] species = reactionPrep.ReactionType.Reagents;
			
			if (species.Length > 0)
			{
				Molecule m;
				Molecule firstMolecule;

				if (FindRandomMoleculeForReaction(species[0], out m))
				{
					reactionPrep.AddMolecule(m);
					firstMolecule = m;
				}
				else
				{ return false; }

				for (int i = 1; i < species.Length; i++) {
					if (FindNearestMoleculeForReaction(firstMolecule, species[i], out m)) 
					{ reactionPrep.AddMolecule(m); }
					else
					{ return false; }
				}
			}
			
			return true;
		}

		private bool FindNearestMoleculeForReaction(Molecule reference, MoleculeSpecies species, out Molecule molecule)
		{
			Vector3 position = reference.Position;

			float minDistance = float.MaxValue;

			molecule = null;

			ShortKeyDict<MoleculeSpecies, MoleculeSet>.Entry entry;
			if (collection.Find (species, out entry))
			{
				MoleculeSet ms = entry.Value;
				
				foreach (var m in ms.Free) {
					float distance = Vector3.Distance(position, m.Position);
					if ((distance < minDistance) && (reference != m))
					{
						minDistance = distance;
						molecule = m;
					}
				}
			}
			else
			{ 
				// molecule species not listed --> molecule == null --> false is returned
			}

			return (molecule != null);
		}

		private bool FindRandomMoleculeForReaction(MoleculeSpecies species, out Molecule molecule)
		{
			ShortKeyDict<MoleculeSpecies, MoleculeSet>.Entry entry;
			if (collection.Find (species, out entry))
			{
				MoleculeCollection free = entry.Value.Free;

				ulong c = free.Count;

				if (c == 0)
				{ 
					molecule = null;
					return false;
				}

				uint i = (uint)(Random.value * c);
				if (i == free.Count) { i--; } // possible because Random.value's maximum is inclusive 1

				foreach (var m in free) {
					if (i == 0)
					{
						molecule = m;
						return true;
					}
					i--;
				}

				throw new System.Exception("should not be possible");
			}
			else
			{
				molecule = null;
				return false;
			}
		}
	}
}