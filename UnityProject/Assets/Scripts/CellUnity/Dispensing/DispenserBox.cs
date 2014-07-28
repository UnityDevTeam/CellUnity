using UnityEngine;
using System.Collections.Generic;

namespace CellUnity.Dispensing
{
	public class DispenserBox
	{
		public DispenserBox(Vector3 location, float size)
			: this(null, location, size)
		{ }
		
		private DispenserBox(DispenserBox parent, Vector3 relativeLocation)
			: this(parent, relativeLocation, parent.SubSize)
		{ }
		
		private DispenserBox(DispenserBox parent, Vector3 relativeLocation, float size)
		{
			this.RelativeLocation = relativeLocation;
			this.parent = parent;
			
			this.Size = size;
			this.AvailableSize = size;
			
			this.Occupation = BoxOccupation.Vacant;
		}
		
		private DispenserBox parent;
		public float Size { get; private set; }
		public float SubSize { get { return Size / 2f; } }
		public float AvailableSize { get; private set; }
		public BoxOccupation Occupation { get; private set; }
		public Vector3 RelativeLocation { get; private set; }
		
		private DispenserBox[] subBoxes;
		
		public DispenserBox[] SubBoxes
		{
			get
			{
				if (subBoxes == null)
				{
					float a = SubSize / 2f;
				
					subBoxes = new DispenserBox[] { 
						new DispenserBox(this, new Vector3(a,a,a)),
						new DispenserBox(this, new Vector3(-a,a,a)),
						new DispenserBox(this, new Vector3(a,-a,a)),
						new DispenserBox(this, new Vector3(-a,-a,a)),
						
						new DispenserBox(this, new Vector3(a,a,-a)),
						new DispenserBox(this, new Vector3(-a,a,-a)),
						new DispenserBox(this, new Vector3(a,-a,-a)),
						new DispenserBox(this, new Vector3(-a,-a,-a))
					};
				}
				
				return subBoxes;
			}
		}
		
		public void Occupy()
		{
			if (Occupation != BoxOccupation.Vacant)
			{ throw new System.Exception("Box is not vacant"); }
			
			AvailableSize = 0;
			
			Occupation = BoxOccupation.Occupied;
			
			if (parent != null)
			{ parent.OccupySubBox(); }
		}
		
		public bool FindSpace(float size, float minimumBoxSize, System.Random random, out Vector3 location)
		{
			if (AvailableSize >= size)
			{
				// ...so there is space left
				
				// Check if small enough to fit in a SubBox and if the SubBox isn't smaller than the minimumBox
				if (SubSize >= size && SubSize >= minimumBoxSize)
				{
					// let's put it into a Sub Box
					List<DispenserBox> availableSubBoxes = new List<DispenserBox>(SubBoxes.Length);
					availableSubBoxes.AddRange(SubBoxes);
					
					bool spaceFound = false;
					do
					{
						int i = random.Next(0, availableSubBoxes.Count);
						spaceFound = availableSubBoxes[i].FindSpace(size, minimumBoxSize, random, out location);
						if (spaceFound)
						{
							return true;
						}
						else
						{
							availableSubBoxes.RemoveAt(i);
						}
					}
					while (availableSubBoxes.Count > 0);
					
					throw new System.Exception("No Space left in SubBoxes."); // This should not be possible.
				}
				else
				{
					// have to occupy this whole box
					Occupy();
					
					// randomize Location inside of this box
					float freeSpace = (this.Size - size);
					
					Vector3 boxCenter = GetLocation();
					location = new Vector3(
						boxCenter.x + (float)(freeSpace*(random.NextDouble() - 0.5)),
						boxCenter.y + (float)(freeSpace*(random.NextDouble() - 0.5)),
						boxCenter.z + (float)(freeSpace*(random.NextDouble() - 0.5))
					);					
					
					return true;
				}
			}
			else
			{
				// no space left
				location = Vector3.zero;
				return false;
			}
		}
		
		public Vector3 GetLocation()
		{
			if (parent == null)
			{
				return 
					RelativeLocation;
			}
			else
			{
				return
					parent.GetLocation() +
					RelativeLocation;
			}
		}
		
		private void OccupySubBox()
		{
			bool allSubBoxesOccupied = true;
			
			AvailableSize = 0;
			
			for (int i = 0; i < subBoxes.Length; i++)
			{
				DispenserBox subBox = subBoxes[i];
				
				allSubBoxesOccupied &= (subBox.Occupation == BoxOccupation.Occupied);
				AvailableSize = System.Math.Max(AvailableSize, subBox.AvailableSize);
			}
			
			if (allSubBoxesOccupied)
			{ Occupation = BoxOccupation.Occupied; }
			else
			{ Occupation = BoxOccupation.Partly; }
			
			if (parent != null)
			{ parent.OccupySubBox(); }
		}
	}
	
	public enum BoxOccupation { Vacant, Partly, Occupied }
}