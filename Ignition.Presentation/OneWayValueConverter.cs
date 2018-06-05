using System;

namespace Ignition.Presentation
{
	public abstract class OneWayValueConverter<TSource, TTarget> : ValueConverter<TSource, TTarget>
	{
		protected override TSource ConvertToSource(TTarget value, object parameter)
		{
			throw new NotSupportedException();
		}
	}
}
