
using System;
using UnityEditor;
using UnityEngine;
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