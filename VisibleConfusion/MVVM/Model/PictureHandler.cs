using System;
using System.IO;
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

			_currentFrame = Properties.Resources.lama1080.ToImage<Rgb, byte>();
			_currentGraph = Properties.Resources.panda.ToImage<Rgb, byte>();
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
			using var tmp = new Image<Rgb, byte>(CurrentFrame?.Width ?? 1, 256, new Rgb(System.Drawing.Color.Black));
			var frameY = CurrentFrame?.Height / 2 ?? 0;

			var color = GetPixelFromPos(new Point2D(0, frameY));
			int lastRVal = 255 - color.R, lastGVal = 255 - color.G, lastBVal = 255 - color.B;

			for (var x = 0; x < CurrentFrame?.Width; x++)
			{
				color = GetPixelFromPos(new Point2D(x, frameY));
				var rVal = 255 - color.R;
				var gVal = 255 - color.G;
				var bVal = 255 - color.B;

				FillLineWithColor(tmp, x, rVal, lastRVal + 1, 0, System.Drawing.Color.Red.R);
				FillLineWithColor(tmp, x, gVal, lastGVal + 1, 1, System.Drawing.Color.Lime.G);
				FillLineWithColor(tmp, x, bVal, lastBVal + 1, 2, System.Drawing.Color.Blue.B);

				lastRVal = rVal;
				lastGVal = gVal;
				lastBVal = bVal;
			}

			if (CurrentGraph?.Width != tmp.Width || CurrentGraph?.Height != tmp.Height)
				CurrentGraph = tmp.Copy();
			else
			{
				tmp.CopyTo(CurrentGraph);
				OnGraphChanged();
			}
		}

		private void FillLineWithColor(Image<Rgb, byte> image, int x, int yBegin, int yEnd, int z, byte val)
		{
			if (yBegin > yEnd)
				(yBegin, yEnd) = (yEnd, yBegin);

			for (var y = yBegin; y <= yEnd && y < image.Height; y++)
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
			if (!File.Exists(uri.AbsolutePath))
			{
				MessageBox.Show("File does not exist in this context.");
				return;
			}

			try
			{
				CurrentFrame = new Image<Rgb, byte>(uri.AbsolutePath);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
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

		public void SetPixelAtPos(Point2D point, Color color)
		{
			var safeX = Int32.Clamp(point.X, 0, CurrentFrame?.Width - 1 ?? 0);
			var safeY = Int32.Clamp(point.Y, 0, CurrentFrame?.Height - 1 ?? 0);
			if (CurrentFrame != null) CurrentFrame[safeY, safeX] = color.ToRgb();
			OnFrameChanged();
		}

		public void SetDotAtPos(Point2D point, Color color, int radius)
		{
			var safeX = Int32.Clamp(point.X, 0, CurrentFrame?.Width - 1 ?? 0);
			var safeY = Int32.Clamp(point.Y, 0, CurrentFrame?.Height - 1 ?? 0);
			if (CurrentFrame != null) CurrentFrame.Draw(new CircleF(new System.Drawing.Point(safeX, safeY), radius), color.ToRgb(), -1);
			OnFrameChanged();
		}

		public void SetFrame(Image<Rgb, byte> frame, Rgb filterColor, bool toGrayscale = false)
		{
			CurrentFrame = toGrayscale ? ToGrayscale(frame) : OnlyPassColor(frame, filterColor);
		}

		private Image<Rgb, byte> ToGrayscale(Image<Rgb, byte> frame)
		{
			for (int i = 0; i < frame.Height; i++)
			{
				for (int j = 0; j < frame.Width; j++)
				{
					var color = frame[i, j];
					var gray = (color.Red + color.Green + color.Blue) / 3;
					frame[i, j] = new Rgb(gray, gray, gray);
				}
			}
			return frame;
		}

		private Image<Rgb, byte> OnlyPassColor(Image<Rgb, byte> frame, Rgb filterColor)
		{
			for (int i = 0; i < frame.Height; i++)
			{
				for (int j = 0; j < frame.Width; j++)
				{
					var color = frame[i, j];
					color.Red = Double.Clamp(color.Red, 0, filterColor.Red);
					color.Green = Double.Clamp(color.Green, 0, filterColor.Green);
					color.Blue = Double.Clamp(color.Blue, 0, filterColor.Blue);
					frame[i, j] = color;
				}
			}
			return frame;
		}

		public void ZoomIn(Point2D point)
		{
			if (CurrentFrame?.Data == null)
				return;

			var newWidth = Convert.ToInt32(CurrentFrame?.Width * (0.25f));
			var newHeight = Convert.ToInt32(CurrentFrame?.Height * (0.25f));

			if (point.X + newWidth / 2 >= CurrentFrame?.Width)
				point.X = CurrentFrame.Width - newWidth / 2 - 1;
			if (point.X - newWidth / 2 < 0)
				point.X = newWidth / 2;
			if (point.Y + newHeight / 2 >= CurrentFrame?.Height)
				point.Y = CurrentFrame.Height - newHeight / 2 - 1;
			if (point.Y - newHeight / 2 < 0)
				point.Y = newHeight / 2;

			var newFrame = new Image<Rgb, byte>(newWidth + 1, newHeight + 1);

			for (int i = 0; i < newFrame.Height; i++)
			{
				for (int j = 0; j < newFrame.Width; j++)
				{
					newFrame[i, j] = CurrentFrame![point.Y + i - newHeight / 2, point.X + j - newWidth / 2];
				}
			}
			CurrentFrame?.Dispose();
			CurrentFrame = newFrame;
			OnFrameChanged();
		}
	}
}
