using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Structure;
using VisibleConfusion.Core;

namespace VisibleConfusion.MVVM.ViewModel
{
	internal class Task1ViewModel : ObservableObject
	{
		public class FilterColors
		{
			public bool Red { get; set; }
			public bool Green { get; set; }
			public bool Blue { get; set; }
		}

		private PictureViewModel _leftPictureViewModel;
		public PictureViewModel LeftPictureViewModel
		{
			get => _leftPictureViewModel;
			set
			{
				_leftPictureViewModel = value;
				OnPropertyChanged();
			}
		}

		private PictureViewModel _rightPictureViewModel;
		public PictureViewModel RightPictureViewModel
		{
			get => _rightPictureViewModel;
			set
			{
				_rightPictureViewModel = value;
				OnPropertyChanged();
			}
		}

		public RelayCommand? LeftToRightCommand { get; set; }
		public RelayCommand? RightToLeftCommand { get; set; }

		public FilterColors SelectedFilter { get; set; }

		public Rgb SelectedFilterColor =>
			new(
				Convert.ToByte(SelectedFilter.Red ? 255 : 0),
				Convert.ToByte(SelectedFilter.Green ? 255 : 0),
				Convert.ToByte(SelectedFilter.Blue ? 255 : 0)
			);

		public Task1ViewModel()
		{
			SelectedFilter = new FilterColors();
			_leftPictureViewModel = new PictureViewModel();
			_rightPictureViewModel = new PictureViewModel();

			LeftToRightCommand = new RelayCommand((o) => _rightPictureViewModel.SetPicture(_leftPictureViewModel?.CurrentFrame ?? null, SelectedFilterColor, false));
			RightToLeftCommand = new RelayCommand((o) => _leftPictureViewModel.SetPicture(_rightPictureViewModel?.CurrentFrame ?? null, SelectedFilterColor, false));
		}

	}
}
