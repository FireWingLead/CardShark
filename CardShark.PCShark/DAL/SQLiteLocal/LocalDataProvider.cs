using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardShark.PCShark.DAL.SQLiteLocal
{
	public class LocalDataProvider
	{
		static LocalCardDataEntities dataContext = new LocalCardDataEntities();
		public static LocalCardDataEntities DataContext { get { return dataContext; } }
	}
}
