using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ListView<T>
{
	public void Gui (IEnumerable<T> items) {
		OnGui (items);
	}

	private HashSet<T> foldedObjects = new HashSet<T>();
	private void FoldObject(T obj, bool fold) {
		if (fold) {
			foldedObjects.Add (obj);
		} else {
			foldedObjects.Remove(obj);
		}
	}
	
	private bool IsFolded(T obj) {
		return foldedObjects.Contains (obj);
	}

	public void FoldOpen(T obj) {
		FoldObject (obj, true);
	}

	private bool GuiFold(T obj, string name) {
		
		bool folded = IsFolded(obj);
		
		// Handle Clicks on the Foldout title, and design stuff...
		
		//CustomGUI.Splitter ();
		
		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.GetControlRect (true, 16f, EditorStyles.foldout);
		Rect foldRect = GUILayoutUtility.GetLastRect ();
		if (Event.current.type == EventType.MouseUp && foldRect.Contains (Event.current.mousePosition)) {
			folded = !folded;
			GUI.changed = true;
			Event.current.Use ();
		}
		
		folded = EditorGUI.Foldout (foldRect, folded, name);
		EditorGUI.EndChangeCheck ();
		
		if (folded) {
			CustomGUI.Splitter ();
		}
		
		FoldObject (
			obj,
			folded
			);
		
		return folded;
	}

	protected void OnGui(IEnumerable<T> items) {
		EditorGUILayout.BeginVertical ("TextArea");
		
		bool hasItems = false;
		
		foreach (var item in items) {
			
			hasItems = true;
			
			EditorGUILayout.BeginVertical ("Box");
			
			if (item == null) {
				GUILayout.Label("null");
			}
			else {
				if (GuiFold(item, item.ToString())) {
					OnItemGui(item);
				}
			}
			
			EditorGUILayout.EndVertical ();
		}
		
		if (!hasItems) {
			GUILayout.Label("no items");
		}
		
		EditorGUILayout.EndVertical ();
	}

	protected virtual void OnItemGui(T item) {
		EditorGUILayout.LabelField (item == null ? "null" : item.ToString ());
	}
}