using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Color = System.Drawing.Color;

namespace VisibleConfusion.MVVM.Model
{
	internal class Picture
	{
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

		public Picture()
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
			CurrentGraph = new Image<Rgb, byte>(100, 100, new Rgb(Color.Black));
		}

		public void ClearFrame()
		{
			CurrentFrame = new Image<Rgb, byte>(100, 100, new Rgb(Color.Black));
		}

		public void GetImageFromFile(Uri uri)
		{
			CurrentFrame = new Image<Rgb, byte>(uri.AbsolutePath);
		}
	}
}
