using UnityEngine;
using CellUnity;
using CellUnity.Reaction;
using System.Collections;

using System;
using System.Threading;
using System.Security.Permissions;

public class ThreadWork {
	public static void DoWork() {
		try {
			for(int i=0; i<100; i++) {
				Debug.Log("Thread - working."); 
				Thread.Sleep(100);
			}
		}
		catch(Exception e) {
			Debug.Log("Thread - caught ThreadAbortException - resetting.");
			Debug.Log("Exception message: "+ e.Message);
			Thread.ResetAbort();
		}
		Debug.Log("Thread - still alive and working."); 
		Thread.Sleep(1000);
		Debug.Log("Thread - finished working.");
	}
}



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
			ThreadStart myThreadDelegate = new ThreadStart(ThreadWork.DoWork);
			Thread myThread = new Thread(myThreadDelegate);
			myThread.Start();
			Thread.Sleep(100);
			Debug.Log("Main - aborting my thread.");
			myThread.Abort();
			myThread.Join();
			Debug.Log("Main ending."); 
		}
		
		int i=1;
		foreach (var item in reactionTypes) {
			if(GUI.Button(new Rect(20,40 + 25*i,280,20), item.ToString())) {
				
			}
			
			i++;	
		}
		
	}
}
