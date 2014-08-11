using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CellUnity;
using CellUnity.Reaction;

public class MoleculeSpeciesListView : ListView<MoleculeSpecies>
{
	protected override void OnItemGui (MoleculeSpecies item)
	{
		if (item == null) {
			base.OnItemGui(item);
			return;
		}

		item.Name = EditorGUILayout.TextField ("Name", item.Name);
		item.InitialQuantity = EditorGUILayout.IntField("Initial Quantity", item.InitialQuantity);
		item.Mass = EditorGUILayout.FloatField("Mass", item.Mass);
		EditorGUILayout.LabelField("Size", item.Size.ToString());
		//EditorGUILayout.LabelField("ID", item.GetInstanceID().ToString());

		EditorUtility.SetDirty (item);

		if (GUILayout.Button ("remove")) {
			CUE cue = CUE.GetInstance();
			cue.RemoveSpecies(item);

			EditorUtility.SetDirty(cue);
		}
	}
}

public class ReactionTypeListView : ListView<ReactionType>
{
	protected override void OnItemGui (ReactionType item)
	{
		CUE cue = CUE.GetInstance ();
		
		if (item == null) {
			base.OnItemGui(item);
			return;
		}

		item.Name = EditorGUILayout.TextField ("Name", item.Name);

		EditorGUILayout.BeginHorizontal ();
		
		item.Reagents = GuiSpeciesList(cue, item.Reagents);
		
		EditorGUILayout.LabelField (" \u2192 ", GUILayout.MaxWidth(30));
		
		item.Products = GuiSpeciesList(cue, item.Products);
		
		EditorGUILayout.EndHorizontal ();
		
		item.Rate = EditorGUILayout.FloatField("Rate", item.Rate);

		EditorUtility.SetDirty (item);

		if (Application.isPlaying) {
			if (GUILayout.Button ("start reaction")) {
				cue.ReactionManager.InitiateReaction(item);
			}
		}

		if (GUILayout.Button ("remove")) {
			cue.RemoveReaction(item);

			EditorUtility.SetDirty(cue);
		}
	}
	
	private MoleculeSpecies[] GuiSpeciesList(CUE cue, MoleculeSpecies[] speciesArray)
	{
		MoleculeSpeciesPopup speciesPopup = new MoleculeSpeciesPopup (cue, speciesArray.Length > 0);
	
		List<MoleculeSpecies> species = new List<MoleculeSpecies>(speciesArray);
		species.Add(null);
		
		for (int i = 0; i < species.Count; i++) {
			if (i != 0) { EditorGUILayout.LabelField ("+", GUILayout.MaxWidth(10)); }
			
			species[i] = speciesPopup.Popup (species[i]);
		}
		
		while (species.Contains(null)) {
			species.Remove(null);
		}
		
		return species.ToArray();
	}
}