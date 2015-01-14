using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardShark.PCShark.Extensions
{
	public static class ListExtensions
	{
		public static List<NewItemType> ConvertToList<OldItemType, NewItemType>(this IEnumerable<OldItemType> oldList, Func<OldItemType, NewItemType> converter) {
			List<NewItemType> newList = new List<NewItemType>();
			foreach (OldItemType oldItem in oldList)
				newList.Add(converter(oldItem));
			return newList;
		}
	}
}
