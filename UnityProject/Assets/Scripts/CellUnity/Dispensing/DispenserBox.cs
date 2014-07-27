using UnityEngine;
using System.Collections.Generic;

namespace CellUnity.Model.Dispensing
{
	public class DispenserBox
	{
		public DispenserBox(BoxLocation location, float size)
			: this(null, location, size)
		{ }
		
		private DispenserBox(DispenserBox parent, BoxLocation location)
			: this(parent, location, parent.SubSize)
		{ }
		
		private DispenserBox(DispenserBox parent, BoxLocation location, float size)
		{
			this.Location = location;
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
		public BoxLocation Location { get; private set; }
		
		private DispenserBox[] subBoxes;
		
		public DispenserBox[] SubBoxes
		{
			get
			{
				if (subBoxes == null)
				{
					subBoxes = new DispenserBox[] { 
						new DispenserBox(this, new BoxLocation(1,1,1)),
						new DispenserBox(this, new BoxLocation(-1,1,1)),
						new DispenserBox(this, new BoxLocation(1,-1,1)),
						new DispenserBox(this, new BoxLocation(-1,-1,1)),
						
						new DispenserBox(this, new BoxLocation(1,1,-1)),
						new DispenserBox(this, new BoxLocation(-1,1,-1)),
						new DispenserBox(this, new BoxLocation(1,-1,-1)),
						new DispenserBox(this, new BoxLocation(-1,-1,-1))
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
					
					location = GetLocation();
					
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
		
		private Vector3 GetLocation()
		{
			if (parent == null)
			{
				return 
					new Vector3(
						Size * Location.X,
						Size * Location.Y,
						Size * Location.Z
						);
			}
			else
			{
				return
					parent.GetLocation() +
						new Vector3(
							Size * Location.X,
							Size * Location.Y,
							Size * Location.Z
							);
			}
		}
		
		public int GetDepth()
		{
			if (parent == null)
			{ return 0; }
			else
			{ return parent.GetDepth() + 1; }
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