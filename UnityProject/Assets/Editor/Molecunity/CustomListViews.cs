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
		EditorGUILayout.LabelField("ID", item.GetInstanceID().ToString());

		EditorUtility.SetDirty (item);

		if (GUILayout.Button ("remove")) {
			CUE CUE = CUE.GetInstance();
			CUE.RemoveSpecies(item);

			EditorUtility.SetDirty(CUE);
		}
	}
}

public class ReactionTypeListView : ListView<ReactionType>
{
	protected override void OnItemGui (ReactionType item)
	{
		if (item == null) {
			base.OnItemGui(item);
			return;
		}
		
		item.Name = EditorGUILayout.TextField ("Name", item.Name);
		//EditorGUILayout.LabelField("ID", item.GetInstanceID().ToString());

		EditorUtility.SetDirty (item);

		if (GUILayout.Button ("remove")) {
			CUE CUE = CUE.GetInstance();
			CUE.RemoveReaction(item);

			EditorUtility.SetDirty(CUE);
		}
	}
}