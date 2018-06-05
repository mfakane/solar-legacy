using System;
using System.Linq;
using System.Windows.Data;

namespace Ignition.Presentation
{
	public class BooleanOrConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return values.Select(_ => _ is bool? && ((bool?)_).HasValue ? ((bool?)_).Value : _)
						 .OfType<bool>()
						 .Aggregate(false, (from, _) => from || _);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
