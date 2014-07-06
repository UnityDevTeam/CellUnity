using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CellUnity;
using CellUnity.Reaction;

public class MoleculeSpeciesPopup {
	
	public MoleculeSpeciesPopup(CUE cue)
	{
		moleculeSpecies = cue.Species;
		
		moleculeSpeciesString = new string[moleculeSpecies.Length];
		for (int i = 0; i < moleculeSpecies.Length; i++) {
			moleculeSpeciesString[i] = moleculeSpecies[i].Name;
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
