using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABI.Windows.UI.Input;
using Emgu.CV;
using Emgu.CV.Structure;

namespace VisibleConfusion.MVVM.Model
{
	class LookUpTableTransforms
	{
		private Transformation _selectedTransformation;
		private int _point1;
		private int _point2;
		private PointsLimits? _numericalUpDownLimits;

		public enum Transformation
		{
			Identity,
			Negative,
			Lightness,
			Threshold,
			Threshold2Points,
			Contrast
		}

		public class PointsLimits
		{
			public int Point1Max { get; set; } = 255;
			public int Point1Min { get; set; } = 0;
			public bool Point1Visible { get; set; } = false;
			public int Point2Max { get; set; } = 255;
			public int Point2Min { get; set; } = 0;
			public bool Point2Visible { get; set; } = false;
		}

		public delegate void ImageChangedEventHandler(Image<Rgb, byte>? sender);
		public event ImageChangedEventHandler? LutImageChanged;

		public delegate void PointsLimitChangedEventHandler(PointsLimits? pointsLimits);
		public event PointsLimitChangedEventHandler? NumericalUpDownLimitsChanged;

		public PointsLimits? NumericalUpDownLimits
		{
			get => _numericalUpDownLimits;
			set
			{
				_numericalUpDownLimits = value;
				OnNumericalUpDownLimitsChanged();
			}
		}

		public Int32 Point1
		{
			get => _point1;
			set
			{
				_point1 = value;
				DoTransformation();
			}
		}

		public Int32 Point2
		{
			get => _point2;
			set
			{
				_point2 = value;
				DoTransformation();
			}
		}

		public byte[]? LookUpTable { get; private set; }

		public Image<Rgb, byte>? CurrentLutImage { get; private set; }

		public Transformation SelectedTransformation
		{
			get => _selectedTransformation;
			set
			{
				_selectedTransformation = value;
				DoTransformation();
			}
		}

		public LookUpTableTransforms()
		{
			_point1 = 0;
			_point2 = 127;
			LookUpTable = new byte[256];
			_numericalUpDownLimits = new();

			CurrentLutImage = new Image<Rgb, byte>(256, 256, new Rgb(Color.Black));
		}

		private void DoTransformation()
		{
			switch (SelectedTransformation)
			{
				case Transformation.Identity:
					for (int i = 0; i < LookUpTable?.Length; i++)
					{
						LookUpTable[i] = (byte)i;
					}

					NumericalUpDownLimits = new();
					break;

				case Transformation.Negative:
					for (int i = 0; i < LookUpTable?.Length; i++)
					{
						LookUpTable[i] = (byte)(255 - i);
					}

					NumericalUpDownLimits = new();
					break;

				case Transformation.Lightness:
					for (int i = 0; i < LookUpTable?.Length; i++)
					{
						LookUpTable[i] = (byte)Int64.Clamp(i + Point1, 0, 255);
					}

					NumericalUpDownLimits = new()
					{
						Point1Min = -255,
						Point1Max = 255,
						Point1Visible = true
					};
					break;

				case Transformation.Threshold:
					for (int i = 0; i < LookUpTable?.Length; i++)
					{
						LookUpTable[i] = (byte)(i < Point1 ? 0 : 255);
					}

					NumericalUpDownLimits = new()
					{
						Point1Min = 0,
						Point1Max = 255,
						Point1Visible = true
					};
					break;

				case Transformation.Threshold2Points:
					for (int i = 0; i < LookUpTable?.Length; i++)
					{
						LookUpTable[i] = (byte)(i < Point1 ? 0 : i < Point2 ? 255 : 0);
					}

					NumericalUpDownLimits = new()
					{
						Point1Min = 0,
						Point1Max = 255,
						Point1Visible = true,
						Point2Min = Point1,
						Point2Max = 255,
						Point2Visible = true
					};
					break;

				case Transformation.Contrast:
					double factor = (259.0 * (Point1 + 255.0)) / (255.0 * (259.0 - Point1));

					for (int i = 0; i < LookUpTable?.Length; i++)
					{
						LookUpTable[i] = (byte)Int64.Clamp((int)(factor * (i - 128) + 128), 0, 255);
					}

					NumericalUpDownLimits = new()
					{
						Point1Min = -255,
						Point1Max = 255,
						Point1Visible = true
					};
					break;

				default:
					break;
			}

			OnLookUpTableChanged();
		}

		private void OnLookUpTableChanged()
		{
			using (var lutImage = new Image<Rgb, byte>(256, 256, new Rgb(Color.Black)))
			{
				Point[] points = new Point[256];

				for (int i = 0; i < LookUpTable?.Length; i++)
				{
					points[i] = new Point(i, 255 - LookUpTable[i]);
				}

				lutImage.DrawPolyline(points, false, new Rgb(Color.White), 4);
				lutImage.CopyTo(CurrentLutImage);
			}

			OnImageChanged();
		}

		private void OnImageChanged()
		{
			LutImageChanged?.Invoke(CurrentLutImage);
		}
		private void OnNumericalUpDownLimitsChanged()
		{
			NumericalUpDownLimitsChanged?.Invoke(NumericalUpDownLimits);
		}

	}
}
