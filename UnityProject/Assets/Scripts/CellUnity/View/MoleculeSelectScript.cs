using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellUnity.Reaction;
using CellUnity.Utility;

namespace CellUnity.View
{
	/// <summary>
	/// Script that shows an info menu about the selected molecule and lets
	/// the user select the next reaction
	/// </summary>
	public class MoleculeSelectScript : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		private bool following = false;
		private Quaternion followMoleculeRotation;
		
		void OnGUI () {
			
			CUE cue = CUE.GetInstance();

			// Get selected molecule
			Molecule m = cue.ReactionManager.SelectedMolecule;

			// Set following
			if (following)
			{
				CameraControl.Follow = m.gameObject;
			}
			else
			{
				CameraControl.Follow = null;
			}

			// Draw menu or remove script if no moleule is selected
			if (m == null)
			{
				RemoveScript();
			}
			else
			{
				GuiBoxReset();

				GuiBeginBox("Molecule", 94);

				string reaction;
				string reactionStatus;

				if (m.ReactionPrep != null)
				{ 
					reaction = m.ReactionPrep.ReactionType.ToString(); 
					reactionStatus = "performing";
				}
				else
				{
					ReactionType selectedReaction = cue.ReactionManager.SelectedReaction;
					if (selectedReaction != null)
					{
						reaction = selectedReaction.ToString(); 
						reactionStatus = "waiting";
					}
					else
					{
						reaction = "none"; 
						reactionStatus = "";
					}
				}

				GuiCaptionValue("Species:", m.Species.Name);
				GuiCaptionValue("Reaction:", reaction + "\n\r" + reactionStatus);

				GUILayout.BeginHorizontal();

				if (GUILayout.Button(following ? "Stay" : "Follow"))
			    {
					following = !following;
				}

				if (GUILayout.Button("Deselect"))
				{
					Deselect();
				}

				if (GUILayout.Button("Hide"))
				{
					Hide ();
				}
				
				GUILayout.EndHorizontal();

				GuiEndBox();

				//
				// Reactions
				//

				List<ReactionType> reactions = new List<ReactionType>();
				foreach (var r in cue.ReactionTypes) {
					if (Utils.ArrayContains<MoleculeSpecies>(r.Reagents, m.Species))
					{
						reactions.Add(r);
					}
				}

				GuiBeginBox("Reaction", (reactions.Count + 1) * (guiReactionHeight) + guiReactionSpace);

				if (GUILayout.Button("none"))
				{
					cue.ReactionManager.SelectedReaction = null;
				}

				GUILayout.Space(guiReactionSpace);
				
				foreach (var r in reactions) {
					if (GUILayout.Button(r.ToString()))
					{
						cue.ReactionManager.SelectedReaction = r;
					}
				}

				GuiEndBox();
				
				if (Input.GetKey("escape"))
				{
					Deselect();
				}
				if (Input.GetKey("return"))
				{
					Hide ();
				} 
			}
		}

		private void Deselect()
		{
			CameraControl.Follow = null;
			CUE cue = CUE.GetInstance ();
			cue.ReactionManager.SelectedMolecule = null;
			
			RemoveScript();
		}

		private void Hide()
		{
			RemoveScript();
		}

		private void GuiCaptionValue(string caption, string value)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(caption);
			GUILayout.Label(value);
			GUILayout.EndHorizontal();
		}

		private int guiBoxY;
		private const int guiBoxSpace = 10;
		private const int guiBoxBorder = 7;
		private const int guiBoxHeader = 24;
		private const int guiBoxWidth = 300;

		private const int guiReactionHeight = 22;
		private const int guiReactionSpace = 5;

		private void GuiBoxReset () { guiBoxY = 0; }

		private void GuiBeginBox(string text, int h)
		{
			guiBoxY += guiBoxSpace;

			int totalH = h + 2 * guiBoxBorder + guiBoxHeader;

			Rect boxRect = new Rect (guiBoxSpace, guiBoxY, guiBoxWidth, totalH);
			GUI.Box (boxRect, "");

			GUI.Box (new Rect(boxRect.x, boxRect.y, boxRect.width, guiBoxHeader), text);

			Rect areaRect = new Rect (boxRect.x + guiBoxBorder, guiBoxY + guiBoxBorder + guiBoxHeader, guiBoxWidth - 2*guiBoxBorder, h);
			GUILayout.BeginArea (areaRect);

			guiBoxY += totalH;
		}

		private void GuiEndBox()
		{
			GUILayout.EndArea();
		}
			
			private void RemoveScript()
		{
			//cue.ScriptManager.RemoveScript<MoleculeSelectScript>();
			enabled = false;
		}
	}
}
