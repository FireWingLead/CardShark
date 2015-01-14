using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CardShark.PCShark.DAL.SQLiteLocal
{
	public class ManaCostItem
	{
		public const char WHITE_ABBREV = 'W';
		public const char BLUE_ABBREV = 'U';
		public const char BLACK_ABBREV = 'B';
		public const char RED_ABBREV = 'R';
		public const char GREEN_ABBREV = 'G';
		public const char PHYR_ABBREV = 'P';
		public const char SNOW_ABBREV = 'S';
		public const char HALF_ABBREV = 'H';

		public ManaCostItem(ManaColors color, int colorlessQuantity, string variable, int itemDuplicationQuantity) {
			Color = color;
			Variable = String.IsNullOrWhiteSpace(variable) ? null : variable[0].ToString();
			ColorlessQuantity = color.HasFlag(ManaColors.Colorless) ? (Variable == null ? colorlessQuantity : 0) : 0;
			DuplicationQuantity = color == ManaColors.Colorless && Variable == null ? 1 : itemDuplicationQuantity;
			SingleUnitText = composeSingleUnitText();
			SingleUnitImageCode = Regex.Replace(SingleUnitText.TrimStart('{').TrimEnd('}'), "/", "_");
		}

		public ManaColors Color { get; private set; }
		public int ColorlessQuantity { get; private set; }
		public string Variable { get; private set; }
		public int DuplicationQuantity { get; private set; }

		public int ConvertedQuantity {
			get {
				if (Variable != null) return 0;
				if (Color == ManaColors.Colorless)
					return ColorlessQuantity;
				return DuplicationQuantity;
			}
		}

		public string SingleUnitText { get; private set; }

		public string SingleUnitImageCode { get; private set; }

		public static bool IsEquivalentType(ManaCostItem item1, ManaCostItem item2) {
			if (item1 == null || item2 == null) return true;
			if (item1.Color == ManaColors.Colorless && item2.Color == ManaColors.Colorless)
				return item1.Variable == item2.Variable;
			return item1.Color == item2.Color && item1.ColorlessQuantity == item2.ColorlessQuantity && item1.Variable == item2.Variable;
		}

		public static ManaCostItem operator +(ManaCostItem lval, ManaCostItem rval) {
			if (lval == null) return rval;
			if (rval == null) return lval;
			if (!ManaCostItem.IsEquivalentType(lval, rval))
				throw new ArgumentException("Cannot add ManaCostItems of different types.");
			if (lval.Color == ManaColors.Colorless && lval.Variable == null)
				return new ManaCostItem(lval.Color, lval.ColorlessQuantity + rval.ColorlessQuantity, lval.Variable, lval.DuplicationQuantity);
			return new ManaCostItem(lval.Color, lval.ColorlessQuantity, lval.Variable, lval.DuplicationQuantity + rval.DuplicationQuantity);
		}
		public static ManaCostItem operator -(ManaCostItem lval, ManaCostItem rval) {
			if (rval == null) return lval;
			if (lval == null) return null;
			if (!ManaCostItem.IsEquivalentType(lval, rval))
				throw new ArgumentException("Cannot subtract ManaCostItems of different types.");
			int quant = -1;
			if (lval.Color == ManaColors.Colorless && lval.Variable == null) {
				quant = lval.ColorlessQuantity - rval.ColorlessQuantity;
				if (quant < 1) return null;
				return new ManaCostItem(lval.Color, quant, lval.Variable, lval.DuplicationQuantity);
			}
			quant = lval.DuplicationQuantity + rval.DuplicationQuantity;
			if (quant < 1) return null;
			return new ManaCostItem(lval.Color, lval.ColorlessQuantity, lval.Variable, quant);
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < DuplicationQuantity; i++)
				sb.Append(SingleUnitText);
			return sb.ToString();
		}

		private string composeSingleUnitText() {
			StringBuilder sb = new StringBuilder("{");
			bool not1st = false;
			if (Color.HasFlag(ManaColors.Colorless)) {
				sb.Append(Variable == null ? ColorlessQuantity.ToString() : Variable);
				not1st = true;
			}
			else if (Variable != null) {
				sb.Append(Variable);
			}
			if (Color.HasFlag(ManaColors.Half)) {
				if (not1st) sb.Append('/');
				else not1st = true;
				sb.Append(HALF_ABBREV);
			}
			if (Color.HasFlag(ManaColors.White)) {
				if (not1st) sb.Append('/');
				else not1st = true;
				sb.Append(WHITE_ABBREV);
			}
			if (Color.HasFlag(ManaColors.Blue)) {
				if (not1st) sb.Append('/');
				else not1st = true;
				sb.Append(BLUE_ABBREV);
			}
			if (Color.HasFlag(ManaColors.Black)) {
				if (not1st) sb.Append('/');
				else not1st = true;
				sb.Append(BLACK_ABBREV);
			}
			if (Color.HasFlag(ManaColors.Red)) {
				if (not1st) sb.Append('/');
				else not1st = true;
				sb.Append(RED_ABBREV);
			}
			if (Color.HasFlag(ManaColors.Green)) {
				if (not1st) sb.Append('/');
				else not1st = true;
				sb.Append(GREEN_ABBREV);
			}
			if (Color.HasFlag(ManaColors.Phyrexian)) {
				if (not1st) sb.Append('/');
				else not1st = true;
				sb.Append(PHYR_ABBREV);
			}
			if (Color.HasFlag(ManaColors.Snow)) {
				if (not1st) sb.Append('/');
				else not1st = true;
				sb.Append(SNOW_ABBREV);
			}
			sb.Append('}');
			return sb.ToString();
		}

		public static bool TryParseSingleTextUnit(string val, out ManaCostItem item) {
			val = Regex.Replace(val, "\\s", string.Empty);
			val = val.TrimStart('{').TrimEnd('}').ToUpper();
			ManaColors color = 0;
			string variable = null;
			int colorlessQuant = -1;

			string[] prts = val.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			if (prts.Length == 0) {
				item = null;
				return true;
			}

			for (int i = 0; i < prts.Length; i++) {
				ManaColors partCol = 0;
				while (prts[i].Length > 0) {
					ManaColors charCol = CharColor(prts[i][0]);
					if (charCol == ManaColors.Colorless) {
						if (char.IsDigit(prts[i][0])) {
							Match numMatch = Regex.Match(prts[i], "^\\d+");
							colorlessQuant = int.Parse(numMatch.Value);
							variable = null;
							if (prts[i].Length > numMatch.Value.Length)
								prts[i] = prts[i].Substring(numMatch.Value.Length);
							else
								prts[i] = string.Empty;
						}
						else {
							variable = prts[i][0].ToString();
							colorlessQuant = -1;
							if (prts[i].Length > 1)
								prts[i] = prts[i].Substring(1);
							else
								prts[i] = string.Empty;
						}
						if (partCol == 0)
							partCol = charCol;
					}
					else {
						partCol = charCol;
						if (prts[i].Length > 1)
							prts[i] = prts[i].Substring(1);
						else
							prts[i] = string.Empty;
					}
				}
				color |= partCol;
			}

			item = new ManaCostItem(color, colorlessQuant, variable, 1);
			return true;
		}
		public static ManaColors CharColor(char chr) {
			switch (chr) {
				case WHITE_ABBREV: return ManaColors.White;
				case BLUE_ABBREV: return ManaColors.Blue;
				case BLACK_ABBREV: return ManaColors.Black;
				case RED_ABBREV: return ManaColors.Red;
				case GREEN_ABBREV: return ManaColors.Green;
				case PHYR_ABBREV: return ManaColors.Phyrexian;
				case SNOW_ABBREV: return ManaColors.Snow;
				case HALF_ABBREV: return ManaColors.Half;
				default: return ManaColors.Colorless;
			}
		}

		public int CompareManaType(ManaCostItem rval) {
			int lvalHCount = this.Color.HybridCount();
			int rvalHCount = rval.Color.HybridCount();
			if (lvalHCount > rvalHCount) return 1;
			if (lvalHCount < rvalHCount) return -1;
			bool lvalIsHybClrls = this.Color != ManaColors.Colorless && this.Color.HasFlag(ManaColors.Colorless);
			bool rvalIsHybClrls = rval.Color != ManaColors.Colorless && rval.Color.HasFlag(ManaColors.Colorless);
			if (lvalIsHybClrls && rvalIsHybClrls) {
				if (this.ColorlessQuantity > rval.ColorlessQuantity) return 1;
				if (this.ColorlessQuantity < rval.ColorlessQuantity) return -1;
			}
			return ((int)this.Color).CompareTo((int)rval.Color);
		}
	}
}
