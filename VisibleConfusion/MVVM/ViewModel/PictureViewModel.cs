using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Emgu.CV;
using VisibleConfusion.Core;

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
			get { return _graphBitmapSource; }
			private set { _graphBitmapSource = value; }
		}


		public RelayCommand? DrawByHandCommand { get; set; }
		public RelayCommand? FromFileCommand { get; set; }
		public RelayCommand? FromCameraCommand { get; set; }
		public RelayCommand? CleanCommand { get; set; }
		public RelayCommand? DoGraphCommand { get; set; }

		public PictureViewModel()
		{
			_pictureBitmapSource = new BitmapImage(new Uri("https://zoowojciechow.pl/images/lama-p-1080.jpeg"));
			_graphBitmapSource = new BitmapImage(new Uri("https://i.gremicdn.pl/image/free/678943239052287d16ff4ae67c083de9/"));

			DrawByHandCommand = new RelayCommand(DrawByHand);
			FromFileCommand = new RelayCommand(FromFile);
			FromCameraCommand = new RelayCommand(FromCamera);
			CleanCommand = new RelayCommand(CleanBitmaps);
			DoGraphCommand = new RelayCommand(DoGraph);
		}

		private void DrawByHand(object obj)
		{
			throw new NotImplementedException();
		}

		private void FromFile(object obj)
		{
			throw new NotImplementedException();
		}

		private void FromCamera(object obj)
		{
			PictureBitmapSource = new BitmapImage(new Uri("https://www.semtec.pl/wp-content/uploads/2016/09/kot.png"));
		}

		private void CleanBitmaps(object obj)
		{
			throw new NotImplementedException();
		}

		private void DoGraph(object obj)
		{
			throw new NotImplementedException();
		}

	}
}
