using CardShark.PCShark.DAL.SQLiteLocal;
using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CardShark.PCShark
{
	/// <summary>
	/// Interaction logic for ManaCostLabel.xaml
	/// </summary>
	public partial class ManaCostLabel : UserControl
	{
		public ManaCostLabel() {
			InitializeComponent();
		}

		public ManaCost ManaCostValue {
			get { return (ManaCost)GetValue(ManaCostValueProperty); }
			set { SetValue(ManaCostValueProperty, value); }
		}
		public static readonly DependencyProperty ManaCostValueProperty = DependencyProperty.Register("ManaCostValue", typeof(ManaCost), typeof(ManaCostLabel),
			new PropertyMetadata(null, OnManaCostValueChanged));
		protected static void OnManaCostValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { UpdateImages((ManaCostLabel)d); }

		protected static void UpdateImages(ManaCostLabel _this) {
			ManaCost newVal = _this.ManaCostValue;
			IEnumerator chldEnum = _this.ItemPanel.Children.GetEnumerator();
			IEnumerator<ManaCostItem> itmsEnum = newVal.GetCostItemsEnumerator();
			int dupInd = 0;
			ManaSymbol symbol = null;
			ManaCostItem item = null;
			Image cChild = null;
			while (chldEnum.MoveNext()) {//Use up already existing Image controls.
				cChild = (Image)chldEnum.Current;
				if (!itmsEnum.MoveNext()) goto USED_ALL_ITEMS;
				item = itmsEnum.Current;
				symbol = _this.ManaSymbolSet.GetManaSymbolByCode(item.SingleUnitImageCode);
				if (symbol == null) symbol = ManaSymbol.NotSupportedByStyleSymbol;
				for (dupInd = 0; dupInd < item.DuplicationQuantity; dupInd++) {
					cChild.Source = symbol.ImageBitmap;
					if (!chldEnum.MoveNext()) {
						dupInd++;
						goto USED_EXISTING_CHILDREN;
					}
					cChild = (Image)chldEnum.Current;
				}
			}
			USED_EXISTING_CHILDREN:
			if (item == null) {//There were no existing children. Nothing has been initialized. Initialize it or move on if no items to handle.
				if (itmsEnum.MoveNext()) {
					item = itmsEnum.Current;
					symbol = _this.ManaSymbolSet.GetManaSymbolByCode(item.SingleUnitImageCode);
					if (symbol == null) symbol = ManaSymbol.NotSupportedByStyleSymbol;
				}
				else {
					goto USED_ALL_ITEMS;
				}
			}
			while (dupInd < item.DuplicationQuantity) {//If last item still needed more duplications, add children for them.
				Image newChild = CreateImageChild(_this);
				newChild.Source = symbol.ImageBitmap;
				_this.ItemPanel.Children.Add(newChild);
				dupInd++;
			}
			while (itmsEnum.MoveNext()) {//Add needed child elements with their appropriate symbol for remaining items.
				item = itmsEnum.Current;
				symbol = _this.ManaSymbolSet.GetManaSymbolByCode(item.SingleUnitImageCode);
				if (symbol == null) symbol = ManaSymbol.NotSupportedByStyleSymbol;
				for (dupInd = 0; dupInd < item.DuplicationQuantity; dupInd++) {
					Image newChild = CreateImageChild(_this);
					newChild.Source = symbol.ImageBitmap;
					_this.ItemPanel.Children.Add(newChild);
				}
			}
			USED_ALL_ITEMS:
			if (cChild != null) {//Remove any remaining Images left over from last ManaCostValue.
				List<Image> toRemove = new List<Image>();
				toRemove.Add(cChild);
				while (chldEnum.MoveNext())
					toRemove.Add((Image)chldEnum.Current);
				foreach (Image child in toRemove)
					_this.ItemPanel.Children.Remove(child);
			}
		}

		private static Image CreateImageChild(ManaCostLabel _this) {
			Image child = new Image();
			child.Stretch = Stretch.Uniform;
			child.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
			child.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
			child.Height = _this.Height;
			return child;
		}

		public ManaSymbolSet ManaSymbolSet {
			get { return (ManaSymbolSet)GetValue(ManaSymbolSetProperty); }
			set { SetValue(ManaCostValueProperty, value); }
		}
		public static readonly DependencyProperty ManaSymbolSetProperty = DependencyProperty.Register("ManaSymbolSet", typeof(ManaSymbolSet), typeof(ManaCostLabel),
			new PropertyMetadata(ManaSymbolSet.Default, OnManaSymbolSetChanged, (d, baseVal) => (baseVal == null ? ((ManaCostLabel)d).ManaSymbolSet : baseVal)));
		protected static void OnManaSymbolSetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { UpdateImages((ManaCostLabel)d); }
	}
}
