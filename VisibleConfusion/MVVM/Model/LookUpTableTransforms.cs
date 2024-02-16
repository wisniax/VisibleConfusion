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

		public enum Transformation
		{
			Identity,
			Negative,
			Lightness,
			Threshold,
			Threshold2Points,
			Contrast
		}

		public delegate void ImageChangedEventHandler(Image<Rgb, byte>? sender);
		public event ImageChangedEventHandler? LutImageChanged;

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
					break;
				case Transformation.Negative:
					for (int i = 0; i < LookUpTable?.Length; i++)
					{
						LookUpTable[i] = (byte)(255 - i);
					}
					break;
				case Transformation.Lightness:
					for (int i = 0; i < LookUpTable?.Length; i++)
					{
						LookUpTable[i] = (byte)Int64.Clamp(i + Point1, 0, 255);
					}
					break;
				case Transformation.Threshold:
					for (int i = 0; i < LookUpTable?.Length; i++)
					{
						LookUpTable[i] = (byte)(i < Point1 ? 0 : 255);
					}
					break;
				case Transformation.Threshold2Points:
					for (int i = 0; i < LookUpTable?.Length; i++)
					{
						LookUpTable[i] = (byte)(i < Point1 ? 0 : i < Point2 ? 255 : 0);
					}
					break;
				case Transformation.Contrast:
					double factor = (259.0 * (Point1 + 255.0)) / (255.0 * (259.0 - Point1));

					for (int i = 0; i < LookUpTable?.Length; i++)
					{
						LookUpTable[i] = (byte)Int64.Clamp((int)(factor * (i - 128) + 128), 0, 255);
					}

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

	}
}
