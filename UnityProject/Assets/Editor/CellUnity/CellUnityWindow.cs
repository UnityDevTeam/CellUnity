using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CellUnity;

[System.Serializable]
public class CellUnityWindow : EditorWindow
{
	[MenuItem ("Window/CellUnity")]
	public static void Init () {
		EditorWindow.GetWindow <CellUnityWindow>();
	}
	
	void OnEnable ()
	{
		hideFlags = HideFlags.HideAndDontSave;
	}
	
	void OnGUI () {
		CueEditor.CueGui (CUE.GetInstance ());
	}

}