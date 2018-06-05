using System;
using System.Windows;
using System.Windows.Controls;

namespace Ignition.Presentation
{
	public class InstantStyleSelector : StyleSelector
	{
		readonly Func<object, DependencyObject, Style> selector;

		public InstantStyleSelector(Func<object, DependencyObject, Style> selector)
		{
			this.selector = selector;
		}

		public override Style SelectStyle(object item, DependencyObject container)
		{
			return selector(item, container) ?? base.SelectStyle(item, container);
		}
	}
}
