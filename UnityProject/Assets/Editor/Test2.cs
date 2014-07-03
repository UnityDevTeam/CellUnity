using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyWindow : EditorWindow
{
	[SerializeField]
	private SerializeMe2 m_SerialziedThing;

	[MenuItem ("Window/Serialization")]
	static void Init () {
		GetWindow <MyWindow>();
	}
	
	void OnEnable ()
	{
		hideFlags = HideFlags.HideAndDontSave;
		if (m_SerialziedThing == null)
			m_SerialziedThing = ScriptableObject.CreateInstance<SerializeMe2>();
	}
	
	void OnGUI () {
		GUILayout.Label ("Serialized Things", EditorStyles.boldLabel);
		m_SerialziedThing.OnGUI ();
	}
}

[Serializable]
public class MyBaseClass : ScriptableObject
{
	[SerializeField]
	protected int m_IntField;
	
	public void OnEnable() { hideFlags = HideFlags.HideAndDontSave; }
	
	public virtual void OnGUI ()
	{
		m_IntField = EditorGUILayout.IntSlider("IntField", m_IntField, 0, 10);
	}
}

[Serializable]
public class ChildClass : MyBaseClass
{
	[SerializeField]
	private float m_FloatField;
	
	public override void OnGUI()
	{
		base.OnGUI ();
		m_FloatField = EditorGUILayout.Slider("FloatField", m_FloatField, 0f, 10f);
	}
}

[Serializable]
public class SerializeMe2 : ScriptableObject
{
	[SerializeField]
	private List<MyBaseClass> m_Instances;
	
	public void OnEnable ()
	{
		if (m_Instances == null)
			m_Instances = new List<MyBaseClass>();
		
		hideFlags = HideFlags.HideAndDontSave;
	}
	
	public void OnGUI ()
	{
		foreach (var instance in m_Instances)
			instance.OnGUI ();
		
		if (GUILayout.Button ("Add Base"))
			m_Instances.Add(CreateInstance<MyBaseClass>());
		if (GUILayout.Button ("Add Child"))
			m_Instances.Add(CreateInstance<ChildClass>());
	}
}