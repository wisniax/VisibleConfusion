using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using VisibleConfusion.Core;
using VisibleConfusion.MVVM.Model;

namespace VisibleConfusion.MVVM.ViewModel
{
	internal class Task2ViewModel : ObservableObject
	{

		private ImageFiltrations _imageFiltrations = new ImageFiltrations();

		private PictureViewModel _leftPictureViewModel;
		private PictureViewModel _rightPictureViewModel;
		private WriteableBitmap? _buffor1Bitmap;
		private WriteableBitmap? _buffor2Bitmap;
		private WriteableBitmap? _buffor3Bitmap;

		public PictureViewModel LeftPictureViewModel
		{
			get => _leftPictureViewModel;
			set
			{
				_leftPictureViewModel = value;
				OnPropertyChanged();
			}
		}

		public PictureViewModel RightPictureViewModel
		{
			get => _rightPictureViewModel;
			set
			{
				_rightPictureViewModel = value;
				OnPropertyChanged();
			}
		}

		public ImageFiltrations.BitwiseOperation BitwiseOperation { get; set; } = ImageFiltrations.BitwiseOperation.And;

		public RelayCommand? LeftImageToBufor1Command { get; private set; }
		public RelayCommand? RightImageToBufor1Command { get; private set; }
		public RelayCommand? Bufor1ImageToLeftImageCommand { get; private set; }
		public RelayCommand? Bufor1ImageToRightImageCommand { get; private set; }

		public RelayCommand? LeftImageToBufor2Command { get; private set; }
		public RelayCommand? RightImageToBufor2Command { get; private set; }
		public RelayCommand? Bufor2ImageToLeftImageCommand { get; private set; }
		public RelayCommand? Bufor2ImageToRightImageCommand { get; private set; }

		public RelayCommand? LeftImageToBufor3Command { get; private set; }
		public RelayCommand? RightImageToBufor3Command { get; private set; }
		public RelayCommand? Bufor3ImageToLeftImageCommand { get; private set; }
		public RelayCommand? Bufor3ImageToRightImageCommand { get; private set; }

		public RelayCommand? ButtonAndCommand { get; private set; }
		public RelayCommand? ButtonOrCommand { get; private set; }
		public RelayCommand? ButtonXorCommand { get; private set; }
		public RelayCommand? ButtonDoBitwiseOperationToBuf3 { get; private set; }

		public WriteableBitmap? Buffor1Bitmap
		{
			get => _buffor1Bitmap;
			set
			{
				if (Equals(value, _buffor1Bitmap)) return;
				_buffor1Bitmap = value;
				OnPropertyChanged();
			}
		}

		public WriteableBitmap? Buffor2Bitmap
		{
			get => _buffor2Bitmap;
			set
			{
				if (Equals(value, _buffor2Bitmap)) return;
				_buffor2Bitmap = value;
				OnPropertyChanged();
			}
		}

		public WriteableBitmap? Buffor3Bitmap
		{
			get => _buffor3Bitmap;
			set
			{
				if (Equals(value, _buffor3Bitmap)) return;
				_buffor3Bitmap = value;
				OnPropertyChanged();
			}
		}

		public Task2ViewModel()
		{
			_imageFiltrations.ImagesChanged += On_imageFiltrations_ImagesChanged;

			LeftPictureViewModel = new PictureViewModel();
			RightPictureViewModel = new PictureViewModel();

			if (LeftPictureViewModel.PictureBitmap != null)
				LeftPictureViewModel.LocalPictureHandler.FrameChanged += OnLeftPictureChanged;
			if (RightPictureViewModel.PictureBitmap != null)
				RightPictureViewModel.LocalPictureHandler.FrameChanged += OnRightPictureChanged;

			OnLeftPictureChanged(null);
			OnRightPictureChanged(null);

			ButtonAndCommand = new RelayCommand((_) => BitwiseOperation = ImageFiltrations.BitwiseOperation.And);
			ButtonOrCommand = new RelayCommand((_) => BitwiseOperation = ImageFiltrations.BitwiseOperation.Or);
			ButtonXorCommand = new RelayCommand((_) => BitwiseOperation = ImageFiltrations.BitwiseOperation.Xor);

			LeftImageToBufor1Command = new RelayCommand((_) => _imageFiltrations.MoveImage(ImageFiltrations.ViewPoint.LeftImage, ImageFiltrations.ViewPoint.Buffer1Image));
			RightImageToBufor1Command = new RelayCommand((_) => _imageFiltrations.MoveImage(ImageFiltrations.ViewPoint.RightImage, ImageFiltrations.ViewPoint.Buffer1Image));
			LeftImageToBufor2Command = new RelayCommand((_) => _imageFiltrations.MoveImage(ImageFiltrations.ViewPoint.LeftImage, ImageFiltrations.ViewPoint.Buffer2Image));
			RightImageToBufor2Command = new RelayCommand((_) => _imageFiltrations.MoveImage(ImageFiltrations.ViewPoint.RightImage, ImageFiltrations.ViewPoint.Buffer2Image));
			LeftImageToBufor3Command = new RelayCommand((_) => _imageFiltrations.MoveImage(ImageFiltrations.ViewPoint.LeftImage, ImageFiltrations.ViewPoint.Buffer3Image));
			RightImageToBufor3Command = new RelayCommand((_) => _imageFiltrations.MoveImage(ImageFiltrations.ViewPoint.RightImage, ImageFiltrations.ViewPoint.Buffer3Image));

			Bufor1ImageToLeftImageCommand = new RelayCommand((_) => _leftPictureViewModel?.SetPicture(_imageFiltrations.Images[ImageFiltrations.ViewPoint.Buffer1Image], null, null));
			Bufor1ImageToRightImageCommand = new RelayCommand((_) => _rightPictureViewModel?.SetPicture(_imageFiltrations.Images[ImageFiltrations.ViewPoint.Buffer1Image], null, null));
			Bufor2ImageToLeftImageCommand = new RelayCommand((_) => _leftPictureViewModel?.SetPicture(_imageFiltrations.Images[ImageFiltrations.ViewPoint.Buffer2Image], null, null));
			Bufor2ImageToRightImageCommand = new RelayCommand((_) => _rightPictureViewModel?.SetPicture(_imageFiltrations.Images[ImageFiltrations.ViewPoint.Buffer2Image], null, null));
			Bufor3ImageToLeftImageCommand = new RelayCommand((_) => _leftPictureViewModel?.SetPicture(_imageFiltrations.Images[ImageFiltrations.ViewPoint.Buffer3Image], null, null));
			Bufor3ImageToRightImageCommand = new RelayCommand((_) => _rightPictureViewModel?.SetPicture(_imageFiltrations.Images[ImageFiltrations.ViewPoint.Buffer3Image], null, null));
			
			ButtonDoBitwiseOperationToBuf3 = new RelayCommand((_) => _imageFiltrations.ApplyBitwiseOperation(BitwiseOperation));

		}

		private void On_imageFiltrations_ImagesChanged(Dictionary<ImageFiltrations.ViewPoint, Image<Rgb, byte>?> images)
		{
			SetBuffor1Bitmap(images[ImageFiltrations.ViewPoint.Buffer1Image]);
			SetBuffor2Bitmap(images[ImageFiltrations.ViewPoint.Buffer2Image]);
			SetBuffor3Bitmap(images[ImageFiltrations.ViewPoint.Buffer3Image]);
		}

		private void SetBuffor1Bitmap(Image<Rgb, byte>? picture)
		{
			if (picture?.Data == null)
				return;

			if (Buffor1Bitmap == null || Buffor1Bitmap.PixelWidth != picture.Width || Buffor1Bitmap.PixelHeight != picture.Height)
				Buffor1Bitmap = new WriteableBitmap(picture.Width, picture.Height, 96, 96, PixelFormats.Rgb24, null);
			var rect = new Int32Rect(0, 0, picture.Width, picture.Height);
			Buffor1Bitmap?.WritePixels(rect, picture.Bytes, picture.Width * 3, 0);
		}

		private void SetBuffor2Bitmap(Image<Rgb, byte>? picture)
		{
			if (picture?.Data == null)
				return;

			if (Buffor2Bitmap == null || Buffor2Bitmap.PixelWidth != picture.Width || Buffor2Bitmap.PixelHeight != picture.Height)
				Buffor2Bitmap = new WriteableBitmap(picture.Width, picture.Height, 96, 96, PixelFormats.Rgb24, null);
			var rect = new Int32Rect(0, 0, picture.Width, picture.Height);
			Buffor2Bitmap?.WritePixels(rect, picture.Bytes, picture.Width * 3, 0);
		}
		private void SetBuffor3Bitmap(Image<Rgb, byte>? picture)
		{
			if (picture?.Data == null)
				return;

			if (Buffor3Bitmap == null || Buffor3Bitmap.PixelWidth != picture.Width || Buffor3Bitmap.PixelHeight != picture.Height)
				Buffor3Bitmap = new WriteableBitmap(picture.Width, picture.Height, 96, 96, PixelFormats.Rgb24, null);
			var rect = new Int32Rect(0, 0, picture.Width, picture.Height);
			Buffor3Bitmap?.WritePixels(rect, picture.Bytes, picture.Width * 3, 0);
		}

		private void OnLeftPictureChanged(Image<Rgb, byte>? img)
		{
			_imageFiltrations.LoadImage(ImageFiltrations.ViewPoint.LeftImage, LeftPictureViewModel.CurrentFrame);
		}

		private void OnRightPictureChanged(Image<Rgb, byte>? img)
		{
			_imageFiltrations.LoadImage(ImageFiltrations.ViewPoint.RightImage, RightPictureViewModel.CurrentFrame);
		}
	}
}
