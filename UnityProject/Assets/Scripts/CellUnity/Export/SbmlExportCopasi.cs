using CellUnity.Simulation.Copasi;

namespace CellUnity.Export
{
	public class SbmlExportCopasi
	{
		public SbmlExportCopasi (CUE cue)
		{
			this.cue = cue;
		}

		private CUE cue;

		public void Export(string filename)
		{
			using (Copasi copasi = new Copasi())
			{
				foreach (var item in cue.Species) {
					copasi.AddSpecies(item);
					copasi.UpdateSpeciesQuantity(
						copasi.GetMetab(item),
						cue.Molecules.GetQuantity(item)
						);
				}

				foreach (var item in cue.ReactionTypes) {
					copasi.AddReaction(item);
				}

				copasi.CompileAndUpdate();

				copasi.ExportSbml(filename);
			}
		}
	}
}

