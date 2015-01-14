using CardShark.PCShark.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CardShark.PCShark.DAL.SQLiteLocal
{
	#region EF 6.x Version
	//public partial class ManaSymbol
	//{
	//	BitmapSource imageBitmap;
	//	public BitmapSource ImageBitmap {
	//		get { return imageBitmap; }
	//		set {
	//			if (imageBitmap == value) return;
	//			if (value == null) {
	//				setNullImg();
	//				return;
	//			}
	//			imageBitmap = value;
	//			ReportPropertyChanging("Image");
	//			_Image = value.ToPngData();
	//			ReportPropertyChanged("Image");
	//			imageObj = ImagingExtensions.SysDrawImageFromData(_Image);
	//		}
	//	}
	//	Image imageObj;
	//	public Image ImageObject {
	//		get { return imageObj; }
	//		set {
	//			if (imageObj == value) return;
	//			if (value == null) {
	//				setNullImg();
	//				return;
	//			}
	//			imageObj = value;
	//			ReportPropertyChanging("Image");
	//			_Image = value.ToPngData();
	//			ReportPropertyChanged("Image");
	//			imageBitmap = ImagingExtensions.WPFImageFromData(_Image);
	//		}
	//	}
	//	partial void OnImageChanged() {
	//		if (_Image == null) {
	//			setNullImg();
	//			return;
	//		}
	//		imageBitmap = ImagingExtensions.WPFImageFromData(_Image);
	//		imageObj = ImagingExtensions.SysDrawImageFromData(_Image);
	//	}

	//	private void setNullImg() {
	//		imageObj = null;
	//		ReportPropertyChanging("Image");
	//		_Image = null;
	//		ReportPropertyChanged("Image");
	//		imageBitmap = null;
	//	}
	//}
	#endregion EF 6.x Version

	#region EF 5.x Version
	public partial class ManaSymbol
	{
		private static readonly ManaSymbol notSupportedByStyleSymbol;
		public static ManaSymbol NotSupportedByStyleSymbol { get { return notSupportedByStyleSymbol; } }

		static ManaSymbol() {
			notSupportedByStyleSymbol = new ManaSymbol() { Code = null };
			BitmapImage img = new BitmapImage(new Uri("pack://application:,,,/InterfaceGraphics/DefManaSymbols/NotSupported.png"));
			notSupportedByStyleSymbol.ImageBitmap = img;
		}

		BitmapSource imageBitmap;
		public BitmapSource ImageBitmap {
			get { return imageBitmap; }
			set {
				if (imageBitmap == value) return;
				if (value == null) {
					setNullImg();
					return;
				}
				imageBitmap = value;
				image = value.ToPngData();
				imageObj = ImagingExtensions.SysDrawImageFromData(image);
			}
		}
		Image imageObj;
		public Image ImageObject {
			get { return imageObj; }
			set {
				if (imageObj == value) return;
				if (value == null) {
					setNullImg();
					return;
				}
				imageObj = value;
				image = value.ToPngData();
				imageBitmap = ImagingExtensions.WPFImageFromData(image);
			}
		}
		byte[] image;
		public byte[] Image {
			get { return image; }
			set {
				if (image == value) return;
				if (value == null) {
					setNullImg();
					return;
				}
				image = value;
				imageBitmap = ImagingExtensions.WPFImageFromData(value);
				imageObj = ImagingExtensions.SysDrawImageFromData(value);
			}
		}

		private void setNullImg() {
			imageObj = null;
			image = null;
			imageBitmap = null;
		}
	}
	#endregion EF 5.x Version
}
