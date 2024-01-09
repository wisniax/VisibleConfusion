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
using Point = Windows.Foundation.Point;

namespace VisibleConfusion.MVVM.ViewModel
{
	internal class PictureViewModel : ObservableObject
	{
		private BitmapSource? _pictureBitmapSource;

		public BitmapSource? PictureBitmapSource
		{
			get => _pictureBitmapSource;
			private set
			{
				_pictureBitmapSource = value;
				OnPropertyChanged();
			}
		}

		private BitmapSource? _graphBitmapSource;

		public BitmapSource? GraphBitmapSource
		{
			get => _graphBitmapSource;
			private set
			{
				_graphBitmapSource = value;
				OnPropertyChanged();
			}
		}

		private PictureHandler _pictureHandler;

		public RelayCommand? DrawByHandCommand { get; set; }
		public RelayCommand? FromFileCommand { get; set; }
		public RelayCommand? FromCameraCommand { get; set; }
		public RelayCommand? CleanCommand { get; set; }
		public RelayCommand? DoGraphCommand { get; set; }
		public RelayCommand? OnMouseLeftButtonDownOnPictureCommand { get; set; }
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



		public PictureViewModel()
		{
			_pictureHandler = new PictureHandler();
			// PictureBitmapSource = _pictureHandler.CurrentFrame?.Convert<Bgr, byte>().ToBitmapSource();
			_pictureHandler.FrameChanged += (sender) => PictureBitmapSource = sender?.Convert<Bgr, byte>().ToBitmapSource();
			// _pictureHandler.FrameChanged += (sender) => PictureBitmapSource = sender?.Convert<Bgr, byte>().ToBitmapSource();
			_pictureHandler.FrameChanged += (_) => MaxPixelPos = _pictureHandler.GetFrameRelativePos(new Point(1, 1));
			_pictureHandler.FrameChanged += (_) => OnSelectedPixelPosChanged();
			_pictureHandler.GraphChanged += (sender) => GraphBitmapSource = sender?.Convert<Bgr, byte>().ToBitmapSource();

			_selectedPixelPos = _pictureHandler.GetFrameRelativePos(new Point(0, 0));
			_maxPixelPos = _pictureHandler.GetFrameRelativePos(new Point(1, 1));
			_selecctedColor = _pictureHandler.GetPixelFromPos(SelectedPixelPos);

			_pictureBitmapSource = new BitmapImage(new Uri("https://zoowojciechow.pl/images/lama-p-1080.jpeg"));
			_graphBitmapSource = new BitmapImage(new Uri("https://i.gremicdn.pl/image/free/678943239052287d16ff4ae67c083de9/"));

			DrawByHandCommand = new RelayCommand((o) => throw new NotImplementedException());
			FromFileCommand = new RelayCommand(FromFile);
			FromCameraCommand = new RelayCommand(FromCamera);
			CleanCommand = new RelayCommand((o) => _pictureHandler.ClearFrame());
			DoGraphCommand = new RelayCommand((o) => throw new NotImplementedException());
			OnMouseLeftButtonDownOnPictureCommand = new RelayCommand(MouseLeftButtonDownOnPicture);
			OnNumericUpDownPixelPosChangedCommand = new RelayCommand((o) => OnSelectedPixelPosChanged());

			CameraButtonEnabled = true;
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
				Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
			};

			if (openFileDialog.ShowDialog() != true) return;
			_pictureHandler.GetImageFromFile(new Uri(openFileDialog.FileName));
		}

		private async void FromCamera(object obj)
		{
			CameraButtonEnabled = false;
			if (!(await _pictureHandler.ToggleCameraFeedAsync()))
				PictureBitmapSource = new BitmapImage(new Uri("http://www.quickmeme.com/img/8e/8ebf027f8f79f4b1b959341cb0a6f91bd28f6ec612dbc32c032712f8a6834a24.jpg"));
			CameraButtonEnabled = true;
		}

		private void OnSelectedPixelPosChanged()
		{
			SelectedColor = _pictureHandler.GetPixelFromPos(_selectedPixelPos);
		}
	}
}
