using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CellUnity.Model.Pdb
{
	public class PdbImport {

		//private static int molCounter = 1;
		public void UserSelectFile() {
			//string molName = "mol" + (molCounter++).ToString ();
			string filename = EditorUtility.OpenFilePanel ("PDB File", "", "pdb");
			
			PdbParser p = PdbParser.FromFile (filename);
			Molecule m = p.Parse();
			Create (m);
		}

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

		private void Create(Molecule m) {

			string molName = m.Name;
			Debug.Log ("Imported: " + m.Atoms.Length + "atoms / " + m.Bonds.Length + "bonds");
			
			Debug.Log ("About to add atoms...");

			int i = 0;
			float mass = 0;
			GameObject[] gameObjects = new GameObject[m.Atoms.Length];

			foreach (Atom atom in m.Atoms) {
				EditorUtility.DisplayProgressBar ("Creating Molecule...", "Atom "+(i+1)+"/"+m.Atoms.Length, (i+1)*1f/m.Atoms.Length);

				gameObjects[i] = AddAtom(atom);
				
				mass += atom.Element.Mass;

				i++;
			}

			EditorUtility.DisplayProgressBar ("Creating Molecule...", "Creating Prefab...", 0.2f);

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
		
		static GameObject AddAtom(Atom atom)
		{
			var sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);

			float scale = atom.Element.Radius * 4;
			sphere.transform.localScale = new Vector3 (scale, scale, scale);
			sphere.transform.position = new Vector3 (atom.X, atom.Y, atom.Z);
			Material m = Resources.Load<Material> ("Atoms/Material" + atom.Element.Symbol);
			if (m != null)
			{ sphere.renderer.material = m; }
			sphere.name = atom.Element.Symbol;

			return sphere;
		}
	}
}