using UnityEngine;
using System.Collections;
using UnityEditor;
using CellUnity;
using CellUnity.Model;
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

	public static void CueGui(CUE cue) {

		EditorGUILayout.LabelField ("ID", cue.ID.ToString());
		
		GUILayout.Label ("Molecule Species", EditorStyles.boldLabel);
		
		listViewMolecules.Gui (cue.Species);

		Space ();
		
		GUILayout.Label ("Add Molecule", EditorStyles.miniLabel);
		
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("From Selection")) {

			MoleculeCreator creator = new MoleculeCreator();
			creator.gameObjects = Selection.gameObjects;
			MoleculeSpecies species = creator.Create();
			
			listViewMolecules.FoldOpen(species);
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
		
		listViewReactions.Gui (cue.ReactionTypes);
		
		Space ();
		GUILayout.Label ("Add Reaction", EditorStyles.miniLabel);
		if (GUILayout.Button ("Add Reaction")) {
			var item = cue.CreateReactionType();
			cue.AddReaction (item);
			EditorUtility.SetDirty (cue);
			
			listViewReactions.FoldOpen(item);
		}
		
		Space ();
		GUILayout.Label ("Debug", EditorStyles.boldLabel);
		if (GUILayout.Button ("Save Assets")) {
			AssetDatabase.SaveAssets();
		}
		if (GUILayout.Button ("Start Reaction")) {
			cue.Molecules.ResetAllReactions();
			cue.ReactionManager.PerformReaction(cue.ReactionTypes[0]);
		}

		Space ();

		if (GUILayout.Button ("Reset")) {
			cue.ResetData();
		}
	}
}