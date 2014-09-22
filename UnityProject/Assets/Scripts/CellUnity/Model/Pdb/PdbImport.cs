using UnityEngine;
using UnityEditor;
using System.Collections;
using CellUnity.Utility;

namespace CellUnity.Model.Pdb
{
	/// <summary>
	/// Class that helps with PDB import
	/// </summary>
	public class PdbImport {

		/// <summary>
		/// Shows an OpenFilePanel and lets the user select a pdb file.
		/// </summary>
		public void UserSelectFile() {

			string filename = EditorUtility.OpenFilePanel ("PDB File", "", "pdb");

			if (!string.IsNullOrEmpty(filename))
			{
				PdbParser p = PdbParser.FromFile (filename);
				Molecule m = p.Parse();
				Create (m);
			}
		}

		/// <summary>
		/// Downloads a PDB file from a specified URL.
		/// Shows a Message Dialog when an error occurs.
		/// </summary>
		/// <returns><c>true</c>, if molecule was imported properly, <c>false</c> otherwise.</returns>
		/// <param name="url">URL.</param>
		/// <param name="name">Name.</param>
		public bool DownloadFile(string url, string name) {
			WWW www = new WWW(url);
			
			while( !www.isDone ) {
				EditorUtility.DisplayProgressBar ("Download", "Downloading...", www.progress);
			}
			
			EditorUtility.ClearProgressBar ();
			
			if (!string.IsNullOrEmpty(www.error)) {
				
				EditorUtility.DisplayDialog("Error", www.error, "Close");
				return false;
				
			} else {

				PdbParser p = PdbParser.FromString(url, www.text, name);
				Molecule m = p.Parse();
				Create(m);

				return true;
			}
		}

		/// <summary>
		/// Create a CellUnity molecule species from a PDB molecule definition
		/// </summary>
		/// <param name="m">PDB molecule definition</param>
		private void Create(Molecule m) {

			string molName = m.Name;
			Debug.Log ("Imported: " + m.Atoms.Length + "atoms / " + m.Bonds.Length + "bonds");
			
			Debug.Log ("About to add atoms...");

			// GameObjects of the atoms
			GameObject[] gameObjects = new GameObject[m.Atoms.Length];

			// atom index
			int i = 0;
			
			// sum of the atom mass
			float mass = 0;

			foreach (Atom atom in m.Atoms) {
				EditorUtility.DisplayProgressBar ("Creating Molecule...", "Atom "+(i+1)+"/"+m.Atoms.Length, (i+1)*1f/m.Atoms.Length);

				gameObjects[i] = AddAtom(atom);
				
				mass += atom.Element.Mass;

				i++;
			}

			EditorUtility.DisplayProgressBar ("Creating Molecule...", "Creating Prefab...", 0.2f);

			// Create species out of atom game objects
			MoleculeCreator creator = new MoleculeCreator ();
			creator.gameObjects = gameObjects;
			creator.name = molName;
			creator.mass = mass;
			creator.Create ();

			EditorUtility.DisplayProgressBar ("Creating Molecule...", "Refresh...", 0.9f);

			EditorUtility.DisplayProgressBar ("Creating Molecule...", "Done", 1f);
			Debug.Log ("done");
			EditorUtility.ClearProgressBar ();
		}

		/// <summary>
		/// Creates a GameObject for an atom.
		/// </summary>
		/// <returns>The created GameObject</returns>
		/// <param name="atom">Atom.</param>
		static GameObject AddAtom(Atom atom)
		{
			// create, resize and locate GameObject
			var sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			float scale = 2 * Utils.ScaleFromNm(atom.Element.VdWRadius); // *2 because scale describes indirectly the diameter
			sphere.transform.localScale = new Vector3 (scale, scale, scale);
			sphere.transform.position = new Vector3 (Utils.ScaleFromNm(atom.X), Utils.ScaleFromNm(atom.Y), Utils.ScaleFromNm(atom.Z));

			// find Material for Element
			Material m = Resources.Load<Material> ("Atoms/Material" + atom.Element.Symbol);
			if (m != null)
			{ sphere.renderer.material = m; }
			sphere.name = atom.Element.Symbol;

			// return GameObject
			return sphere;
		}
	}
}