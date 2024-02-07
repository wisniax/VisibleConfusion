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

			byte offset;
			if (parameter is not string parameterString || !Byte.TryParse(parameterString, System.Globalization.NumberStyles.Integer, null, out offset))
				offset = 74;

			color.R = (color.R + offset) > Byte.MaxValue ? Byte.MaxValue : (byte)(color.R + offset);
			color.G = (color.G + offset) > Byte.MaxValue ? Byte.MaxValue : (byte)(color.G + offset);
			color.B = (color.B + offset) > Byte.MaxValue ? Byte.MaxValue : (byte)(color.B + offset);
			return color;
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
