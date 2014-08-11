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
		
		//item.Name = EditorGUILayout.TextField ("Name", item.Name);
		//EditorGUILayout.LabelField("ID", item.GetInstanceID().ToString());

		MoleculeSpeciesPopup speciesPopup = new MoleculeSpeciesPopup (cue, false);
		MoleculeSpeciesPopup speciesPopupNullable = new MoleculeSpeciesPopup (cue, true);

		EditorGUILayout.BeginHorizontal ();
		item.Reagent1 = speciesPopup.Popup (item.Reagent1);
		EditorGUILayout.LabelField (" + ", GUILayout.MaxWidth(20));
		item.Reagent2 = speciesPopupNullable.Popup (item.Reagent2);
		EditorGUILayout.LabelField (" + ", GUILayout.MaxWidth(20));
		item.Reagent3 = speciesPopupNullable.Popup (item.Reagent3);
		EditorGUILayout.LabelField (" \u2192 ", GUILayout.MaxWidth(30));
		item.Product1 = speciesPopup.Popup (item.Product1);
		EditorGUILayout.LabelField (" + ", GUILayout.MaxWidth(20));
		item.Product2 = speciesPopupNullable.Popup (item.Product2);
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
}