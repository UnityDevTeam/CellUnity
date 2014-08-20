using UnityEngine;
using System.Collections;
using UnityEditor;
using CellUnity;
using CellUnity.Model;
using CellUnity.Model.Pdb;
using CellUnity.Dispensing;
using CellUnity.Simulation;
using CellUnity.Utility;

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
	
	private static Dispenser dispenser = new Dispenser();
	
	public static void CueGui(CUE cue) {

		EditorGUILayout.LabelField ("ID", cue.ID.ToString());
		
		GUILayout.Label ("Molecule Species", EditorStyles.boldLabel);
		
		listViewMolecules.Gui (cue.Species);

		Space ();
		
		GUILayout.Label ("Add Molecule", EditorStyles.miniLabel);
		
		/*GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("From Selection")) {

			MoleculeCreator creator = new MoleculeCreator();
			creator.gameObjects = Selection.gameObjects;
			MoleculeSpecies species = creator.Create();
			
			listViewMolecules.FoldOpen(species);
		}
		GUILayout.EndHorizontal ();*/
		
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
		GUILayout.Label ("Placing", EditorStyles.boldLabel);

		GUILayout.BeginHorizontal ();
		dispenser.Radius = EditorGUILayout.FloatField("Radius [nm]:", dispenser.Radius);
		if (GUILayout.Button("f.Sim", GUILayout.MaxWidth(50)))
		{
			dispenser.Radius = Utils.GetSphereRadius(cue.Volume);
		}
		GUILayout.EndHorizontal ();
		
		if (GUILayout.Button ("Remove all Molecules")) {
			cue.RemoveMolecules();
		}
		
		if (GUILayout.Button ("Place Molecules")) {
			
			foreach (var species in cue.Species) {
				dispenser.AddMolecules(species, species.InitialQuantity);
			}
			
			dispenser.Place();
		}
		
		Space();
		
		GUILayout.Label ("Simulation", EditorStyles.boldLabel);

		cue.Volume = Mathf.Max (0, EditorGUILayout.FloatField("Volume [nl]:", (float)cue.Volume));
		/*bool volumeGizom = EditorGUILayout.Toggle ("Volume Visible:", cue.ScriptManager.HasScript<CellUnity.View.VolumeGizmo> ());
		if (volumeGizom)
		{ cue.ScriptManager.GetOrAddScript<CellUnity.View.VolumeGizmo>(); }
		else
		{ cue.ScriptManager.RemoveScript<CellUnity.View.VolumeGizmo>(); }
		*/


		EditorGUILayout.LabelField("Radius [nm]", Utils.GetSphereRadius(cue.Volume).ToString());
		cue.SimulationStep = Mathf.Max (0, EditorGUILayout.FloatField("Simulation Step [s]:", (float)cue.SimulationStep));
		cue.VisualizationStep = Mathf.Max (0, EditorGUILayout.FloatField("Visualization Step [s]:", (float)cue.VisualizationStep));
		
		GUILayout.BeginHorizontal ();
		
		if (GUILayout.Button ("Reload",  GUILayout.MaxWidth(80))) {
			cue.SimulationManager.Reload();
		}

		SimulationState simulationState = cue.SimulationManager.State;
		int oldSimulatorSelection;
		if (simulationState == SimulationState.Running)
		{ oldSimulatorSelection = 0; }
		else if (simulationState == SimulationState.Paused)
		{ oldSimulatorSelection = 1; }
		else if (simulationState == SimulationState.Stopped)
		{ oldSimulatorSelection = 2; }
		else { oldSimulatorSelection = -1; }

		int simulaorSelection =
			GUILayout.SelectionGrid(oldSimulatorSelection, new string[] {"Start", "Pause", "Stop"} , 3 );

		if (simulaorSelection != oldSimulatorSelection)
		{
			if (simulaorSelection == 0)
			{ cue.SimulationManager.Start(); }
			else if (simulaorSelection == 1)
			{ cue.SimulationManager.Pause(); }
			else if (simulaorSelection == 2)
			{ cue.SimulationManager.Stop(); }
		}

		GUILayout.EndHorizontal ();
		
		Space ();
		GUILayout.Label ("Export", EditorStyles.boldLabel);
		if (GUILayout.Button ("Export SBML...")) {

			string filename = EditorUtility.SaveFilePanel ("Export SBML", "", "model.xml", "xml");
			new CellUnity.Export.SbmlExportCopasi(cue).Export(filename);
		}
 		Space ();

		GUILayout.Label ("Debug", EditorStyles.boldLabel);
		if (GUILayout.Button ("Auto Run Reactions")) {
			GameObject autoRun = new GameObject("AutoRun");
			autoRun.AddComponent<AutoReaction>();
		}
		
		if (GUILayout.Button ("Save Assets")) {
			AssetDatabase.SaveAssets();
		}

		if (GUILayout.Button("Compartment"))
	    {
			cue.MakeCompartment();
		}

		Space ();

		//if (GUILayout.Button ("Reset")) {
		//	cue.ResetData();
		//}
	}
}