
//using System;
using UnityEditor;
using UnityEngine;

class CreatePrefabFromSelected
{
	const string menuTitle = "GameObject/Create Prefab From Selected";
	
	/// <summary>
	/// Creates a prefab from the selected game object.
	/// </summary>
	[MenuItem (menuTitle)]
	static void CreatePrefab ()
	{
		GameObject obj = Selection.activeGameObject;
		string name = obj.name;
		
		Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + name + ".prefab");
		PrefabUtility.ReplacePrefab(obj, prefab);
		AssetDatabase.Refresh();
	}
	
	/// <summary>
	/// Validates the menu.
	/// </summary>
	/// <remarks>The item will be disabled if no game object is selected.</remarks>
	[MenuItem (menuTitle, true)]
	static bool ValidateCreatePrefab ()
	{
		return Selection.activeGameObject != null;
	}
}

/*
public class MyWindow : EditorWindow
{
	[SerializeField]
	private SerializeMe2 m_SerialziedThing;
	
	[MenuItem ("Window/SerializationY")]
	static void Init () {
		GetWindow <MyWindow>();
	}
	
	void OnEnable ()
	{
		hideFlags = HideFlags.HideAndDontSave;
		if (m_SerialziedThing == null)
			m_SerialziedThing = new SerializeMe2 ();
	}
	
	void OnGUI () {
		GUILayout.Label ("Serialized Things", EditorStyles.boldLabel);
		m_SerialziedThing.OnGUI ();
	}
}



[Serializable]
public class NestedClass : ScriptableObject
	
{
	
	[SerializeField]
	
	private float m_StructFloat;
	
	public void OnGUI()
		
	{
		
		m_StructFloat = EditorGUILayout.FloatField("Float", m_StructFloat);
		
	}
	
}



[Serializable]

public class SerializeMe
	
{
	
	[SerializeField]
	
	private NestedClass m_Class1;
	
	
	
	[SerializeField]
	
	private NestedClass m_Class2;
	
	
	
	public void OnGUI ()
		
	{
		
		if (m_Class1 == null)
			
		m_Class1 = ScriptableObject.CreateInstance<NestedClass> ();
		
		if (m_Class2 == null)
			
			m_Class2 = m_Class1;
		
		
		
		m_Class1.OnGUI();
		
		m_Class2.OnGUI();
		
	}
	
}*/