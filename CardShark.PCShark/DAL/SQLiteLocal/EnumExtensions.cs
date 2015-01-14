using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardShark.PCShark.DAL.SQLiteLocal
{
	public static class EnumExtensions
	{
		public const int NON_HYBRIB_MANA_COLOR_COUNT = 7;

		public static int HybridCount(this ManaColors color) {
			int hCount = 0;
			if (color.HasFlag(ManaColors.Colorless)) hCount++;
			if (color.HasFlag(ManaColors.White)) hCount++;
			if (color.HasFlag(ManaColors.Blue)) hCount++;
			if (color.HasFlag(ManaColors.Black)) hCount++;
			if (color.HasFlag(ManaColors.Red)) hCount++;
			if (color.HasFlag(ManaColors.Green)) hCount++;
			if (color.HasFlag(ManaColors.Phyrexian)) hCount++;
			if (color.HasFlag(ManaColors.Snow)) hCount++;
			return hCount;
		}
		public static int SpecificityRank(this ManaColors color) {
			int rank = NON_HYBRIB_MANA_COLOR_COUNT - color.HybridCount();
			if (color.HasFlag(ManaColors.Colorless)) rank++;
			return rank;
		}
	}
}
