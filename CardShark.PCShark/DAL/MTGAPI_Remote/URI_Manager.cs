using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web;

namespace CardShark.PCShark.DAL.MTGAPI_Remote
{
	public class URI_Manager
	{
		private static readonly string[] SUPPORTED_VERSIONS = new string[] { "v2" };
		private static readonly string DEFAULT_VERSION = SUPPORTED_VERSIONS[0];
		private static readonly string DEFAULT_DOMAIN_URI = "http://api.mtgapi.com";

		private static URI_Manager defManager = new URI_Manager();
		public static URI_Manager Default { get { return defManager; } set { if (value != null) defManager = value; } }

		public static string AppConnectionString {
			get { try { return ConfigurationManager.ConnectionStrings["MTGAPI"].ConnectionString; } catch { return null; } }
		}

		public URI_Manager() : this(AppConnectionString) {

		}
		public URI_Manager(string conStr) {
			ConnectionString = conStr;
		}

		private string connectionString;
		public string ConnectionString {
			get { return connectionString; }
			set {
				string[] parts = value.Split(';');
				foreach (string part in parts) {
					string _part = part.Trim();
					if (string.IsNullOrEmpty(_part))
						continue;
					string[] halfs = _part.Split(new char[] { '=' }, 2);
					if (halfs.Length < 2)
						continue;
					halfs[0] = halfs[0].Trim().ToLower();
					halfs[1] = halfs[1].Trim();
					switch (halfs[0]) {
						case "domain url": case "domain_url": case "domainurl": case "domain uri": case "domain_uri": case "domainuri":
							DomainURI = halfs[1]; break;
						case "version":
							Version = halfs[1]; break;
					}
				}
			}
		}

		private string domainURI = DEFAULT_DOMAIN_URI;
		public string DomainURI { get { return domainURI; } set { if (!string.IsNullOrWhiteSpace(value)) domainURI = value; } }
		private string version = DEFAULT_VERSION;
		public string Version { get { return version; } set { if (SUPPORTED_VERSIONS.Contains(value)) version = value; } }

		public string URIForModel(string contName) { return DomainURI + (DomainURI[0] == '/' ? "" : "/") + Version + "/" + contName; }

		public string QueriedURIForModel(string contName, object searchValsModel, params PropertyBinding[] modelSearchProps) {
			StringBuilder url = new StringBuilder(URIForModel(contName));
			url.Append('?');
			bool not1st = false;
			foreach (PropertyBinding pb in modelSearchProps) {
				if (not1st) url.Append('&');
				else not1st = true;
				object val = pb.LocalProperty.GetMethod.Invoke(searchValsModel, null);
				url.Append(pb.RemoteName);
				url.Append('=');
				url.Append(HttpUtility.UrlEncode(val == null ? "" : val.ToString()));
			}
			return url.ToString();
		}

		private static URI_Manager defaultManager = new URI_Manager();
		public static URI_Manager DefaultManager { get { return defaultManager; } }
	}
}
