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
		private int _numUpDownElem11 = 1;
		private int _numUpDownElem12 = 1;
		private int _numUpDownElem13 = 1;
		private int _numUpDownElem21 = 1;
		private int _numUpDownElem22 = 1;
		private int _numUpDownElem23 = 1;
		private int _numUpDownElem31 = 1;
		private int _numUpDownElem32 = 1;
		private int _numUpDownElem33 = 1;

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

		public RelayCommand? ButtonLowPassCommand { get; private set; }
		public RelayCommand? ButtonHighPassCommand { get; private set; }
		public RelayCommand? ButtonDilatationCommand { get; private set; }
		public RelayCommand? ButtonErosionCommand { get; private set; }
		public RelayCommand? ButtonFiltrateCommand { get; private set; }

		public int[,] StructuringElement
		{
			get
			{
				return new int[3, 3]
				{
					{NumUpDownElem11, NumUpDownElem12, NumUpDownElem13},
					{NumUpDownElem21, NumUpDownElem22, NumUpDownElem23},
					{NumUpDownElem31, NumUpDownElem32, NumUpDownElem33}
				};
			}
			set
			{
				NumUpDownElem11 = value[0, 0];
				NumUpDownElem12 = value[0, 1];
				NumUpDownElem13 = value[0, 2];
				NumUpDownElem21 = value[1, 0];
				NumUpDownElem22 = value[1, 1];
				NumUpDownElem23 = value[1, 2];
				NumUpDownElem31 = value[2, 0];
				NumUpDownElem32 = value[2, 1];
				NumUpDownElem33 = value[2, 2];
			}
		}

		public int NumUpDownElem11
		{
			get => _numUpDownElem11;
			set
			{
				if (value == _numUpDownElem11) return;
				_numUpDownElem11 = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StructuringElement));
			}
		}
		public int NumUpDownElem12
		{
			get => _numUpDownElem12;
			set
			{
				if (value == _numUpDownElem12) return;
				_numUpDownElem12 = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StructuringElement));
			}
		}
		public int NumUpDownElem13
		{
			get => _numUpDownElem13;
			set
			{
				if (value == _numUpDownElem13) return;
				_numUpDownElem13 = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StructuringElement));
			}
		}
		public int NumUpDownElem21
		{
			get => _numUpDownElem21;
			set
			{
				if (value == _numUpDownElem21) return;
				_numUpDownElem21 = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StructuringElement));
			}
		}
		public int NumUpDownElem22
		{
			get => _numUpDownElem22;
			set
			{
				if (value == _numUpDownElem22) return;
				_numUpDownElem22 = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StructuringElement));
			}
		}
		public int NumUpDownElem23
		{
			get => _numUpDownElem23;
			set
			{
				if (value == _numUpDownElem23) return;
				_numUpDownElem23 = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StructuringElement));
			}
		}
		public int NumUpDownElem31
		{
			get => _numUpDownElem31;
			set
			{
				if (value == _numUpDownElem31) return;
				_numUpDownElem31 = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StructuringElement));
			}
		}
		public int NumUpDownElem32
		{
			get => _numUpDownElem32;
			set
			{
				if (value == _numUpDownElem32) return;
				_numUpDownElem32 = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StructuringElement));
			}
		}
		public int NumUpDownElem33
		{
			get => _numUpDownElem33;
			set
			{
				if (value == _numUpDownElem33) return;
				_numUpDownElem33 = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StructuringElement));
			}
		}


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

			ButtonLowPassCommand = new RelayCommand((_) => StructuringElement = _imageFiltrations.GetFiltrationValues(ImageFiltrations.FiltrationType.LowwPass) ?? new int[3, 3]);
			ButtonHighPassCommand = new RelayCommand((_) => StructuringElement = _imageFiltrations.GetFiltrationValues(ImageFiltrations.FiltrationType.HighwPass) ?? new int[3, 3]);
			ButtonDilatationCommand = new RelayCommand((_) => StructuringElement = _imageFiltrations.GetFiltrationValues(ImageFiltrations.FiltrationType.Dilatation) ?? new int[3, 3]);
			ButtonErosionCommand = new RelayCommand((_) => StructuringElement = _imageFiltrations.GetFiltrationValues(ImageFiltrations.FiltrationType.Erosion) ?? new int[3, 3]);
			ButtonFiltrateCommand = new RelayCommand((_) => _rightPictureViewModel?.SetPicture(_imageFiltrations.DoFiltration(StructuringElement), null, null));

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

			Buffor1Bitmap = new WriteableBitmap(picture.Convert<Bgr, byte>().ToBitmapSource());
		}

		private void SetBuffor2Bitmap(Image<Rgb, byte>? picture)
		{
			if (picture?.Data == null)
				return;

			Buffor2Bitmap = new WriteableBitmap(picture.Convert<Bgr, byte>().ToBitmapSource());
		}

		private void SetBuffor3Bitmap(Image<Rgb, byte>? picture)
		{
			if (picture?.Data == null)
				return;

			Buffor3Bitmap = new WriteableBitmap(picture.Convert<Bgr, byte>().ToBitmapSource());
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
