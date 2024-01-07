using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using VisibleConfusion.Core;
using VisibleConfusion.MVVM.Model;

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

		private Picture _picture;

		public RelayCommand? DrawByHandCommand { get; set; }
		public RelayCommand? FromFileCommand { get; set; }
		public RelayCommand? FromCameraCommand { get; set; }
		public RelayCommand? CleanCommand { get; set; }
		public RelayCommand? DoGraphCommand { get; set; }

		public PictureViewModel()
		{
			_picture = new Picture();
			//PictureBitmapSource = _picture.CurrentFrame?.Convert<Bgr, byte>().ToBitmapSource();
			_picture.FrameChanged += (sender) => PictureBitmapSource = sender?.Convert<Bgr, byte>().ToBitmapSource();
			_picture.GraphChanged += (sender) => GraphBitmapSource = sender?.Convert<Bgr, byte>().ToBitmapSource();

			_pictureBitmapSource = new BitmapImage(new Uri("https://zoowojciechow.pl/images/lama-p-1080.jpeg"));
			_graphBitmapSource = new BitmapImage(new Uri("https://i.gremicdn.pl/image/free/678943239052287d16ff4ae67c083de9/"));

			DrawByHandCommand = new RelayCommand((o) => throw new NotImplementedException());
			FromFileCommand = new RelayCommand(FromFile);
			FromCameraCommand = new RelayCommand(FromCamera);
			CleanCommand = new RelayCommand((o) => _picture.ClearFrame());
			DoGraphCommand = new RelayCommand((o) => throw new NotImplementedException());
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
			_picture.GetImageFromFile(new Uri(openFileDialog.FileName));
		}

		private void FromCamera(object obj)
		{
			PictureBitmapSource = new BitmapImage(new Uri("https://www.semtec.pl/wp-content/uploads/2016/09/kot.png"));
		}
	}
}
