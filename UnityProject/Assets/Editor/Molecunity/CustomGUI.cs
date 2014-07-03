using UnityEngine;
using UnityEditor;

// Source: http://answers.unity3d.com/questions/216584/horizontal-line.html

static class CustomGUI {
	
	public static readonly GUIStyle splitter;
	
	static CustomGUI() {
		splitter = new GUIStyle();
		splitter.normal.background = EditorGUIUtility.whiteTexture;
		splitter.stretchWidth = true;
		splitter.margin = new RectOffset(0, 0, 0, 0);
	}
	
	private static readonly Color splitterColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);
	
	// GUILayout Style
	public static void Splitter(Color rgb, float thickness) {
		Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitter, GUILayout.Height(thickness));
		
		if (Event.current.type == EventType.Repaint) {
			Color restoreColor = GUI.color;
			GUI.color = rgb;
			splitter.Draw(position, false, false, false, false);
			GUI.color = restoreColor;
		}
	}
	
	public static void Splitter(float thickness, GUIStyle splitterStyle) {
		Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Height(thickness));
		
		if (Event.current.type == EventType.Repaint) {
			Color restoreColor = GUI.color;
			GUI.color = splitterColor;
			splitterStyle.Draw(position, false, false, false, false);
			GUI.color = restoreColor;
		}
	}

	public static void Splitter() {
		Splitter (1);
	}

	public static void SplitterSoft() {
		Splitter (new Color(0f,0f,0f,0.1f),1);
	}
	
	public static void Splitter(float thickness) {
		Splitter(thickness, splitter);
	}
	
	// GUI Style
	public static void Splitter(Rect position) {
		if (Event.current.type == EventType.Repaint) {
			Color restoreColor = GUI.color;
			GUI.color = splitterColor;
			splitter.Draw(position, false, false, false, false);
			GUI.color = restoreColor;
		}
	}
	
}