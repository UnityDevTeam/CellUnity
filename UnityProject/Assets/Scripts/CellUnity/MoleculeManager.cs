using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Reaction;
using CellUnity.Utility;

namespace CellUnity
{
	/// <summary>
	/// MoleculeManager. The assignment of the MoleculeManager is to keep track of all molecules in the system.
	/// When the play mode in Unity is activated, each molecule registers itself to the MoleculeManager of the CUE. 
	/// In the manager, all molecules are organized in separate MoleculeCollections, depending on their species and depending on
	/// if they are free or already in use for a reaction. These lists are implemented as doubly linked lists.
	/// One molecule can only be in one list at a time. This enables to find free molecules quickly and efficient.
	/// 
	/// The molecules are registering themselfes in play mode, because the MoleculeManager is reset by Unity when
	/// playmode is activated.
	/// </summary>
	public class MoleculeManager {
		
		public MoleculeManager() { }

		/// <summary>
		/// contains all molecules in sets, dependent on their species
		/// </summary>
		private ShortKeyDict<MoleculeSpecies, MoleculeSet> collection = new ShortKeyDict<MoleculeSpecies, MoleculeSet>();

		/// <summary>
		/// A MoleculeSet manages one species. It 
		/// contains 2 MoleculeCollections, a Free list and a Reacting list.
		/// </summary>
		private class MoleculeSet
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="CellUnity.MoleculeManager+MoleculeSet"/> class.
			/// </summary>
			/// <param name="species">Species of the MolculeSet</param>
			public MoleculeSet(MoleculeSpecies species)
			{
				Free = new MoleculeCollection(species.Name + " free");
				Reacting = new MoleculeCollection(species.Name + " reacting");
			}

			/// <summary>
			/// Contains free molecules of this species
			/// </summary>
			/// <value>The free.</value>
			public MoleculeCollection Free { get; private set; }
			/// <summary>
			/// Contains reacting molecules of this sepcies
			/// </summary>
			/// <value>The reacting.</value>
			public MoleculeCollection Reacting { get; private set; }
		}

		/// <summary>
		/// Registers a molecule to the MoleculeManager
		/// All molecules have to call this method once. The molecule added
		/// must not have a ReactionPrep assigned.
		/// </summary>
		/// <param name="molecule">Molecule to add, must not be null</param>
		public void Add(Molecule molecule) {
			if (molecule == null) { throw new System.ArgumentException("molecule must not be null"); }

			// try to find a entry of the species
			ShortKeyDict<MoleculeSpecies, MoleculeSet>.Entry entry;
			if (!collection.Find (molecule.Species, out entry))
			{
				// species not found -> create a new entry
				entry = collection.Set(molecule.Species, new MoleculeSet(molecule.Species));
			}

			// add the molecule to the free list.
			entry.Value.Free.Add (molecule);
		}

		/// <summary>
		/// Removes a molecule from the MoleculeManager. Each molecule must call
		/// this method when it is deleted.
		/// </summary>
		/// <param name="molecule">Molecule.</param>
		public void Remove(Molecule molecule)
		{
			if (molecule.Collection != null)
			{
				molecule.Collection.Remove(molecule);
			}
		}

		/// <summary>
		/// Called by the Molecule's AssignReactionPrep method.
		/// Removes the molecule from the free-list and adds it to the reacting-list
		/// </summary>
		/// <param name="molecule">Molecule.</param>
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

		/// <summary>
		/// Called by the Molecule's ClearReactionPrep method.
		/// Removes the molecule from the reacting-list and doesn't add it to free again.
		/// Therefore the molecule is removed from the molecule manager.
		/// This Method is called when a reaction was performed and the molecule will be deleted  next.
		/// It can not be added to free again, to avoid it is assigned to a new reaction as free molecule.
		/// </summary>
		/// <param name="molecule">Molecule.</param>
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

		/// <summary>
		/// Called by the Molecule's ReleaseReactionPrep method.
		/// Removes the molecule from reacting-list and adds it to the free-list again.
		/// </summary>
		/// <param name="molecule">Molecule.</param>
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

		/// <summary>
		/// Gets the quantity of molecules of a specific species in the environment.
		/// Returns only the free molecules.
		/// </summary>
		/// <returns>The count of molecules</returns>
		/// <param name="species">Species.</param>
		public ulong GetQuantity(MoleculeSpecies species)
		{
			if (Application.isPlaying) 
			{
				ShortKeyDict<MoleculeSpecies, MoleculeSet>.Entry entry;
				if (collection.Find (species, out entry))
				{
					return entry.Value.Free.Count;
				}
				else { return 0; }
			}
			else
			{
				// when not playing, the molecules are not registered to the molecule
				// manager. Therefore they have to be counted this way:

				ulong count = 0;
				foreach (var m in GameObject.FindObjectsOfType<Molecule>()) {
					if (m.Species == species)
					{
						count++;
					}
				}
				return count;
			}
		}

		/// <summary>
		/// Finds free molecuels for a new ReactinPrep.
		/// If not enough molecules are found, the ReactionPrep is not released. It has to be done by the caller.
		/// </summary>
		/// <returns><c>true</c>, if molecuels for reaction were found, <c>false</c> if not enough molecules are availalble.</returns>
		/// <param name="reactionPrep">ReactionPrep. Must not have any Molecules added alreay.</param>
		/// <param name="referenceMolecule">Molecule that is wished to be part of the reaction</param>
		public bool FindMolecuelsForReaction(ReactionPrep reactionPrep, Molecule referenceMolecule)
		{
			// species needed for the reaction
			List<MoleculeSpecies> species = new List<MoleculeSpecies>(reactionPrep.ReactionType.Reagents);

			// add reference
			reactionPrep.AddMolecule (referenceMolecule);

			// remove the species from the reference as it is already added
			if (!species.Remove(referenceMolecule.Species))
			{ throw new System.Exception("Species of referenceMolecule isn't needed in reaction"); }

			// find near molecules for missing species.
			Molecule m;
			for (int i = 0; i < species.Count; i++) {
				if (FindNearestMoleculeForReaction(referenceMolecule, species[i], out m)) 
				{ reactionPrep.AddMolecule(m); }
				else
				{ return false; }
			}

			// successful
			return true;
		}

		/// <summary>
		/// Finds free molecuels for a new ReactinPrep.
		/// If not enough molecules are found, the ReactionPrep is not released. It has to be done by the caller.
		/// </summary>
		/// <returns><c>true</c>, if molecuels for reaction were found, <c>false</c> if not enough molecules are availalble.</returns>
		/// <param name="reactionPrep">ReactionPrep. Must not have any Molecules added alreay.</param>
		public bool FindMolecuelsForReaction(ReactionPrep reactionPrep)
		{
			MoleculeSpecies[] species = reactionPrep.ReactionType.Reagents;
			
			if (species.Length > 0)
			{
				Molecule m;

				if (FindRandomMoleculeForReaction(species[0], out m))
				{
					return FindMolecuelsForReaction(reactionPrep, m);
				}
				else
				{ return false; }
			}
			
			return true;
		}

		/// <summary>
		/// Finds the nearest free molecule of a specific species.
		/// </summary>
		/// <returns><c>true</c>, if nearest molecule for reaction was found, <c>false</c> otherwise.</returns>
		/// <param name="reference">Reference.</param>
		/// <param name="species">Species.</param>
		/// <param name="molecule">Molecule.</param>
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

		/// <summary>
		/// Finds a random free molecule of a specific species.
		/// </summary>
		/// <returns><c>true</c>, if random molecule for reaction was found, <c>false</c> otherwise.</returns>
		/// <param name="species">Species.</param>
		/// <param name="molecule">Molecule.</param>
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
				if (i == free.Count) { i--; } // because Random.value's maximum is inclusive 1

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