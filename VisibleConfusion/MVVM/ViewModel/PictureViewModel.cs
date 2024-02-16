using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using VisibleConfusion.Core;
using VisibleConfusion.MVVM.Model;
using Windows.Graphics;
using Emgu.CV.CvEnum;
using Point = Windows.Foundation.Point;

namespace VisibleConfusion.MVVM.ViewModel
{
	internal class PictureViewModel : ObservableObject
	{
		private PictureHandler _pictureHandler;
		public Image<Rgb, byte>? CurrentFrame => _pictureHandler.CurrentFrame;

		public RelayCommand? DrawByHandCommand { get; set; }
		public RelayCommand? FromFileCommand { get; set; }
		public RelayCommand? FromCameraCommand { get; set; }
		public RelayCommand? CleanCommand { get; set; }
		public RelayCommand? DoGraphCommand { get; set; }
		public RelayCommand? OnMouseLeftButtonDownOnPictureCommand { get; set; }
		public RelayCommand? OnMouseMoveOnPictureCommand { get; set; }
		public RelayCommand? OnNumericUpDownPixelPosChangedCommand { get; set; }

		private PictureHandler.Point2D _selectedPixelPos;

		public PictureHandler.Point2D SelectedPixelPos
		{
			get => _selectedPixelPos;
			set
			{
				_selectedPixelPos = value;
				OnPropertyChanged();
				OnSelectedPixelPosChanged();
			}
		}

		private PictureHandler.Color _selecctedColor;

		public PictureHandler.Color SelectedColor
		{
			get => _selecctedColor;
			set
			{
				_selecctedColor = value;
				OnPropertyChanged();
			}
		}

		private PictureHandler.Point2D _maxPixelPos;

		public PictureHandler.Point2D MaxPixelPos
		{
			get => _maxPixelPos;
			set
			{
				_maxPixelPos = value;
				OnPropertyChanged();
			}
		}

		private bool _cameraButtonEnabled;

		public bool CameraButtonEnabled
		{
			get => _cameraButtonEnabled;
			set
			{
				_cameraButtonEnabled = value;
				OnPropertyChanged();
			}
		}

		private bool _drawingByHandEnabled = false;

		public bool DrawingByHandEnabled
		{
			get => _drawingByHandEnabled;
			set
			{
				_drawingByHandEnabled = value;
				OnPropertyChanged();
			}
		}

		private WriteableBitmap? _pictureBitmap;

		public WriteableBitmap? PictureBitmap
		{
			get => _pictureBitmap;
			private set
			{
				_pictureBitmap = value;
				OnPropertyChanged();
			}
		}

		private WriteableBitmap? _graphBitmap;

		public WriteableBitmap? GraphBitmap
		{
			get => _graphBitmap;
			private set
			{
				_graphBitmap = value;
				OnPropertyChanged();
			}
		}


		public PictureViewModel()
		{
			_pictureHandler = new PictureHandler();
			// PictureBitmapSource = _pictureHandler.CurrentFrame?.Convert<Bgr, byte>().ToBitmapSource();
			_pictureHandler.FrameChanged += SetWriteablePictureBitmap;
			// _pictureHandler.FrameChanged += (sender) => PictureBitmapSource = sender?.Convert<Bgr, byte>().ToBitmapSource();
			_pictureHandler.FrameChanged += (_) => OnFrameChanged();
			_pictureHandler.GraphChanged += SetWriteableGraphBitmap;

			_selectedPixelPos = _pictureHandler.GetFrameRelativePos(new Point(0, 0));
			_maxPixelPos = _pictureHandler.GetFrameRelativePos(new Point(1, 1));
			_selecctedColor = _pictureHandler.GetPixelFromPos(SelectedPixelPos);

			SetWriteablePictureBitmap(_pictureHandler.CurrentFrame);
			SetWriteableGraphBitmap(_pictureHandler.CurrentGraph);

			DrawByHandCommand = new RelayCommand((o) => DrawingByHandEnabled = !DrawingByHandEnabled);
			FromFileCommand = new RelayCommand(FromFile);
			FromCameraCommand = new RelayCommand(FromCamera);
			CleanCommand = new RelayCommand((o) => _pictureHandler.ClearFrame());
			DoGraphCommand = new RelayCommand((o) => throw new NotImplementedException());
			OnMouseLeftButtonDownOnPictureCommand = new RelayCommand(MouseLeftButtonDownOnPicture);
			OnMouseMoveOnPictureCommand = new RelayCommand(MouseMoveOnPicture);
			OnNumericUpDownPixelPosChangedCommand = new RelayCommand((o) => OnSelectedPixelPosChanged());

			CameraButtonEnabled = true;
		}

		private void SetWriteablePictureBitmap(Image<Rgb, byte>? picture)
		{
			if (picture?.Data == null)
				return;

			if (PictureBitmap == null || PictureBitmap.PixelWidth != picture.Width || PictureBitmap.PixelHeight != picture.Height)
				PictureBitmap = new WriteableBitmap(picture.Width, picture.Height, 96, 96, PixelFormats.Rgb24, null);
			var rect = new Int32Rect(0, 0, picture.Width, picture.Height);
			PictureBitmap?.WritePixels(rect, picture.Bytes, picture.Width * 3, 0);
		}

		private void SetWriteableGraphBitmap(Image<Rgb, byte>? picture)
		{
			if (picture?.Data == null)
				return;

			if (GraphBitmap == null || GraphBitmap.PixelWidth != picture.Width || GraphBitmap.PixelHeight != picture.Height)
				GraphBitmap = new WriteableBitmap(picture.Width, picture.Height, 96, 96, PixelFormats.Rgb24, null);
			GraphBitmap?.WritePixels(new Int32Rect(0, 0, picture.Width, picture.Height), picture.Bytes, picture.Width * 3, 0);
		}

		public void SetPicture(Image<Rgb, byte>? picture, Rgb? filterColor, bool? toGrayscale)
		{
			if (picture?.Data == null)
				return;
			var newPicture = picture.Clone();
			_pictureHandler.SetFrame(newPicture, filterColor ?? new Rgb(System.Drawing.Color.White), toGrayscale ?? false);
		}

		private void OnFrameChanged()
		{
			if (!DrawingByHandEnabled)
				SelectedColor = _pictureHandler.GetPixelFromPos(_selectedPixelPos);

			MaxPixelPos = _pictureHandler.GetFrameRelativePos(new Point(1, 1));
		}

		private void MouseMoveOnPicture(object obj)
		{
			if (obj is not MouseEventArgs mouseEventArgs) return;
			if (mouseEventArgs.LeftButton != MouseButtonState.Pressed) return;
			if (mouseEventArgs.OriginalSource is not Image actualImage) return;
			var point = mouseEventArgs.GetPosition(actualImage);
			SelectedPixelPos = _pictureHandler.GetFrameRelativePos(new Point(point.X / actualImage.ActualWidth, point.Y / actualImage.ActualHeight));
		}

		private void MouseLeftButtonDownOnPicture(object obj)
		{
			if (obj is not MouseButtonEventArgs mouseEventArgs) return;
			if (mouseEventArgs.OriginalSource is not Image actualImage) return;
			var point = mouseEventArgs.GetPosition(actualImage);
			SelectedPixelPos = _pictureHandler.GetFrameRelativePos(new Point(point.X / actualImage.ActualWidth, point.Y / actualImage.ActualHeight));
		}

		private void FromFile(object obj)
		{
			var openFileDialog = new OpenFileDialog
			{
				Multiselect = false,
				CheckPathExists = true,
				CheckFileExists = true,
				Filter = "Image files (*.jpg;*.jpeg)|*.jpg;*.jpeg|All files (*.*)|*.*"
			};

			if (openFileDialog.ShowDialog() != true) return;
			_pictureHandler.GetImageFromFile(new Uri(openFileDialog.FileName));
		}

		private async void FromCamera(object obj)
		{
			CameraButtonEnabled = false;
			if (!(await _pictureHandler.ToggleCameraFeedAsync()))
				SetPicture(Properties.Resources.cameraWhere.ToImage<Rgb, byte>(), null, null);
			CameraButtonEnabled = true;
		}

		private void OnSelectedPixelPosChanged()
		{
			if (DrawingByHandEnabled)
				_pictureHandler.SetDotAtPos(_selectedPixelPos, _selecctedColor, 3);
			else
				SelectedColor = _pictureHandler.GetPixelFromPos(_selectedPixelPos);
		}
	}
}
