using CardShark.PCShark.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MTGImageRequestManager = CardShark.PCShark.DAL.MTGImage_Remote.RequestManager;
using MTGImageURIManager = CardShark.PCShark.DAL.MTGImage_Remote.URI_Manager;

namespace CardShark.PCShark.DAL.SQLiteLocal
{
	public partial class ManaSymbolSet
	{
		//private static readonly string DEF_MANA_SYMBOL_DIRECTORY = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "InterfaceGraphics\\DefManaSymbols\\");
		private static readonly string DEF_MANA_SYMBOL_DIRECTORY = "pack://application:,,,/InterfaceGraphics/DefManaSymbols/";
		private static readonly string[] DEF_MANA_SYMBOL_CODES = new string[] {
			"W", "U", "B", "R", "G", "P", "S", "H",
			"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
			"100", "1000000", "∞", "X", "Y", "Z",
			"W_U", "W_B", "W_R", "W_G", "U_B", "U_R", "U_G", "B_R", "B_G", "R_G",
			"H_W", "H_U", "H_B", "H_R", "H_G", "W_P", "U_P", "B_P", "R_P", "G_P",
			"2_W", "2_U", "2_B", "2_R", "2_G"
		};

		static ManaSymbolSet defSet = new ManaSymbolSet() { Id = -1 };
		public static ManaSymbolSet Default { get { return defSet; } }

		static ManaSymbolSet() {
			foreach (string mCode in DEF_MANA_SYMBOL_CODES) {
				string fName = DEF_MANA_SYMBOL_DIRECTORY + mCode + "-64.png";
				BitmapSource bmp = null;

				bmp = new BitmapImage(new Uri(fName));
				defSet.ManaSymbols.Add(new ManaSymbol() { Code = mCode, ManaSymbolSetId = -1, ImageBitmap = bmp, ManaSymbolSet = defSet });
			}
		}

		public ManaSymbol GetManaSymbolByCode(string unitImgCode) {
			return this.ManaSymbols.FirstOrDefault(ms => ms.Code == unitImgCode);
		}
	}
}
