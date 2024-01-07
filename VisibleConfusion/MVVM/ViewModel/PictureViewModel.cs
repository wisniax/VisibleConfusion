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
		public BitmapSource? PictureBitmapSource { get; set; }
		public BitmapSource? GraphBitmapSource { get; set; }

		public RelayCommand? DrawByHandCommand { get; set; }
		public RelayCommand? FromFileCommand { get; set; }
		public RelayCommand? FromCameraCommand { get; set; }
		public RelayCommand? CleanCommand { get; set; }
		public RelayCommand? DoGraphCommand { get; set; }

		public PictureViewModel()
		{
			PictureBitmapSource = new BitmapImage(new Uri("https://zoowojciechow.pl/images/lama-p-1080.jpeg"));
			GraphBitmapSource = new BitmapImage(new Uri("https://i.gremicdn.pl/image/free/678943239052287d16ff4ae67c083de9/"));
		}

	}
}
