using UnityEngine;
using System.Collections;
using CellUnity.Reaction;

namespace CellUnity.Simulation
{
	/// <summary>
	/// Reports the number of reactions occurded
	/// </summary>
	public class ReactionCount
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Simulation.ReactionCount"/> class.
		/// </summary>
		/// <param name="reactionType">Reaction type.</param>
		/// <param name="count">Number of reactions.</param>
		public ReactionCount(ReactionType reactionType, ulong count)
		{
			this.ReactionType = reactionType;
			this.Count = count;
		}

		/// <summary>
		/// Gets the type of the reaction.
		/// </summary>
		/// <value>The type of the reaction.</value>
		public ReactionType ReactionType { get; private set; }

		/// <summary>
		/// Gets the count of reactions occured.
		/// </summary>
		/// <value>The count.</value>
		public ulong Count { get; private set; }
	}
}