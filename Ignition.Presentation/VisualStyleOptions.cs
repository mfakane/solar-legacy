using System.Windows;

namespace Ignition.Presentation
{
	public static class VisualStyleOptions
	{
		public static readonly DependencyProperty IsRadioCheckMenuItemProperty = DependencyProperty.RegisterAttached("IsRadioCheckMenuItem", typeof(bool), typeof(VisualStyleOptions), new UIPropertyMetadata(false));

		public static bool GetIsRadioCheckMenuItem(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsRadioCheckMenuItemProperty);
		}

		public static void SetIsRadioCheckMenuItem(DependencyObject obj, bool value)
		{
			obj.SetValue(IsRadioCheckMenuItemProperty, value);
		}
	}
}
