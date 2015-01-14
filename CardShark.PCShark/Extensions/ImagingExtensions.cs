using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardShark.PCShark.Extensions
{
	public static class ImagingExtensions
	{
		public static BitmapImage WPFImageFromStream(Stream strm) {
			if (strm == null || strm.Length == 0) return null;
			strm.Position = 0;
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.UriSource = null;
			image.StreamSource = strm;
			image.EndInit();
			image.Freeze();
			return image;
		}
		public static BitmapImage WPFImageFromData(byte[] imgData) {
			if (imgData == null || imgData.Length == 0) return null;
			BitmapImage image;
			using (MemoryStream strm = new MemoryStream(imgData)) {
				image = WPFImageFromStream(strm);
			}
			return image;
		}
		public static byte[] ToPngData(this BitmapSource bmp) {
			PngBitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bmp));
			byte[] data;
			using (MemoryStream strm = new MemoryStream()) {
				encoder.Save(strm);
				data = strm.ToArray();
				strm.Close();
			}
			return data;
		}

		public static Image SysDrawImageFromData(byte[] imgData) {
			if (imgData == null) return null;
			Image img;
			using (MemoryStream strm = new MemoryStream(imgData)) {
				img = Image.FromStream(strm);
				strm.Close();
			}
			return img;
		}
		public static byte[] ToPngData(this Image img) {
			byte[] data;
			using (MemoryStream strm = new MemoryStream()) {
				img.Save(strm, ImageFormat.Png);
				data = strm.ToArray();
			}
			return data;
		}
	}
}
