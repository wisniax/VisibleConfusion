using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Emgu.CV;
using Emgu.CV.Structure;

namespace VisibleConfusion.MVVM.Model
{
	public class ImageFiltrations
	{
		public enum ViewPoint
		{
			LeftImage,
			RightImage,
			Buffer1Image,
			Buffer2Image,
			Buffer3Image
		}

		public enum BitwiseOperation
		{
			And,
			Or,
			Xor
		}

		public enum FiltrationType
		{
			LowwPass,
			HighwPass,
			Dilatation,
			Erosion,
			Custom
		}

		public delegate void ImagesChangedEventHandler(Dictionary<ViewPoint, Image<Rgb, byte>?> images);
		public event ImagesChangedEventHandler? ImagesChanged;

		public Dictionary<ViewPoint, Image<Rgb, byte>?> Images { get; private set; } =
			new Dictionary<ViewPoint, Image<Rgb, byte>?>();

		public ImageFiltrations()
		{
			Images.Add(ViewPoint.LeftImage, null);
			Images.Add(ViewPoint.RightImage, null);
			Images.Add(ViewPoint.Buffer1Image, null);
			Images.Add(ViewPoint.Buffer2Image, null);
			Images.Add(ViewPoint.Buffer3Image, null);
		}

		public void MoveImage(ViewPoint from, ViewPoint to)
		{
			if (!Images.ContainsKey(from) || !Images.ContainsKey(to))
				return;
			Images[to]?.Dispose();
			Images[to] = Images[from]?.Copy();
			OnImagesChanged();
		}

		public void LoadImage(ViewPoint viewPoint, Image<Rgb, byte>? image)
		{
			if (Images.ContainsKey(viewPoint))
				Images[viewPoint]?.Dispose();
			Images[viewPoint] = image?.Copy();
			OnImagesChanged();
		}

		public void ApplyBitwiseOperation(BitwiseOperation operation)
		{
			if (!Images.ContainsKey(ViewPoint.Buffer1Image) || !Images.ContainsKey(ViewPoint.Buffer2Image))
				return;

			if (Images[ViewPoint.Buffer1Image]?.Width != Images[ViewPoint.Buffer2Image]?.Width ||
				Images[ViewPoint.Buffer1Image]?.Height != Images[ViewPoint.Buffer2Image]?.Height)
			{
				MessageBox.Show("Buffor internal sizes DO NOT match. \n Operation cannot be performed");
				return;
			}

			if (Images.ContainsKey(ViewPoint.Buffer3Image))
				Images[ViewPoint.Buffer3Image]?.Dispose();

			Images[ViewPoint.Buffer3Image] = new Image<Rgb, byte>(Images[ViewPoint.Buffer1Image]?.Width ?? 1, Images[ViewPoint.Buffer1Image]?.Height ?? 1);

			switch (operation)
			{
				case BitwiseOperation.And:
					for (int i = 0; i < Images[ViewPoint.Buffer1Image]?.Height; i++)
					{
						for (int j = 0; j < Images[ViewPoint.Buffer2Image]?.Width; j++)
						{
							Images[ViewPoint.Buffer3Image]![i, j] = new Rgb
							{
								Red = ((byte)(Images[ViewPoint.Buffer1Image]?[i, j].Red ?? 0)) &
											 ((byte)(Images[ViewPoint.Buffer2Image]?[i, j].Red ?? 0)),
								Green = ((byte)(Images[ViewPoint.Buffer1Image]?[i, j].Green ?? 0)) &
											 ((byte)(Images[ViewPoint.Buffer2Image]?[i, j].Green ?? 0)),
								Blue = ((byte)(Images[ViewPoint.Buffer1Image]?[i, j].Blue ?? 0)) &
											 ((byte)(Images[ViewPoint.Buffer2Image]?[i, j].Blue ?? 0))
							};
						}
					}
					break;

				case BitwiseOperation.Or:
					for (int i = 0; i < Images[ViewPoint.Buffer1Image]?.Height; i++)
					{
						for (int j = 0; j < Images[ViewPoint.Buffer2Image]?.Width; j++)
						{
							Images[ViewPoint.Buffer3Image]![i, j] = new Rgb
							{
								Red = ((byte)(Images[ViewPoint.Buffer1Image]?[i, j].Red ?? 0)) |
											 ((byte)(Images[ViewPoint.Buffer2Image]?[i, j].Red ?? 0)),
								Green = ((byte)(Images[ViewPoint.Buffer1Image]?[i, j].Green ?? 0)) |
											 ((byte)(Images[ViewPoint.Buffer2Image]?[i, j].Green ?? 0)),
								Blue = ((byte)(Images[ViewPoint.Buffer1Image]?[i, j].Blue ?? 0)) |
											 ((byte)(Images[ViewPoint.Buffer2Image]?[i, j].Blue ?? 0))
							};
						}
					}
					break;

				case BitwiseOperation.Xor:
					for (int i = 0; i < Images[ViewPoint.Buffer1Image]?.Height; i++)
					{
						for (int j = 0; j < Images[ViewPoint.Buffer2Image]?.Width; j++)
						{
							Images[ViewPoint.Buffer3Image]![i, j] = new Rgb
							{
								Red = ((byte)(Images[ViewPoint.Buffer1Image]?[i, j].Red ?? 0)) ^
											 ((byte)(Images[ViewPoint.Buffer2Image]?[i, j].Red ?? 0)),
								Green = ((byte)(Images[ViewPoint.Buffer1Image]?[i, j].Green ?? 0)) ^
											 ((byte)(Images[ViewPoint.Buffer2Image]?[i, j].Green ?? 0)),
								Blue = ((byte)(Images[ViewPoint.Buffer1Image]?[i, j].Blue ?? 0)) ^
											 ((byte)(Images[ViewPoint.Buffer2Image]?[i, j].Blue ?? 0))
							};
						}
					}
					break;

				default:
					return;
			}
			OnImagesChanged();
		}

		public Int32[,]? GetFiltrationValues(FiltrationType filtrationType)
		{
			switch (filtrationType)
			{
				case FiltrationType.LowwPass:
					return new Int32[3, 3]
					{
						{1, 2, 1},
						{2, 4, 2},
						{1, 2, 1}
					};

				case FiltrationType.HighwPass:
					return new Int32[3, 3]
					{
						{1, 0, -1},
						{1, 0, -1},
						{1, 0, -1}
					};

				case FiltrationType.Dilatation:
					return new Int32[3, 3]
					{
						{1, 1, 1},
						{1, 0, 1},
						{1, 1, 1}
					};

				case FiltrationType.Erosion:
					return new Int32[3, 3]
					{
						{1, 1, 1},
						{1, 1, 1},
						{1, 1, 1}
					};

				default:
					return null;
			}
		}

		public Image<Rgb, byte>? DoFiltration(Int32[,] matrix)
		{
			if (!Images.ContainsKey(ViewPoint.LeftImage))
				return null;

			if (Images[ViewPoint.LeftImage] == null)
				return null;

			Int32 matrixSum = matrix.Cast<int>().Sum() == 0 ? 1 : matrix.Cast<int>().Sum();

			var img = new Image<Rgb, byte>(Images[ViewPoint.LeftImage]?.Width - 2 ?? 1, Images[ViewPoint.LeftImage]?.Height - 2 ?? 1);

			for (int i = 1; i < img.Height; i++)
			{
				for (int j = 1; j < img.Width; j++)
				{
					Rgb color = new Rgb();

					for (int k = -1; k < 2; k++)
					{
						for (int l = -1; l < 2; l++)
						{
							color.Red += (Images[ViewPoint.LeftImage]![i + k, j + l].Red * matrix[k + 1, l + 1]);
							color.Green += (Images[ViewPoint.LeftImage]![i + k, j + l].Green * matrix[k + 1, l + 1]);
							color.Blue += (Images[ViewPoint.LeftImage]![i + k, j + l].Blue * matrix[k + 1, l + 1]);
						}
					}

					color.Red /= matrixSum;
					color.Green /= matrixSum;
					color.Blue /= matrixSum;

					img[i - 1, j - 1] = color;
				}
			}

			return img;
		}

		public void OnImagesChanged()
		{
			ImagesChanged?.Invoke(Images);
		}

	}
}
