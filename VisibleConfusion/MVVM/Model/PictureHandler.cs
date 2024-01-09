using System;
using System.Threading.Tasks;
using System.Windows;
using Windows.Graphics;
using ABI.Windows.Media.Audio;
using Emgu.CV;
using Emgu.CV.Structure;
using Xceed.Wpf.Toolkit.Core.Converters;
using Point = Windows.Foundation.Point;
using Size = Windows.Foundation.Size;

namespace VisibleConfusion.MVVM.Model
{
	internal class PictureHandler
	{
		public class Point2D
		{
			public Int32 X { get; set; }

			public Int32 Y { get; set; }

			public Point2D() : this(0, 0)
			{ }
			public Point2D(Int32 x, Int32 y)
			{
				X = x;
				Y = y;
			}
		}

		public class Color
		{
			public byte R { get; set; }
			public byte G { get; set; }
			public byte B { get; set; }

			public Color() : this(0, 0, 0)
			{ }
			public Color(Int32 r, Int32 g, Int32 b) : this(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b))
			{ }
			public Color(double r, double g, double b) : this(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b))
			{ }
			public Color(Rgb rgb) : this(Convert.ToByte(rgb.Red), Convert.ToByte(rgb.Green), Convert.ToByte(rgb.Blue))
			{ }
			public Color(byte r, byte g, byte b)
			{
				R = r;
				G = g;
				B = b;
			}

			public Rgb ToRgb()
			{
				return new Rgb(R, G, B);
			}
		}

		public Size? FrameSize { get; set; }
		public Size? GraphSize { get; set; }

		public delegate void FrameChangedEventHandler(Image<Rgb, byte>? sender);
		public event FrameChangedEventHandler? FrameChanged;
		public event FrameChangedEventHandler? GraphChanged;

		private Image<Rgb, byte>? _currentFrame;
		public Image<Rgb, byte>? CurrentFrame
		{
			get => _currentFrame;
			private set
			{
				_currentFrame?.Dispose();
				_currentFrame = value;
				LastFrameTimeStamp = DateTime.Now;
				OnFrameChanged();
			}
		}
		public DateTime LastFrameTimeStamp { get; private set; }


		private Image<Rgb, byte>? _currentGraph;

		public Image<Rgb, byte>? CurrentGraph
		{
			get => _currentGraph;
			private set
			{
				_currentGraph?.Dispose();
				_currentGraph = value;
				OnGraphChanged();
			}
		}

		VideoCapture? _capture;
		public bool IsCaptureRunning { get; private set; }

		public PictureHandler()
		{
			ClearFrame();
		}

		private void OnFrameChanged()
		{
			FrameChanged?.Invoke(CurrentFrame);
			CreateGraphFromFrame();
		}

		private void OnGraphChanged()
		{
			GraphChanged?.Invoke(CurrentGraph);
		}

		private void CreateGraphFromFrame()
		{
			var tmp = new Image<Rgb, byte>(CurrentFrame?.Width ?? 1, 256, new Rgb(System.Drawing.Color.Black));
			var frameY = CurrentFrame?.Height / 2 ?? 0;
			int lastRVal = 0, lastGVal = 0, lastBVal = 0;
			for (var x = 0; x < CurrentFrame?.Width; x++)
			{
				var color = GetPixelFromPos(new Point2D(x, frameY));
				var rVal = 255 - color.R;
				var gVal = 255 - color.G;
				var bVal = 255 - color.B;

				FillLineWithColor(ref tmp, x, rVal, lastRVal, 0, System.Drawing.Color.Red.R);
				FillLineWithColor(ref tmp, x, gVal, lastGVal, 1, System.Drawing.Color.Green.G);
				FillLineWithColor(ref tmp, x, bVal, lastBVal, 2, System.Drawing.Color.Blue.B);

				lastRVal = rVal;
				lastGVal = gVal;
				lastBVal = bVal;
			}

			CurrentGraph = tmp;
		}

		private void FillLineWithColor(ref Image<Rgb, byte> image, int x, int yBegin, int yEnd, int z, byte val)
		{
			if (yBegin > yEnd)
				(yBegin, yEnd) = (yEnd, yBegin);

			for (var y = yBegin; y < yEnd; y++)
			{
				image.Data[y, x, z] = val;
			}
		}

		public void ClearFrame()
		{
			CurrentFrame = new Image<Rgb, byte>(1024, 768, new Rgb(System.Drawing.Color.Black));
		}

		public void GetImageFromFile(Uri uri)
		{
			CurrentFrame = new Image<Rgb, byte>(uri.AbsolutePath);
		}

		public Task<bool> ToggleCameraFeedAsync()
		{
			if (_capture == null)
				return Task.Run(CreateCapture);

			if (LastFrameTimeStamp > DateTime.Now - TimeSpan.FromSeconds(0.5) & IsCaptureRunning)
			{
				_capture.Stop();
				IsCaptureRunning = false;
			}
			else if (!IsCaptureRunning)
			{
				_capture.Start();
				IsCaptureRunning = true;
			}
			else
			{
				return Task.Run(CreateCapture);
			}

			return Task.Run(() => true);
		}

		private bool CreateCapture()
		{
			_capture?.Dispose();
			_capture = new VideoCapture();
			if (!_capture.IsOpened) return _capture.IsOpened;
			_capture.ImageGrabbed += OnImageGrabbed;
			_capture.Start();
			IsCaptureRunning = true;
			return true;
		}

		private void OnImageGrabbed(object? sender, EventArgs args)
		{
			if (_capture == null) return;
			var frame = new Image<Bgr, byte>(_capture.Width, _capture.Height);
			_capture?.Retrieve(frame);
			Application.Current?.Dispatcher.Invoke(() => CurrentFrame = frame.Convert<Rgb, byte>());
		}

		public Point2D GetFrameRelativePos(Point relPoint)
		{
			var absPoint = new Point2D(
				Int32.Clamp(Convert.ToInt32(relPoint.X * CurrentFrame?.Width), 0, CurrentFrame?.Width - 1 ?? 0),
				Int32.Clamp(Convert.ToInt32(relPoint.Y * CurrentFrame?.Height), 0, CurrentFrame?.Height - 1 ?? 0));
			return absPoint;
		}

		public Color GetPixelFromPos(Point2D point)
		{
			var safeX = Int32.Clamp(point.X, 0, CurrentFrame?.Width - 1 ?? 0);
			var safeY = Int32.Clamp(point.Y, 0, CurrentFrame?.Height - 1 ?? 0);
			var rgb = CurrentFrame?[safeY, safeX] ?? new Rgb(System.Drawing.Color.Black);
			return new Color(rgb);
		}
	}
}
