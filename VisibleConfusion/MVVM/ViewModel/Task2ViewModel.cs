using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VisibleConfusion.Core;
using VisibleConfusion.MVVM.Model;

namespace VisibleConfusion.MVVM.ViewModel
{
	internal class Task2ViewModel : ObservableObject
	{

		private PictureViewModel _leftPictureViewModel;
		private PictureViewModel _rightPictureViewModel;

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



	}
}
