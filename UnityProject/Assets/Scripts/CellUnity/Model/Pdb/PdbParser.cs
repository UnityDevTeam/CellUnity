using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Globalization;

namespace CellUnity.Model.Pdb
{
	public class PdbParser : IDisposable
	{
		private PdbParser(string source)
		{
			this.source = source;
		}

		public static PdbParser FromFile(string filename) {
			
			PdbParser p = new PdbParser ("file://" + filename);
			p.data = File.ReadAllLines(filename);
			return p;
		}

		public static PdbParser FromString(string source, string data, string name) {
			
			PdbParser p = new PdbParser (source);
			p.data = data.Replace("\r","").Split('\n');
			p.name = name;
			return p;
		}
		
		private string source;
		private string name;
		private string[] data;
		private Dictionary<int, Atom> serialAtom = new Dictionary<int, Atom>();
		private List<Bond> bonds = new List<Bond>();
		
		public Molecule Parse()
		{
			if (string.IsNullOrEmpty (name)) {
				name = Path.ChangeExtension (Path.GetFileName (source), "").TrimEnd (new char[]{'.',' '});
			}

			foreach (string line in data)
			{
				string recordName = ReadColumnString(line, 1, 6);
				
				if (recordName == "ATOM") { ReadAtom(line); }
				else if (recordName == "HETATM") { ReadAtom(line); }
				else if (recordName == "CONECT") { ReadBond(line); }
			}

			Atom[] atomArray = new Atom[serialAtom.Count];
			serialAtom.Values.CopyTo (atomArray, 0);

			Molecule molecule = new Molecule(
				name,
				source,
				atomArray,
				bonds.ToArray()
				);
			
			Dispose();
			
			return molecule;
		}
		
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

		private float AngstromToNm(float f)
		{
			return f * 0.1f;
		}
		
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
		
		public void Dispose()
		{
			data = null;
			serialAtom = null;
			bonds = null;
		}
		
		private Atom AtomBySerial(int serial)
		{
			return serialAtom[serial];
		}
		
		private string ReadColumnString(string line, int startChar, int endChar)
		{
			int i = startChar - 1;
			int c = endChar - startChar + 1;
			if (i + c >= line.Length) { c = line.Length - i; }
			return line.Substring(i, c).Trim();
		}
		
		private float ReadColumnFloat(string line, int startChar, int endChar)
		{
			string v = ReadColumnString(line, startChar, endChar);
			return float.Parse(v, CultureInfo.InvariantCulture);
		}
		
		private int ReadColumnInt(string line, int startChar, int endChar)
		{
			string v = ReadColumnString(line, startChar, endChar);
			return int.Parse(v, CultureInfo.InvariantCulture);
		}
		
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