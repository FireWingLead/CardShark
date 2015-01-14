using CardShark.PCShark.DAL.SQLiteLocal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CardShark.PCShark
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
			InitializeComponent();

			//LibrarySetListWaitAnimator.StartWait();
			//BackgroundWorker bw = new BackgroundWorker();
			//bw.DoWork += (sender, dwEvntArgs) => {
			//	BindingList<Set> sets = new BindingList<Set>();
			//	sets.Add(new Set() { Code = "M14" });
			//	((DAL.MTGAPI_Remote.SetAdapter)dwEvntArgs.Argument).RefreshSet(sets[0]);
			//	dwEvntArgs.Result = sets;
			//	Thread.Sleep(5000);
			//};
			//bw.RunWorkerCompleted += (sender, wcEvntArgs) => {
			//	LibrarySetsList.ItemsSource = (BindingList<Set>)wcEvntArgs.Result;
			//	LibrarySetListWaitAnimator.EndWait();
			//};
			
			//bw.RunWorkerAsync(RemoteSetContext);

			Card testCard = new Card() { ManaCost = "{R/W}{B/R}{w/r}{s}{p/U}{H/G}{2/U}{z}{Y}{X}{18}{u}{g}{∞}", Name = "Test Card", Id = -1, CardTypeData = CardType.FromString("Creature — Human ARES-Operator"), Author = "Perrin Larson" };
			CardVariation testVariation1 = new CardVariation() { Card = testCard, CardId = testCard.Id, Id = -1, FullCardImageBitmap = new BitmapImage(new Uri("pack://application:,,,/InterfaceGraphics/Test/Afterburner.png")) };
			CardVariation testVariation2 = new CardVariation() { Card = testCard, CardId = testCard.Id, Id = -2, FullCardImageBitmap = new BitmapImage(new Uri("pack://application:,,,/InterfaceGraphics/Test/BlankFF.jpg")) };
			testCard.CardVariations.Add(testVariation1);
			testCard.CardVariations.Add(testVariation2);
			LibraryCardsList.ItemsSource = new BindingList<Card>(new Card[] { testCard, testCard });
        }

		DAL.MTGAPI_Remote.SetAdapter RemoteSetContext = new DAL.MTGAPI_Remote.SetAdapter();
    }
}
