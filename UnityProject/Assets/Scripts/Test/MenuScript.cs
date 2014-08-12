using UnityEngine;
using CellUnity;
using CellUnity.Reaction;
using System.Collections;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI () {
		
		CUE cue = CUE.GetInstance();
		ReactionType[] reactionTypes = cue.ReactionTypes;
		
		// Make a background box
		GUI.Box(new Rect(10,10,300,30 + reactionTypes.Length * 25), "Reaction For Molecule");
		
		if(GUI.Button(new Rect(20,40,280,20), "Attach")) {
			
		}
		
		int i=1;
		foreach (var item in reactionTypes) {
			if(GUI.Button(new Rect(20,40 + 25*i,280,20), item.ToString())) {
				
			}
			
			i++;	
		}
		
	}
}
