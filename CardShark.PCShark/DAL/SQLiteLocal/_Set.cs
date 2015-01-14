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
	//public partial class Set
	//{
	//	bool suspendPropSyncUpdates = false;
	//	void setSymbolPropNull(ref byte[] data, ref BitmapSource bmp, ref Image img) {
	//		data = null;
	//		img = null;
	//		bmp = null;
	//	}
	//	void setSymbolPropData(byte[] value, ref byte[] data, ref BitmapSource bmp, ref Image img) {
	//		if (value == null) {
	//			setSymbolPropNull(ref data, ref bmp, ref img);
	//			return;
	//		}
	//		bmp = ImagingExtensions.WPFImageFromData(value);
	//		img = ImagingExtensions.SysDrawImageFromData(value);
	//	}
	//	void setSymbolPropBmp(BitmapSource value, ref byte[] data, ref BitmapSource bmp, ref Image img, string propName) {
	//		if (bmp == value) return;
	//		if (value == null) {
	//			ReportPropertyChanging(propName);
	//			setSymbolPropNull(ref data, ref bmp, ref img);
	//			ReportPropertyChanged(propName);
	//			return;
	//		}
	//		bmp = value;
	//		ReportPropertyChanging(propName);
	//		data = value.ToPngData();
	//		ReportPropertyChanged(propName);
	//		img = ImagingExtensions.SysDrawImageFromData(data);
	//	}
	//	void setSymbolPropImg(Image value, ref byte[] data, ref BitmapSource bmp, ref Image img, string propName) {
	//		if (img == value) return;
	//		if (value == null) {
	//			ReportPropertyChanging(propName);
	//			setSymbolPropNull(ref data, ref bmp, ref img);
	//			ReportPropertyChanged(propName);
	//			return;
	//		}
	//		img = value;
	//		ReportPropertyChanging(propName);
	//		data = value.ToPngData();
	//		ReportPropertyChanged(propName);
	//		bmp = ImagingExtensions.WPFImageFromData(data);
	//	}

	//	BitmapSource commonSymbolBmp;
	//	Image commonSymbolImg;
	//	partial void OnCommonSymbolChanged() {
	//		setSymbolPropData(_CommonSymbol, ref _CommonSymbol, ref commonSymbolBmp, ref commonSymbolImg);
	//	}
	//	public BitmapSource CommonSymbolBitmap {
	//		get { return commonSymbolBmp; }
	//		set { setSymbolPropBmp(value, ref _CommonSymbol, ref commonSymbolBmp, ref commonSymbolImg, "CommonSymbol"); }
	//	}
	//	public Image CommonSymbolImage {
	//		get { return commonSymbolImg; }
	//		set { setSymbolPropImg(value, ref _CommonSymbol, ref commonSymbolBmp, ref commonSymbolImg, "CommonSymbol"); }
	//	}

	//	BitmapSource uncommonSymbolBmp;
	//	Image uncommonSymbolImg;
	//	partial void OnUncommonSymbolChanged() {
	//		setSymbolPropData(_UncommonSymbol, ref _UncommonSymbol, ref uncommonSymbolBmp, ref uncommonSymbolImg);
	//	}
	//	public BitmapSource UncommonSymbolBitmap {
	//		get { return uncommonSymbolBmp; }
	//		set { setSymbolPropBmp(value, ref _UncommonSymbol, ref uncommonSymbolBmp, ref uncommonSymbolImg, "UncommonSymbol"); }
	//	}
	//	public Image UncommonSymbolImage {
	//		get { return uncommonSymbolImg; }
	//		set { setSymbolPropImg(value, ref _UncommonSymbol, ref uncommonSymbolBmp, ref uncommonSymbolImg, "UncommonSymbol"); }
	//	}

	//	BitmapSource rareSymbolBmp;
	//	Image rareSymbolImg;
	//	partial void OnRareSymbolChanged() {
	//		setSymbolPropData(_UncommonSymbol, ref _RareSymbol, ref rareSymbolBmp, ref rareSymbolImg);
	//	}
	//	public BitmapSource RareSymbolBitmap {
	//		get { return rareSymbolBmp; }
	//		set { setSymbolPropBmp(value, ref _RareSymbol, ref rareSymbolBmp, ref rareSymbolImg, "RareSymbol"); }
	//	}
	//	public Image RareSymbolImage {
	//		get { return rareSymbolImg; }
	//		set { setSymbolPropImg(value, ref _RareSymbol, ref rareSymbolBmp, ref rareSymbolImg, "RareSymbol"); }
	//	}

	//	BitmapSource mythicSymbolBmp;
	//	Image mythicSymbolImg;
	//	partial void OnMythicSymbolChanged() {
	//		setSymbolPropData(_UncommonSymbol, ref _MythicSymbol, ref mythicSymbolBmp, ref mythicSymbolImg);
	//	}
	//	public BitmapSource MythicSymbolBitmap {
	//		get { return mythicSymbolBmp; }
	//		set { setSymbolPropBmp(value, ref _MythicSymbol, ref mythicSymbolBmp, ref mythicSymbolImg, "MythicSymbol"); }
	//	}
	//	public Image MythicSymbolImage {
	//		get { return mythicSymbolImg; }
	//		set { setSymbolPropImg(value, ref _MythicSymbol, ref mythicSymbolBmp, ref mythicSymbolImg, "MythicSymbol"); }
	//	}

	//	BitmapSource specialSymbolBmp;
	//	Image specialSymbolImg;
	//	partial void OnSpecialSymbolChanged() {
	//		setSymbolPropData(_UncommonSymbol, ref _SpecialSymbol, ref specialSymbolBmp, ref specialSymbolImg);
	//	}
	//	public BitmapSource SpecialSymbolBitmap {
	//		get { return specialSymbolBmp; }
	//		set { setSymbolPropBmp(value, ref _SpecialSymbol, ref specialSymbolBmp, ref specialSymbolImg, "SpecialSymbol"); }
	//	}
	//	public Image SpecialSymbolImage {
	//		get { return specialSymbolImg; }
	//		set { setSymbolPropImg(value, ref _SpecialSymbol, ref specialSymbolBmp, ref specialSymbolImg, "SpecialSymbol"); }
	//	}

	//	public string CommonSymbolURI { get; set; }
	//	public string UncommonSymbolURI { get; set; }
	//	public string RareSymbolURI { get; set; }
	//	public string MythicSymbolURI { get; set; }
	//	public string SpecialSymbolURI { get; set; }
	//}
	#endregion EF 6.x Version

	#region EF 5.x Version
	public partial class Set
	{
		bool suspendPropSyncUpdates = false;
		void setSymbolPropNull(ref byte[] data, ref BitmapSource bmp, ref Image img) {
			data = null;
			img = null;
			bmp = null;
		}
		void setSymbolPropData(byte[] value, ref byte[] data, ref BitmapSource bmp, ref Image img) {
			if (data == value) return;
			if (value == null) {
				setSymbolPropNull(ref data, ref bmp, ref img);
				return;
			}
			data = value;
			bmp = ImagingExtensions.WPFImageFromData(value);
			img = ImagingExtensions.SysDrawImageFromData(value);
		}
		void setSymbolPropBmp(BitmapSource value, ref byte[] data, ref BitmapSource bmp, ref Image img) {
			if (bmp == value) return;
			if (value == null) {
				setSymbolPropNull(ref data, ref bmp, ref img);
				return;
			}
			bmp = value;
			data = value.ToPngData();
			img = ImagingExtensions.SysDrawImageFromData(data);
		}
		void setSymbolPropImg(Image value, ref byte[] data, ref BitmapSource bmp, ref Image img) {
			if (img == value) return;
			if (value == null) {
				setSymbolPropNull(ref data, ref bmp, ref img);
				return;
			}
			img = value;
			data = value.ToPngData();
			bmp = ImagingExtensions.WPFImageFromData(data);
		}

		byte[] commonSymbolData;
		BitmapSource commonSymbolBmp;
		Image commonSymbolImg;
		public byte[] CommonSymbol {
			get { return commonSymbolData; }
			set { setSymbolPropData(value, ref commonSymbolData, ref commonSymbolBmp, ref commonSymbolImg); }
		}
		public BitmapSource CommonSymbolBitmap {
			get { return commonSymbolBmp; }
			set { setSymbolPropBmp(value, ref commonSymbolData, ref commonSymbolBmp, ref commonSymbolImg); }
		}
		public Image CommonSymbolImage {
			get { return commonSymbolImg; }
			set { setSymbolPropImg(value, ref commonSymbolData, ref commonSymbolBmp, ref commonSymbolImg); }
		}

		byte[] uncommonSymbolData;
		BitmapSource uncommonSymbolBmp;
		Image uncommonSymbolImg;
		public byte[] UncommonSymbol {
			get { return uncommonSymbolData; }
			set { setSymbolPropData(value, ref uncommonSymbolData, ref uncommonSymbolBmp, ref uncommonSymbolImg); }
		}
		public BitmapSource UncommonSymbolBitmap {
			get { return uncommonSymbolBmp; }
			set { setSymbolPropBmp(value, ref uncommonSymbolData, ref uncommonSymbolBmp, ref uncommonSymbolImg); }
		}
		public Image UncommonSymbolImage {
			get { return uncommonSymbolImg; }
			set { setSymbolPropImg(value, ref uncommonSymbolData, ref uncommonSymbolBmp, ref uncommonSymbolImg); }
		}

		byte[] rareSymbolData;
		BitmapSource rareSymbolBmp;
		Image rareSymbolImg;
		public byte[] RareSymbol {
			get { return rareSymbolData; }
			set { setSymbolPropData(value, ref rareSymbolData, ref rareSymbolBmp, ref rareSymbolImg); }
		}
		public BitmapSource RareSymbolBitmap {
			get { return rareSymbolBmp; }
			set { setSymbolPropBmp(value, ref rareSymbolData, ref rareSymbolBmp, ref rareSymbolImg); }
		}
		public Image RareSymbolImage {
			get { return rareSymbolImg; }
			set { setSymbolPropImg(value, ref rareSymbolData, ref rareSymbolBmp, ref rareSymbolImg); }
		}

		byte[] mythicSymbolData;
		BitmapSource mythicSymbolBmp;
		Image mythicSymbolImg;
		public byte[] MythicSymbol {
			get { return mythicSymbolData; }
			set { setSymbolPropData(value, ref mythicSymbolData, ref mythicSymbolBmp, ref mythicSymbolImg); }
		}
		public BitmapSource MythicSymbolBitmap {
			get { return mythicSymbolBmp; }
			set { setSymbolPropBmp(value, ref mythicSymbolData, ref mythicSymbolBmp, ref mythicSymbolImg); }
		}
		public Image MythicSymbolImage {
			get { return mythicSymbolImg; }
			set { setSymbolPropImg(value, ref mythicSymbolData, ref mythicSymbolBmp, ref mythicSymbolImg); }
		}

		byte[] specialSymbolData;
		BitmapSource specialSymbolBmp;
		Image specialSymbolImg;
		public byte[] SpecialSymbol {
			get { return specialSymbolData; }
			set { setSymbolPropData(value, ref specialSymbolData, ref specialSymbolBmp, ref specialSymbolImg); }
		}
		public BitmapSource SpecialSymbolBitmap {
			get { return specialSymbolBmp; }
			set { setSymbolPropBmp(value, ref specialSymbolData, ref specialSymbolBmp, ref specialSymbolImg); }
		}
		public Image SpecialSymbolImage {
			get { return specialSymbolImg; }
			set { setSymbolPropImg(value, ref specialSymbolData, ref specialSymbolBmp, ref specialSymbolImg); }
		}

		public string CommonSymbolURI { get; set; }
		public string UncommonSymbolURI { get; set; }
		public string RareSymbolURI { get; set; }
		public string MythicSymbolURI { get; set; }
		public string SpecialSymbolURI { get; set; }
	}
	#endregion EF 5.x Version
}
