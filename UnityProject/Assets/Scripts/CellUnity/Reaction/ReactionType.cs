using UnityEngine;
using System.Collections;
using CellUnity.Utility;

namespace CellUnity.Reaction
{
	[System.Serializable]
	public class ReactionType : ScriptableObject {

		public string Name = "";

		void OnEnable ()
		{
			hideFlags = HideFlags.HideInHierarchy;
		}

		public override string ToString ()
		{
			return Name;
		}

		public override int GetHashCode ()
		{
			return GetInstanceID ();
		}

		public override bool Equals (object o)
		{
			ReactionType other;
			if (Utils.TypeEquals<ReactionType> (this, o, out other)) {

				return 
					(GetInstanceID() == other.GetInstanceID()) &&
					(Name == other.Name);

			} else { return false; }
		}
	}
}