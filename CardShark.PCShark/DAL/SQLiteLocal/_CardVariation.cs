using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CardShark.PCShark.Extensions;

namespace CardShark.PCShark.DAL.SQLiteLocal
{
	public partial class CardVariation
	{
		public ManaSymbolSet SetManaStyle {
			get { return Set == null ? ManaSymbolSet.Default : (Set.ManaSymbolSet == null ? ManaSymbolSet.Default : Set.ManaSymbolSet); }
		}

		private byte[] fullCardImageData = null;
		private BitmapSource fullCardBitmap = null;
		public BitmapSource FullCardImageBitmap {
			get { return fullCardBitmap; }
			set {
				if (value == null) {
					fullCardImageData = null;
					fullCardBitmap = null;
					return;
				}
				fullCardBitmap = value;
				fullCardImageData = fullCardBitmap.ToPngData();
			}
		}
		public byte[] FullCardImage {
			get { return fullCardImageData; }
			set {
				if (value == null) {
					fullCardImageData = null;
					fullCardBitmap = null;
					return;
				}
				fullCardBitmap = ImagingExtensions.WPFImageFromData(value);
				fullCardImageData = value;
			}
		}
	}
}
