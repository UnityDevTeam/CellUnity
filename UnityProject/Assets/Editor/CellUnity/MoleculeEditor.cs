using UnityEngine;
using System.Collections;
using UnityEditor;
using CellUnity;

[CustomEditor(typeof(Molecule))]
public class MoleculeEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		Molecule t = (Molecule)target;
		CUE cue = CUE.GetInstance ();

		MoleculeSpeciesPopup speciesPopup = new MoleculeSpeciesPopup (cue);

		t.Species = speciesPopup.Popup (t.Species);

		EditorUtility.SetDirty (t);

		if (GUILayout.Button("show CellUnity Editor")) {
			CellUnityWindow.Init();
		}
	}
}