using System;
using System.IO;

public static class FLog
{
	public static void I (string msg)
	{
		string line = DateTime.Now.ToLongTimeString () + ": " + msg + Environment.NewLine;

		File.AppendAllText (@"c:\temp\flog.txt", line);
	}
}


