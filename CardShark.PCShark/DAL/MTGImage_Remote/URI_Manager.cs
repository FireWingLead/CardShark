using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CardShark.PCShark.DAL.MTGImage_Remote
{
	public class URI_Manager
	{
		private const string DEF_DOMAIN_URI = "http://mtgimage.com/";
		private const string DEF_MANA_URI = "symbol/mana/";
		private const string DEF_MANA_SIZE = "64";
		private const string DEF_MANA_FMT = "png";

		private static URI_Manager defManager = new URI_Manager();
		public static URI_Manager Default { get { return defManager; } set { if (value != null) defManager = value; } }

		private string domainURI = DEF_DOMAIN_URI;
		public string DomainURI { get { return domainURI; } set { if (!string.IsNullOrWhiteSpace(value)) domainURI = value; } }

		public string GetManaSymbolURI(string symbolItemCode) {
			symbolItemCode = symbolItemCode.Replace("_", string.Empty);
			return DomainURI + DEF_MANA_URI + symbolItemCode + "/" + DEF_MANA_SIZE + "." + DEF_MANA_FMT;
		}
	}
}
