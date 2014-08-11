using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CellUnity;
using CellUnity.Reaction;

public class MoleculeSpeciesPopup {
	
	public MoleculeSpeciesPopup(CUE cue, bool nullPossible)
	{
		MoleculeSpecies[] cueSpecies = cue.Species;
		
		moleculeSpecies = new MoleculeSpecies[cueSpecies.Length + (nullPossible ? 1 : 0)];
		
		moleculeSpeciesString = new string[moleculeSpecies.Length];
		
		int i = 0;
		
		if (nullPossible)
		{
			moleculeSpecies[i] = null;
			moleculeSpeciesString[i] = "none";
			i++;
		}
		
		foreach (var item in cueSpecies) {
			moleculeSpecies[i] = item;
			moleculeSpeciesString[i] = item.Name;
			i++;
		}
	}
	
	private string[] moleculeSpeciesString;
	private MoleculeSpecies[] moleculeSpecies;
	
	private MoleculeSpecies GetSelection(MoleculeSpecies[] species, int index)
	{
		if (index < 0 || index >= species.Length) { return null; }
		return species [index];
	}
	
	public MoleculeSpecies Popup(MoleculeSpecies selected)
	{
		int i = ArrayUtility.IndexOf<MoleculeSpecies> (moleculeSpecies, selected);
		i = EditorGUILayout.Popup (i, moleculeSpeciesString);
		return GetSelection (moleculeSpecies, i);
	}
}
