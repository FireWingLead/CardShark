using CardShark.PCShark.DAL.SQLiteLocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CardShark.PCShark
{
	public enum ManaCostTextBlockDisplayModes
	{
		Text, Image
	}

	public class ManaCostTextBlock : TextBlock
	{
		protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
			if (ManaCostDisplayMode != ManaCostTextBlockDisplayModes.Text)
				SwitchToText();
			Editing = true;
		}
		protected override void OnLostKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e) {
			base.OnLostKeyboardFocus(e);
			if (this.Text != displayText) {
				ManaCost parsedCost = null;
				ManaCost.TryParse(this.Text, out parsedCost);
				ManaCostValue = parsedCost;
				SwitchToImages();
			}
			Editing = false;
		}

		public ManaCost ManaCostValue {
			get { return (ManaCost)GetValue(ManaCostValueProperty); }
			set { SetValue(ManaCostValueProperty, value); }
		}
		public static readonly DependencyProperty ManaCostValueProperty = DependencyProperty.Register("ManaCostValue", typeof(ManaCost), typeof(ManaCostTextBlock),
			new PropertyMetadata(null, OnManaCostValueChanged));
		protected static void OnManaCostValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManaCostTextBlock _this = (ManaCostTextBlock)d;
			ManaCost newVal = (ManaCost)e.NewValue;
			_this.displayText = newVal.ToString();
			UpdateImageInlines(_this, newVal);
			if (!_this.Editing)//This condition prevents an endless recursive property setting loop.
				RefreshViewContents(_this);
		}

		public ManaCostTextBlockDisplayModes ManaCostDisplayMode { get; private set; }

		private List<InlineUIContainer> displayImageInlines = new List<InlineUIContainer>();
		private string displayText;
		private void SwitchToText() {
			this.Inlines.Clear();
			this.Text = displayText;
			ManaCostDisplayMode = ManaCostTextBlockDisplayModes.Text;
		}
		private void SwitchToImages() {
			this.Inlines.Clear();
			this.Text = string.Empty;
			this.Inlines.AddRange(displayImageInlines);
			ManaCostDisplayMode = ManaCostTextBlockDisplayModes.Image;
		}

		public bool Editing { get; private set; }

		public ManaSymbolSet ManaSymbolSet {
			get { return (ManaSymbolSet)GetValue(ManaSymbolSetProperty); }
			set { SetValue(ManaCostValueProperty, value); }
		}
		public static readonly DependencyProperty ManaSymbolSetProperty = DependencyProperty.Register("ManaSymbolSet", typeof(ManaSymbolSet), typeof(ManaCostTextBlock),
			new PropertyMetadata(ManaSymbolSet.Default, OnManaSymbolSetChanged, (d, baseVal) => (baseVal == null ? ((ManaCostTextBlock)d).ManaSymbolSet : baseVal)));
		protected static void OnManaSymbolSetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ManaCostTextBlock _this = (ManaCostTextBlock)d;
			UpdateImageInlines(_this, _this.ManaCostValue);
			if (!_this.Editing)//This condition prevents an endless recursive property setting loop.
				RefreshViewContents(_this);
		}

		protected static void UpdateImageInlines(ManaCostTextBlock _this, ManaCost newVal) {
			_this.displayImageInlines.Clear();
			IEnumerator<ManaCostItem> valItems = newVal.GetCostItemsEnumerator();
			while (valItems.MoveNext()) {
				ImageSource unitImgSrc = _this.ManaSymbolSet.GetManaSymbolByCode(valItems.Current.SingleUnitImageCode).ImageBitmap;
				if (unitImgSrc == null) continue;
				for (int i = 0; i < valItems.Current.DuplicationQuantity; i++) {
					Image image = new Image();
					image.Source = unitImgSrc;
					image.Height = _this.LineHeight;
					image.Stretch = System.Windows.Media.Stretch.Uniform;
					InlineUIContainer cont = new InlineUIContainer(image);
					cont.BaselineAlignment = BaselineAlignment.Baseline;
					_this.displayImageInlines.Add(cont);
				}
			}
		}

		protected static void RefreshViewContents(ManaCostTextBlock _this) {
			if (_this.ManaCostDisplayMode == ManaCostTextBlockDisplayModes.Image)
				_this.SwitchToImages();
			else if (_this.ManaCostDisplayMode == ManaCostTextBlockDisplayModes.Text)
				_this.SwitchToText();
		}
	}
}
