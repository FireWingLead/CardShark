using CardShark.PCShark.DAL.SQLiteLocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CardShark.PCShark
{
	/// <summary>
	/// Interaction logic for SetImporterDialog.xaml
	/// </summary>
	public partial class SetImporterDialog : Window
	{
		public SetImporterDialog() {
			InitializeComponent();
		}
	}

	public class SetListItem
	{
		public Set Set { get; set; }
		public bool Selected { get; set; }
	}
	public class CardListItem
	{
		public Card Card { get; set; }
		public bool Selected { get; set; }
	}
	public class CardVariationListItem
	{
		public CardVariation CardVariation { get; set; }
		public bool Selected { get; set; }
	}
}
