using org.COPASI;
using CellUnity.Reaction;

namespace CellUnity.Simulation.Copasi
{
		public class CopasiReactionGroup
		{
				public CopasiReactionGroup (CReaction copasiReaction, ReactionType reactionType, CModelValue modelValueParticleFlux)
				{
					//this.copasiReaction = copasiReaction;
					this.reactionType = reactionType;
					this.modelValueParticleFlux = modelValueParticleFlux;
				}
				
				//private CReaction copasiReaction;
				private ReactionType reactionType;
				private CModelValue modelValueParticleFlux;
				
				private ulong processedParticleFlux = 0;
				
				public ReactionCount CalcParticleFlux()
				{
					ulong newFlux = (ulong)System.Math.Floor(modelValueParticleFlux.getValue());

					ulong delta =  newFlux - processedParticleFlux;
					processedParticleFlux = newFlux;
					
					return new ReactionCount(reactionType, delta);
				}
		}
}