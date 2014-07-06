using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CellUnity.Model.Pdb
{
	public class PdbImport {

		private static int molCounter = 1;
		public void UserSelectFile() {
			string molName = "mol" + (molCounter++).ToString ();
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
			
			GameObject mol = new GameObject(molName);
			Debug.Log ("About to add atoms...");

			int i = 0;
			foreach (Atom atom in m.Atoms) {
				i++;
				EditorUtility.DisplayProgressBar ("Creating Molecule...", "Atom "+i+"/"+m.Atoms.Length, i*1f/m.Atoms.Length);

				AddAtom(
					atom,
					mol.transform
					);
			}

			EditorUtility.DisplayProgressBar ("Creating Molecule...", "Creating Species...", 0.3f);

			CUE cue = CUE.GetInstance ();
			MoleculeSpecies species = cue.CreateMoleculeSpecies ();
			species.Name = molName;
			cue.AddSpecies (species);

			EditorUtility.DisplayProgressBar ("Creating Molecule...", "Creating Species...", 0.5f);

			mol.AddComponent<CellUnity.Molecule>();
			CellUnity.Molecule script = mol.GetComponent<CellUnity.Molecule> ();
			script.Species = species;
			
			EditorUtility.DisplayProgressBar ("Creating Molecule...", "Creating Prefab...", 0.1f);
			
			Object prefab = PrefabUtility .CreateEmptyPrefab("Assets/Molecules/"+molName+".prefab");
			EditorUtility.DisplayProgressBar ("Creating Molecule...", "Creating Prefab...", 0.2f);
			PrefabUtility.ReplacePrefab(mol, prefab);
			EditorUtility.DisplayProgressBar ("Creating Molecule...", "Refresh...", 0.9f);
			AssetDatabase.Refresh();

			species.Prefab = prefab;

			EditorUtility.DisplayProgressBar ("Creating Molecule...", "Done", 1f);
			Debug.Log ("done");
			EditorUtility.ClearProgressBar ();
		}
		
		static void AddAtom(Atom atom, Transform transform)
		{
			var sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			sphere.transform.parent = transform;
			
			float scale = atom.Element.Radius * 4;
			sphere.transform.localScale = new Vector3 (scale, scale, scale);
			sphere.transform.position = new Vector3 (atom.X, atom.Y, atom.Z);
			Material m = Resources.Load<Material> ("Atoms/Material" + atom.Element.Symbol);
			if (m != null)
			{ sphere.renderer.material = m; }
		}
	}
}