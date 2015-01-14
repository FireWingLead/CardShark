using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeJSON;
using System.Net.Http;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace CardShark.PCShark.DAL.MTGAPI_Remote
{
	public class RequestManager
	{
		public static JSONNode GetRequest(string uri) {
			HttpClient client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
			HttpResponseMessage response = null;
			try { response = client.SendAsync(request).Result; }
			catch (HttpRequestException ex) {
				Logger.Default.LogError("An HTTP error occurred while fetching a request from MTGAPI.", ex, null);
				return null;
			}
			catch (Exception ex) {
				Logger.Default.LogError("An error occurred while fetching a request from MTGAPI.", ex, null);
				return null;
			}
			
			JSONParser parser = new JSONParser();
			JSONNode parsed = null;
			Stream raw = null;
			try {
				raw = response.Content.ReadAsStreamAsync().Result;
				parsed = parser.Parse(new StreamReader(raw));
			}
			catch (JSONSyntaxException ex) {
				raw.Position = 0;
				StreamReader reader = new StreamReader(raw);
				Logger.Default.LogError("Error parsing response from MTGAPI.", ex, "<RequestBody><![CDATA[" + reader.ReadToEnd() + "]]></RequestBody>");
				return null;
			}
			catch (Exception ex) {
				raw.Position = 0;
				StreamReader reader = new StreamReader(raw);
				Logger.Default.LogError("Error parsing response from MTGAPI.", ex, "<RequestBody><![CDATA[" + reader.ReadToEnd() + "]]></RequestBody>");
				return null;
			}
			return parsed;
		}

		public static Image GetImage(string uri) {
			HttpClient client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
			HttpResponseMessage response = null;
			try { response = client.SendAsync(request).Result; }
			catch (HttpRequestException ex) {
				Logger.Default.LogError("An HTTP error occurred while fetching an image via a url derived from MTGAPI data.", ex, null);
				return null;
			}
			catch (Exception ex) {
				Logger.Default.LogError("An error occurred while fetching an image via a url derived from MTGAPI data.", ex, null);
				return null;
			}

			Image img = null;
			try { img = Image.FromStream(response.Content.ReadAsStreamAsync().Result); }
			catch (Exception ex) {
				Logger.Default.LogError("Error decoding image data fetched via a url derived from MTGAPI data.", ex, null);
				return null;
			}
			return img;
		}
	}
}
