using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Globalization;

namespace CellUnity.Model.Pdb
{
	/// <summary>
	/// Parser for reading PDB files
	/// </summary>
	public class PdbParser : IDisposable
	{
		/// <summary>
		/// Private constructor.
		/// Initializes a new instance of the <see cref="CellUnity.Model.Pdb.PdbParser"/> class.
		/// </summary>
		/// <param name="source">Source.</param>
		private PdbParser(string source)
		{
			this.source = source;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Model.Pdb.PdbParser"/> class.
		/// The PDB data is aquired from the specified filename
		/// </summary>
		/// <returns>new PDB Parser instance.</returns>
		/// <param name="filename">Filename of the PDB file.</param>
		public static PdbParser FromFile(string filename) {
			
			PdbParser p = new PdbParser ("file://" + filename);
			p.data = File.ReadAllLines(filename);
			return p;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Model.Pdb.PdbParser"/> class.
		/// The PDB data is aquired from the data string.
		/// </summary>
		/// <returns>new PDB Parser instance.</returns>
		/// <param name="source">Source of the definition. e.g. URL or Filename of the PDB file.</param>
		/// <param name="data">PDB data.</param>
		/// <param name="name">Name of the species.</param>
		public static PdbParser FromString(string source, string data, string name) {
			
			PdbParser p = new PdbParser (source);
			p.data = data.Replace("\r","").Split('\n');
			p.name = name;
			return p;
		}

		/// <summary>
		/// source of the definition. e.g. URL or Filename of the PDB file
		/// </summary>
		private string source;
		/// <summary>
		/// name of the molecule
		/// </summary>
		private string name;
		/// <summary>
		/// PDB data. Each string in this array defines a line in the PDB file
		/// </summary>
		private string[] data;
		/// <summary>
		/// Atoms read out of the PDB file with Atom serial as key.
		/// </summary>
		private Dictionary<int, Atom> serialAtom = new Dictionary<int, Atom>();
		/// <summary>
		/// The bonds read out of the PDB file
		/// </summary>
		private List<Bond> bonds = new List<Bond>();

		/// <summary>
		/// Parses the defined PDB data and returns a molecule object that
		/// represents the data of the molecule.
		/// The instance is automatically disposed afer parsing.
		/// </summary>
		public Molecule Parse()
		{
			// Get name from source if no name defined
			if (string.IsNullOrEmpty (name)) {
				name = Path.ChangeExtension (Path.GetFileName (source), "").TrimEnd (new char[]{'.',' '});
			}

			// parse lines
			foreach (string line in data)
			{
				// char 1-6 defines the reacord type of this line
				string recordName = ReadColumnString(line, 1, 6);
				
				if (recordName == "ATOM") { ReadAtom(line); }
				else if (recordName == "HETATM") { ReadAtom(line); }
				else if (recordName == "CONECT") { ReadBond(line); }
			}

			// Copy atoms to array
			Atom[] atomArray = new Atom[serialAtom.Count];
			serialAtom.Values.CopyTo (atomArray, 0);

			// create molecule object
			Molecule molecule = new Molecule(
				name,
				source,
				atomArray,
				bonds.ToArray()
				);
			
			Dispose();
			
			return molecule;
		}

		/// <summary>
		/// Creates an atom out an ATOM or HETATM line, and adds it to 
		/// the serialAtom dictionary.
		/// </summary>
		/// <param name="line">Line.</param>
		private void ReadAtom(string line)
		{
			int serial = ReadColumnInt(line, 7, 11);
			float x = AngstromToNm(ReadColumnFloat(line, 31, 38));
			float y = AngstromToNm(ReadColumnFloat(line, 39, 46));
			float z = AngstromToNm(ReadColumnFloat(line, 47, 54));
			string element = ReadColumnString(line, 77, 78);
			
			Atom atom = new Atom(
				Element.BySymbol(element),
				x,
				y,
				z
				);
			
			serialAtom.Add(serial, atom);
		}

		/// <summary>
		/// Convert Angstrom To Nm
		/// </summary>
		/// <returns>nm</returns>
		/// <param name="f">Angstroms</param>
		private float AngstromToNm(float f)
		{
			return f * 0.1f;
		}

		/// <summary>
		/// Creates a bond out of a CONECT line.
		/// </summary>
		/// <param name="line">Line.</param>
		private void ReadBond(string line)
		{
			Atom atom = AtomBySerial(ReadColumnInt(line, 7, 11));
			
			for (int i = 12; i < line.Length; i += 5)
			{
				int bondSerial;
				if (TryReadColumnInt(line, i, i + 4, out bondSerial))
				{
					Atom bondedAtom = AtomBySerial(bondSerial);
					Bond bond = new Bond(
						atom,
						bondedAtom
						);
					
					bonds.Add(bond);
				}
				else { break; }
			}
		}

		/// <summary>
		/// Releases the PDB data.
		/// </summary>
		public void Dispose()
		{
			data = null;
			serialAtom = null;
			bonds = null;
		}

		/// <summary>
		/// Gets an Atom by its seril number
		/// </summary>
		/// <returns>Atom instance</returns>
		/// <param name="serial">Serial.</param>
		private Atom AtomBySerial(int serial)
		{
			return serialAtom[serial];
		}

		/// <summary>
		/// Reads a substring by a defined start char and a defined end char.
		/// The first char has the index 1.
		/// </summary>
		/// <returns>The defined part of the string</returns>
		/// <param name="line">Line.</param>
		/// <param name="startChar">Index of start char.</param>
		/// <param name="endChar">Index of end char.</param>
		private string ReadColumnString(string line, int startChar, int endChar)
		{
			int i = startChar - 1;
			int c = endChar - startChar + 1;
			if (i + c >= line.Length) { c = line.Length - i; }
			return line.Substring(i, c).Trim();
		}

		/// <summary>
		/// Same as ReadColumnString, but the result is converted to float.
		/// </summary>
		private float ReadColumnFloat(string line, int startChar, int endChar)
		{
			string v = ReadColumnString(line, startChar, endChar);
			return float.Parse(v, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Same as ReadColumnString, but the result is converted to int.
		/// </summary>
		private int ReadColumnInt(string line, int startChar, int endChar)
		{
			string v = ReadColumnString(line, startChar, endChar);
			return int.Parse(v, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Same as ReadColumnString, but the result is converted to int.
		/// If the value string at this position is empty, false is returned.
		/// </summary>
		/// <returns><c>true</c>, if read substring is not empty, <c>false</c> otherwise.</returns>
		/// <param name="line">Line.</param>
		/// <param name="startChar">Start char.</param>
		/// <param name="endChar">End char.</param>
		/// <param name="value">Value.</param>
		private bool TryReadColumnInt(string line, int startChar, int endChar, out int value)
		{
			string v = ReadColumnString(line, startChar, endChar);
			if (string.IsNullOrEmpty(v))
			{
				value = 0;
				return false;
			}
			else
			{
				value = int.Parse(v, CultureInfo.InvariantCulture);
				return true;
			}
		}
	}
}