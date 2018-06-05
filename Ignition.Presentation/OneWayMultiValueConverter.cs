using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace Ignition.Presentation
{
	public abstract class OneWayMultiValueConverter<TSource, TTarget> : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ConvertFromSource(values.Cast<TSource>(), parameter);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		protected abstract TTarget ConvertFromSource(IEnumerable<TSource> value, object parameter);
	}
}
