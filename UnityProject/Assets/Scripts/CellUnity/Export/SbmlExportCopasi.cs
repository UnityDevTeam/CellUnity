using CellUnity.Simulation.Copasi;

namespace CellUnity.Export
{
	/// <summary>
	/// SBML Export Class that uses COPASI to generate the SBML file.
	/// </summary>
	public class SbmlExportCopasi
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Export.SbmlExportCopasi"/> class.
		/// </summary>
		/// <param name="cue">CUE to export</param>
		public SbmlExportCopasi (CUE cue)
		{
			this.cue = cue;
		}

		private CUE cue;

		/// <summary>
		/// Export the environment to the specified filename.
		/// </summary>
		/// <param name="filename">Filename</param>
		public void Export(string filename)
		{
			using (Copasi copasi = new Copasi())
			{
				// Copy the environment to COPASI
				// (Compartment, Species, Reactions)

				copasi.UpdateCompartmentVolume(cue.Volume);

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

				// Compile and Export
				copasi.CompileAndUpdate();
				copasi.ExportSbml(filename);
			}
		}
	}
}

