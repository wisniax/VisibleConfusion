using System;
using System.Collections.Generic;
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
		private WriteableBitmap? _lookUpTableBitmap;

		public PictureViewModel RightPictureViewModel
		{
			get => _rightPictureViewModel;
			set
			{
				_rightPictureViewModel = value;
				OnPropertyChanged();
			}
		}

		public LookUpTableTransforms LookUpTableTransforms { get; private set; }

		public RelayCommand? LeftToRightCommand { get; set; }
		public RelayCommand? RightToLeftCommand { get; set; }

		public WriteableBitmap? LookUpTableBitmap
		{
			get => _lookUpTableBitmap;
			private set
			{
				_lookUpTableBitmap = value;
				OnPropertyChanged();
			}
		}

		public FilterColors SelectedFilter { get; set; }

		public Rgb SelectedFilterColor =>
			new(
				Convert.ToByte(SelectedFilter.Red ? 255 : 0),
				Convert.ToByte(SelectedFilter.Green ? 255 : 0),
				Convert.ToByte(SelectedFilter.Blue ? 255 : 0)
			);

		public RelayCommand? ButtonIdentityCommand { get; private set; }
		public RelayCommand? ButtonNegativeCommand { get; private set; }
		public RelayCommand? ButtonLightnessCommand { get; private set; }
		public RelayCommand? ButtonThresholdCommand { get; private set; }
		public RelayCommand? ButtonThreshold2PointsCommand { get; private set; }
		public RelayCommand? ButtonContrastCommand { get; private set; }

		public RelayCommand? OnNumericUpDownPoint1ChangedCommand { get; set; }
		public RelayCommand? OnNumericUpDownPoint2ChangedCommand { get; set; }

		public LookUpTableTransforms.PointsLimits NumericalUpDownLimits => LookUpTableTransforms.NumericalUpDownLimits ?? new LookUpTableTransforms.PointsLimits();


		public Task1ViewModel()
		{
			SelectedFilter = new FilterColors();
			_leftPictureViewModel = new PictureViewModel();
			_rightPictureViewModel = new PictureViewModel();
			LookUpTableTransforms = new LookUpTableTransforms();
			LookUpTableTransforms.SelectedTransformation = LookUpTableTransforms.Transformation.Identity;

			LookUpTableTransforms.LutImageChanged += SetWriteableLutBitmap;
			LookUpTableTransforms.NumericalUpDownLimitsChanged += (_) => OnPropertyChanged(nameof(NumericalUpDownLimits));

			LeftToRightCommand = new RelayCommand((o) => _rightPictureViewModel.SetPicture(_leftPictureViewModel?.CurrentFrame ?? null, SelectedFilterColor, false));
			RightToLeftCommand = new RelayCommand((o) => _leftPictureViewModel.SetPicture(_rightPictureViewModel?.CurrentFrame ?? null, SelectedFilterColor, false));

			SetWriteableLutBitmap(LookUpTableTransforms.CurrentLutImage);

			ButtonIdentityCommand = new RelayCommand((o) => LookUpTableTransforms.SelectedTransformation = LookUpTableTransforms.Transformation.Identity);
			ButtonNegativeCommand = new RelayCommand((o) => LookUpTableTransforms.SelectedTransformation = LookUpTableTransforms.Transformation.Negative);
			ButtonLightnessCommand = new RelayCommand((o) => LookUpTableTransforms.SelectedTransformation = LookUpTableTransforms.Transformation.Lightness);
			ButtonThresholdCommand = new RelayCommand((o) => LookUpTableTransforms.SelectedTransformation = LookUpTableTransforms.Transformation.Threshold);
			ButtonThreshold2PointsCommand = new RelayCommand((o) => LookUpTableTransforms.SelectedTransformation = LookUpTableTransforms.Transformation.Threshold2Points);
			ButtonContrastCommand = new RelayCommand((o) => LookUpTableTransforms.SelectedTransformation = LookUpTableTransforms.Transformation.Contrast);

			OnNumericUpDownPoint1ChangedCommand = new RelayCommand((o) =>
			{
				if (o is RoutedPropertyChangedEventArgs<object> rpc)
					LookUpTableTransforms.Point1 = Convert.ToInt32(rpc.NewValue);
			});

			OnNumericUpDownPoint2ChangedCommand = new RelayCommand((o) =>
			{
				if (o is RoutedPropertyChangedEventArgs<object> rpc)
					LookUpTableTransforms.Point2 = Convert.ToInt32(rpc.NewValue);
			});
		}

		private void SetWriteableLutBitmap(Image<Rgb, byte>? picture)
		{
			if (picture?.Data == null)
				return;

			if (LookUpTableBitmap == null || LookUpTableBitmap.PixelWidth != picture.Width || LookUpTableBitmap.PixelHeight != picture.Height)
				LookUpTableBitmap = new WriteableBitmap(picture.Width, picture.Height, 96, 96, PixelFormats.Rgb24, null);

			LookUpTableBitmap?.WritePixels(new Int32Rect(0, 0, picture.Width, picture.Height), picture.Bytes, picture.Width * 3, 0);
		}

	}
}
