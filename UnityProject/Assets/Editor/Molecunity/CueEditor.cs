using UnityEngine;
using System.Collections;
using UnityEditor;
using CellUnity;
using CellUnity.Model.Pdb;

[CustomEditor(typeof(CUE))]
public class CueEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		CueGui((CUE)target);
	}

	private static MoleculeSpeciesListView listViewMolecules = new MoleculeSpeciesListView ();
	private static ReactionTypeListView listViewReactions = new ReactionTypeListView();

	private static void Space() {
		GUILayout.Space (10);
	}

	public static void CueGui(CUE CUE) {

		EditorGUILayout.LabelField ("ID", CUE.ID.ToString());
		
		GUILayout.Label ("Molecule Species", EditorStyles.boldLabel);
		
		listViewMolecules.Gui (CUE.Species);

		Space ();
		
		GUILayout.Label ("Add Molecule", EditorStyles.miniLabel);
		
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("From Selected")) {
			
			var item = CUE.CreateMoleculeSpecies();
			CUE.AddSpecies(item);
			
			EditorUtility.SetDirty (CUE);
			EditorUtility.SetDirty (item);
			
			listViewMolecules.FoldOpen(item);
		}
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal ();

		if (GUILayout.Button ("From PDB File")) {
			PdbImport pdbImport = new PdbImport();
			pdbImport.UserSelectFile();
		}

		if (GUILayout.Button ("Download From PDB")) {
			PdbImportWindow.UserDownload();
		}
		GUILayout.EndHorizontal ();
		
		Space ();
		GUILayout.Label ("Reactions", EditorStyles.boldLabel);
		
		listViewReactions.Gui (CUE.ReactionTypes);
		
		Space ();
		GUILayout.Label ("Add Reaction", EditorStyles.miniLabel);
		if (GUILayout.Button ("Add Reaction")) {
			var item = CUE.CreateReactionType();
			CUE.AddReaction (item);
			EditorUtility.SetDirty (CUE);
			
			listViewReactions.FoldOpen(item);
		}
		
		Space ();
		GUILayout.Label ("Debug", EditorStyles.boldLabel);
		if (GUILayout.Button ("Save Assets")) {
			AssetDatabase.SaveAssets();
		}
		
		Space ();

		if (GUILayout.Button ("Reset")) {
			CUE.ResetData();
		}
	}
}