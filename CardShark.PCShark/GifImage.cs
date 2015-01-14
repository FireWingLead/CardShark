using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CardShark.PCShark
{
	public class GifImage : Image
	{
		private bool _isInitialized;
		private GifBitmapDecoder _gifDecoder;
		private Int32Animation _animation;
		private bool _runningInvis = true;//True if the animation hasn't been stopped by an external entity.

		public GifImage()
			: base() {
				this.Loaded += GifImage_Loaded;
		}
		void GifImage_Loaded(object sender, RoutedEventArgs e) {
			if (GifSource != null && AutoStart) {
				StartAnimation();
			}
		}

		private void Initialize() {
			if (this.GifSource == null) {
				InternalStopAnimation();
				_gifDecoder = null;
				_animation = null;
				_isInitialized = false;
			}
			InitImage();
			InitAnimation();
			_isInitialized = true;
		}
		private void InitImage() {
			_gifDecoder = new GifBitmapDecoder(GifSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
			this.Source = _gifDecoder.Frames[0];
		}
		private void InitAnimation() {
			_animation = new Int32Animation(0, _gifDecoder.Frames.Count - 1, new Duration(new TimeSpan((long)(((double)_gifDecoder.Frames.Count / this.FPS) * TimeSpan.TicksPerSecond))));
			_animation.RepeatBehavior = RepeatBehavior.Forever;
		}

		static GifImage() {
			VisibilityProperty.OverrideMetadata(typeof(GifImage),
				new FrameworkPropertyMetadata(VisibilityChanged));
		}

		private static void VisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			GifImage this_ = sender as GifImage;
			if ((Visibility)e.NewValue == Visibility.Visible && (this_._runningInvis || this_.AutoStart))
				this_.StartAnimation();
			else
				this_.InternalStopAnimation();
		}

		public static readonly DependencyProperty FrameIndexProperty = DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage),
			new UIPropertyMetadata(0, ChangingFrameIndex));
		public int FrameIndex {
			get { return (int)GetValue(FrameIndexProperty); }
			set { SetValue(FrameIndexProperty, value); }
		}
		static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
			GifImage this_ = obj as GifImage;
			this_.Source = this_._gifDecoder.Frames[(int)args.NewValue];
		}

		/// <summary>
		/// Defines whether the animation starts on it's own
		/// </summary>
		public static readonly DependencyProperty AutoStartProperty = DependencyProperty.Register("AutoStart", typeof(bool), typeof(GifImage),
			new UIPropertyMetadata(true, AutoStartPropertyChanged));
		public bool AutoStart {
			get { return (bool)GetValue(AutoStartProperty); }
			set { SetValue(AutoStartProperty, value); }
		}
		private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			GifImage this_ = sender as GifImage;
			if ((bool)e.NewValue && this_.IsVisible)
				this_.StartAnimation();
		}

		public static readonly DependencyProperty GifSourceProperty = DependencyProperty.Register("GifSource", typeof(Uri), typeof(GifImage),
			new UIPropertyMetadata(null, GifSourcePropertyChanged));
		public Uri GifSource {
			get { return (Uri)GetValue(GifSourceProperty); }
			set { SetValue(GifSourceProperty, value); }
		}
		private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			GifImage this_ = sender as GifImage;
			this_.Initialize();
			if (this_.GifSourceUpdated != null)
				this_.GifSourceUpdated(this_, new EventArgs());
		}
		public event EventHandler GifSourceUpdated;

		public static readonly DependencyProperty FPSProperty = DependencyProperty.Register("FPS", typeof(double), typeof(GifImage),
			new PropertyMetadata(10.0, FPSChanged));
		public double FPS {
			get { return (double)GetValue(FPSProperty); }
			set { SetValue(FPSProperty, value); }
		}
		private static void FPSChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			GifImage this_ = sender as GifImage;
			if (this_._gifDecoder == null) return;
			this_._animation.Duration = new Duration(new TimeSpan((long)(((double)this_._gifDecoder.Frames.Count / (double)e.NewValue) * TimeSpan.TicksPerSecond)));
		}

		/// <summary>
		/// Starts the animation
		/// </summary>
		public void StartAnimation() {
			if (!_isInitialized) this.Initialize();
			BeginAnimation(FrameIndexProperty, _animation);
			this._runningInvis = true;
		}

		/// <summary>
		/// Stops the animation
		/// </summary>
		public void StopAnimation() {
			BeginAnimation(FrameIndexProperty, null);
			if (!AutoStart) this._runningInvis = false;
		}
		private void InternalStopAnimation() {
			BeginAnimation(FrameIndexProperty, null);
		}
	}
}
