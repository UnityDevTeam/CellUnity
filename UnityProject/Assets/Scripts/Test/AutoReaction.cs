using UnityEngine;
using System.Collections;
using CellUnity;

public class AutoReaction : MonoBehaviour {

	// Use this for initialization
	void Start () {
		cue = CUE.GetInstance();
		Debug.Log("Auto Reaction started");
	}
	
	private CUE cue;
	
	// Update is called once per frame
	void Update () {
		foreach (var reaction in cue.ReactionTypes) {
			try
			{
				cue.ReactionManager.PerformReaction(reaction);	
			}
			catch { }
		}
	}
}
