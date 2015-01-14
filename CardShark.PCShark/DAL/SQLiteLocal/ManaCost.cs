using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CardShark.PCShark.DAL.SQLiteLocal
{
	public class ManaCost : IComparable
	{

		public ManaCost(IEnumerable<ManaCostItem> costItems) {
			if (costItems == null) return;
			foreach (ManaCostItem item in costItems)
				AddCostItem(item);
		}

		private List<ManaCostItem> manaTypeParts = new List<ManaCostItem>();

		private void AddCostItem(ManaCostItem item) {
			if (item == null) return;
			int equivInd = manaTypeParts.FindIndex(mci => ManaCostItem.IsEquivalentType(item, mci));
			if (equivInd == -1) {
				int insInd = manaTypeParts.Count;
				while (insInd > 0 && item.CompareManaType(manaTypeParts[insInd - 1]) < 0)
					insInd--;
				manaTypeParts.Insert(insInd, item);
			}
			else {
				manaTypeParts[equivInd] += item;
			}
		}

		public IEnumerator<ManaCostItem> GetCostItemsEnumerator() { return manaTypeParts.GetEnumerator(); }

		public int ConvertedCost { get { return manaTypeParts.Aggregate(0, (ac, mci) => ac + mci.ConvertedQuantity); } }
		public ManaColors UsedColors { get { return manaTypeParts.Aggregate((ManaColors)0, (ac, mci) => ac | mci.Color); } }
		public int RequiredColorCount {
			get {
				throw new NotImplementedException();
			}
		}
		public int ColorSpecificityRank {
			get {
				throw new NotImplementedException();
			}
		}

		public int CompareTo(object obj) {
			if (!(obj is ManaCost))
				throw new ArgumentException("Cannot compare a ManaCost to a non-ManaCost object.");
			ManaCost rval = (ManaCost)obj;
			int lvalConvCost = this.ConvertedCost;
			int rvalConvCost = rval.ConvertedCost;
			if (lvalConvCost > rvalConvCost) return 1;
			if (lvalConvCost < rvalConvCost) return -1;
			ManaColors lvalUsedCols = this.UsedColors;
			ManaColors rvalUsedCols = rval.UsedColors;
			int lvalUsedColCount = lvalUsedCols.HybridCount();
			int rvalUsedColCount = rvalUsedCols.HybridCount();
			if (lvalUsedColCount > rvalUsedColCount) return 1;
			if (lvalUsedColCount < rvalUsedColCount) return -1;
			return ((int)lvalUsedCols).CompareTo((int)rvalUsedCols);
		}

		public static bool TryParse(string mcText, out ManaCost cost) {
			mcText = Regex.Replace(mcText, "\\s", string.Empty).TrimStart('{').TrimEnd('}');
			string[] textItems = mcText.Split(new string[] { "}{" }, StringSplitOptions.RemoveEmptyEntries);
			ManaCost parsedCost = new ManaCost(null);
			foreach (string tItem in textItems) {
				ManaCostItem item = null;
				if (ManaCostItem.TryParseSingleTextUnit(tItem, out item)) {
					parsedCost.AddCostItem(item);
				}
				else {
					cost = null;
					return false;
				}
			}
			cost = parsedCost;
			return true;
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder(manaTypeParts.Count * 3);
			foreach (ManaCostItem item in manaTypeParts)
				sb.Append(item.ToString());
			return sb.ToString();
		}
	}
}
