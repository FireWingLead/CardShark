using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardShark.PCShark.DAL.MTGAPI_Remote
{
	public class ItemNotFoundException : Exception
	{
		public ItemNotFoundException(string typeName, params KeyValuePair<string, string>[] searchTerms)
			: base("Could not find " + typeName + " with " + composeSTList(searchTerms) + ".") {
				SearchTermsUsed = searchTerms;
		}

		public IEnumerable<KeyValuePair<string, string>> SearchTermsUsed { get; private set; }

		private static string composeSTList(KeyValuePair<string, string>[] searchTerms) {
			int termCount = searchTerms.Length;
			if (termCount == 0) return "no properties to search by";
			StringBuilder list = new StringBuilder();
			int i = 1;
			foreach (KeyValuePair<string, string> term in searchTerms) {
				if (i != 1) {
					if (termCount > 2)
						list.Append(',');
					if (i < termCount)
						list.Append(" ");
					else
						list.Append(" and ");
				}
				list.Append(term.Key);
				list.Append('=');
				list.Append(term.Value);
				i++;
			}
			return list.ToString();
		}
	}
}
