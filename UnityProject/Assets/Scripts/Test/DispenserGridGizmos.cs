using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Dispensing;
using CellUnity;

public class DispenserGridGizmos : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public int MinBoxCount;
	public bool ShowBoxes = true;
	public bool ShowSubBoxes = true;
	
	void OnDrawGizmos() {
	
		CUE cue = CUE.GetInstance();
				
		DispenserBoxGridCube x = new DispenserBoxGridCube();
		
		foreach (var species in cue.Species) {
			x.Size = Mathf.Max(species.Size, x.Size);
		}
		
		x.MinBoxCount = this.MinBoxCount;
		
		
		List<DispenserBox> boxes = x.Create();
		
		Color[] colors = new Color[]
		{
			new Color(1,0,0, 0.2f),
			new Color(0,1,0, 0.2f),
			new Color(0,0,1, 0.2f),
			new Color(1,1,0, 0.2f),
			new Color(0,1,1, 0.2f),
		};
		
		
		int i = 0;
		foreach (var item in boxes) {
			if (ShowBoxes)
			{
				Vector3 loc = item.GetLocation();
				Gizmos.color = colors[i++%colors.Length];
				Gizmos.DrawCube(loc, new Vector3(item.Size, item.Size, item.Size));
			}
			
			if (ShowSubBoxes)
			{
				foreach (var subBox in item.SubBoxes) {
					Gizmos.color = colors[i++%colors.Length];
					Gizmos.DrawCube(subBox.GetLocation(), new Vector3(subBox.Size, subBox.Size, subBox.Size));
				}
			}
		}
	}
}
