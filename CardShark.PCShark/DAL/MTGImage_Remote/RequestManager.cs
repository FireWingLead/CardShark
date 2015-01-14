using CardShark.PCShark.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardShark.PCShark.DAL.MTGImage_Remote
{
	public class RequestManager
	{
		[DllImport("wininet.dll")]
		private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
		public static bool TestInternetConnection() {
			int desc;
			return InternetGetConnectedState(out desc, 0);
		}

		private static HttpResponseMessage GetResponse(string uri) {
			HttpClient client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
			HttpResponseMessage response = null;
			try { response = client.SendAsync(request).Result; }
			catch (HttpRequestException ex) {
				Logger.Default.LogError("An HTTP error occurred while fetching an image via mtgimage.com.", ex, "<URI><![CDATA[" + uri + "]]></URI>");
				return null;
			}
			catch (Exception ex) {
				Logger.Default.LogError("An error occurred while fetching an image via mtgimage.com.", ex, "<URI><![CDATA[" + uri + "]]></URI>");
				return null;
			}
			return response;
		}

		public static byte[] GetImageData(string uri) {
			HttpResponseMessage response = GetResponse(uri);
			if (response == null) return null;
			byte[] data;
			try { data = response.Content.ReadAsByteArrayAsync().Result; }
			catch (Exception ex) {
				Logger.Default.LogError("Error reading data returned from mtgimage.com.", ex, "<URI><![CDATA[" + uri + "]]></URI>");
				return null;
			}
			return data;
		}

		public static Image GetImageObject(string uri) {
			HttpResponseMessage response = GetResponse(uri);
			if (response == null) return null;
			Image img = null;
			try { img = Image.FromStream(response.Content.ReadAsStreamAsync().Result); }
			catch (Exception ex) {
				Logger.Default.LogError("Error decoding image data fetched via a mtgimage.com.", ex, null);
				return null;
			}
			return img;
		}

		public static BitmapSource GetImageBitmap(string uri) {
			HttpResponseMessage response = GetResponse(uri);
			if (response == null) return null;
			try { return ImagingExtensions.WPFImageFromStream(response.Content.ReadAsStreamAsync().Result); }
			catch (Exception ex) {
				Logger.Default.LogError("Error decoding image data fetched via a mtgimage.com.", ex, null);
				return null;
			}
		}
	}
}
