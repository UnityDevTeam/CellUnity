using org.COPASI;
using CellUnity.Reaction;

namespace CellUnity.Simulation.Copasi
{
		/// <summary>
		/// Combines COPASI reaction with ReactionType and the model value for ParticleFlux in one object
		/// </summary>
		public class CopasiReactionGroup
		{
				/// <summary>
				/// Initializes a new instance of the <see cref="CellUnity.Simulation.Copasi.CopasiReactionGroup"/> class.
				/// </summary>
				/// <param name="copasiReaction">Copasi reaction.</param>
				/// <param name="reactionType">Reaction type.</param>
				/// <param name="modelValueParticleFlux">Model value particle flux.</param>
				public CopasiReactionGroup (CReaction copasiReaction, ReactionType reactionType, CModelValue modelValueParticleFlux)
				{
					//this.copasiReaction = copasiReaction;
					this.reactionType = reactionType;
					this.modelValueParticleFlux = modelValueParticleFlux;
				}
				
				//private CReaction copasiReaction;
				private ReactionType reactionType;
				/// <summary>
				/// Gets the type of the reaction.
				/// </summary>
				/// <value>The type of the reaction.</value>
				public ReactionType ReactionType { get{ return reactionType; } }

				private CModelValue modelValueParticleFlux;
				
				private ulong processedParticleFlux = 0;
				
				/// <summary>
				/// Calculates the particle flux since the last call of this function
				/// </summary>
				/// <returns>ReactionCount instance containing the number of reactions simulated since the last call</returns>
				public ReactionCount CalcParticleFlux()
				{
					ulong newFlux = (ulong)System.Math.Floor(modelValueParticleFlux.getValue());

					ulong delta =  newFlux - processedParticleFlux;
					processedParticleFlux = newFlux;
					
					return new ReactionCount(reactionType, delta);
				}
		}
}