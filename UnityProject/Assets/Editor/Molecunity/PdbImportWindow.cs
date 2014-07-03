using UnityEngine;
using UnityEditor;
using System.Collections;
using CellUnity.Model.Pdb;

public class PdbImportWindow : EditorWindow {

	private string pdbName = "";
	public PdbImport pdbImport = new PdbImport ();

	public static void UserDownload() {
		PdbImportWindow window = (PdbImportWindow)EditorWindow.GetWindow <PdbImportWindow>();
		window.pdbImport = new PdbImport ();
	}

	void OnGUI() {

		pdbName = EditorGUILayout.TextField("PDB Name", pdbName);
		
		if (GUILayout.Button("Download Molecule")) {
			OnDownloadClick();
			GUIUtility.ExitGUI();
		}
	}
	
	void OnDownloadClick() {
		pdbName = pdbName.Trim ();

		if (pdbImport != null) {

			//pdbImport.DownloadFile("http://www.rcsb.org/pdb/download/downloadFile.do?fileFormat=pdb&compression=NO&structureId="+WWW.EscapeURL(pdbName));

			//if (pdbImport.DownloadFile("http://test.xa1.at/data/test.pdb")) {
			if (pdbImport.DownloadFile("http://www.rcsb.org/pdb/download/downloadFile.do?fileFormat=pdb&compression=NO&structureId="+WWW.EscapeURL(pdbName), pdbName)) {

			}
		}
	}
}
