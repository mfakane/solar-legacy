using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ignition.Presentation
{
	public abstract class ValueConverter<TSource, TTarget> : Freezable, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is TSource)
				return ConvertFromSource((TSource)value, parameter);
			else
				return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is TTarget)
				return ConvertToSource((TTarget)value, parameter);
			else
				return DependencyProperty.UnsetValue;
		}

		protected abstract TTarget ConvertFromSource(TSource value, object parameter);
		protected abstract TSource ConvertToSource(TTarget value, object parameter);

		protected override Freezable CreateInstanceCore()
		{
			return (Freezable)Activator.CreateInstance(this.GetType());
		}
	}
}
