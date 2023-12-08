using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace VisibleConfusion.Converters
{
	internal class ColorConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is not Color color)
				throw new ArgumentException("Value must be a color", nameof(value));
			color.R = (color.R + 74) > Byte.MaxValue ? Byte.MaxValue : (byte)(color.R + 74);
			color.G = (color.G + 74) > Byte.MaxValue ? Byte.MaxValue : (byte)(color.G + 74);
			color.B = (color.B + 74) > Byte.MaxValue ? Byte.MaxValue : (byte)(color.B + 74);
			return color;
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
