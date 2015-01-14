using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeJSON
{
	public class JSONSyntaxException : Exception
	{
		public JSONSyntaxException(string expectedItemDesc, string foundItemDesc, string foundVal, long line, long chr)
			: base("JSON syntax error during parse. Expected " + expectedItemDesc + ". Found " + foundItemDesc + (foundVal == null ? "" : "\"" + foundVal + "\" ") + "instead on line#" + line.ToString() + ", character#" + chr.ToString() + ".") {
				Line = line;
				Char = chr;
		}
		public long Line { get; private set; }
		public long Char { get; private set; }
	}
}
