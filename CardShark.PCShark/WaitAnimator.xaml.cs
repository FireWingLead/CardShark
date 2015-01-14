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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CardShark.PCShark
{
	/// <summary>
	/// Interaction logic for WaitAnimator.xaml
	/// </summary>
	public partial class WaitAnimator : UserControl
	{
		public WaitAnimator() {
			InitializeComponent();
		}
		public WaitAnimator(Control waitingCont) : this() {
			WaitingControl = waitingCont;
		}

		public static readonly DependencyProperty WaitingControlProperty = DependencyProperty.Register("WaitingControl", typeof(Control), typeof(WaitAnimator),
			new PropertyMetadata() { DefaultValue = null, PropertyChangedCallback = OnWaitingControlChanged });
		public Control WaitingControl {
			get { return (Control)GetValue(WaitingControlProperty); }
			set { SetValue(WaitingControlProperty, value); }
		}
		private static void OnWaitingControlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			WaitAnimator this_ = (WaitAnimator)sender;
			Control oldCont = (Control)e.OldValue, newCont = (Control)e.NewValue;
			detachWaitingCont(this_, oldCont);
			if (newCont == null) return;
			this_.Margin = new Thickness(newCont.Margin.Left, newCont.Margin.Top, newCont.Margin.Right, newCont.Margin.Bottom);
			this_.HorizontalAlignment = newCont.HorizontalAlignment;
			this_.VerticalAlignment = newCont.VerticalAlignment;
			Panel.SetZIndex(this_, Panel.GetZIndex(newCont) + 1);
			attachWaitingCont(this_, newCont);
			this_.UpdateLayoutToWaitingCont(newCont);
		}
		private static void attachWaitingCont(WaitAnimator sender, Control attach) {
			if (attach == null) return;
			attach.SizeChanged += sender.WaitingCont_SizeChanged;
		}
		private static void detachWaitingCont(WaitAnimator sender, Control detach) {
			if (detach == null) return;
			detach.SizeChanged -= sender.WaitingCont_SizeChanged;
		}
		void WaitingCont_SizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateLayoutToWaitingCont((Control)sender);
		}
		private void UpdateLayoutToWaitingCont(Control wCont) {
			this.Width = wCont.ActualWidth;
			this.Height = wCont.ActualHeight;
		}

		public static readonly DependencyProperty MaxIconSizeProperty = DependencyProperty.Register("MaxIconSize", typeof(Size), typeof(WaitAnimator),
			new PropertyMetadata() { DefaultValue = new Size(100, 100), PropertyChangedCallback = OnMaxIconSizeChanged });
		public Size MaxIconSize {
			get { return (Size)GetValue(MaxIconSizeProperty); }
			set {
				SetValue(MaxIconSizeProperty, value);
			}
		}
		private static void OnMaxIconSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			FitIconSizeToMaxIconSize((WaitAnimator)sender);
			UpdateIconLayout((WaitAnimator)sender);
		}

		public void StartWait() {
			this.Visibility = System.Windows.Visibility.Visible;
			this.IsHitTestVisible = true;
			WaitingControl.IsEnabled = false;
		}
		public void EndWait() {
			this.Visibility = System.Windows.Visibility.Collapsed;
			this.IsHitTestVisible = false;
			WaitingControl.IsEnabled = true;
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateIconLayout((WaitAnimator)sender);
		}

		private static void FitIconSizeToMaxIconSize(WaitAnimator sender) {
			//double iconW = Math.Max(WaitIcon.Source.Width, MaxIconSize.Width), iconH = Math.Max(WaitIcon.Source.Height, MaxIconSize.Height);
			//if (WaitIcon.Source.Width > WaitIcon.Source.Height) {
			//	iconH = iconW * (WaitIcon.Source.Height / WaitIcon.Source.Width);
			//}
			//else if (WaitIcon.Source.Height > WaitIcon.Source.Width) {
			//	iconW = iconH * (WaitIcon.Source.Width / WaitIcon.Source.Height);
			//}
			//WaitIcon.MaxWidth = iconW;
			//WaitIcon.MaxHeight = iconH;

			sender.WaitIcon.MaxWidth = sender.MaxIconSize.Width;
			sender.WaitIcon.MaxHeight = sender.MaxIconSize.Height;
		}
		private static void UpdateIconLayout(WaitAnimator sender) {
			double iconW = sender.WaitIcon.MaxWidth, iconH = sender.WaitIcon.MaxHeight;
			if (iconW > sender.Width) {
				iconW = sender.Width;
				//iconH = iconW * (sender.WaitIcon.Source.Height / sender.WaitIcon.Source.Width);
			}
			if (iconH > sender.Height) {
				iconH = sender.Height;
				//iconW = iconH * (sender.WaitIcon.Source.Width / sender.WaitIcon.Source.Height);
			}
			iconW = iconH = Math.Min(iconW, iconH);
			//double iconX = sender.Width / 2 - iconW / 2;
			//double iconY = sender.Height / 2 - iconH / 2;
			sender.WaitIcon.Width = iconW;
			sender.WaitIcon.Height = iconH;
		}

		private void WaitIcon_GifSourceUpdated(object sender, EventArgs e) {

		}
	}
}
