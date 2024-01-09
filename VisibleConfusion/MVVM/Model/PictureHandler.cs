using System;
using Windows.Foundation;
using Windows.Graphics;
using Emgu.CV;
using Emgu.CV.Structure;
using Xceed.Wpf.Toolkit.Core.Converters;

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
				OnFrameChanged();
			}
		}

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
			CurrentGraph = new Image<Rgb, byte>(100, 100, new Rgb(System.Drawing.Color.Black));
		}

		public void ClearFrame()
		{
			CurrentFrame = new Image<Rgb, byte>(1, 1, new Rgb(System.Drawing.Color.Black));
		}

		public void GetImageFromFile(Uri uri)
		{
			CurrentFrame = new Image<Rgb, byte>(uri.AbsolutePath);
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
