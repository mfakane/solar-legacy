using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Ignition.Presentation
{
	public static class WindowOptions
	{
		static readonly PropertyChangedCallback UpdateProperty = (sender, e) =>
		{
			var w = sender as Window;

			if (w == null)
				return;

			w.SourceInitialized -= SourceInitialized;
			w.SourceInitialized += SourceInitialized;
		};
		public static readonly DependencyProperty ShowIconProperty = DependencyProperty.RegisterAttached("ShowIcon", typeof(bool?), typeof(WindowOptions), new UIPropertyMetadata(null, UpdateProperty));
		public static readonly DependencyProperty MaximizeBoxProperty = DependencyProperty.RegisterAttached("MaximizeBox", typeof(bool?), typeof(WindowOptions), new UIPropertyMetadata(null, UpdateProperty));
		public static readonly DependencyProperty MinimizeBoxProperty = DependencyProperty.RegisterAttached("MinimizeBox", typeof(bool?), typeof(WindowOptions), new UIPropertyMetadata(null, UpdateProperty));

		[DllImport("user32")]
		static extern int GetWindowLong(IntPtr hWnd, int index);
		[DllImport("user32")]
		static extern int SetWindowLong(IntPtr hWnd, int index, int newStyle);
		[DllImport("user32")]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height, uint flags);

		const int GWL_STYLE = -16;
		const int GWL_EXSTYLE = -20;
		const int WS_MINIMIZEBOX = 0x20000;
		const int WS_MAXIMIZEBOX = 0x10000;
		const int WS_EX_DLGMODALFRAME = 0x0001;
		const int SWP_NOSIZE = 0x0001;
		const int SWP_NOMOVE = 0x0002;
		const int SWP_NOZORDER = 0x0004;
		const int SWP_FRAMECHANGED = 0x0020;

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static bool? GetShowIcon(DependencyObject obj)
		{
			return (bool?)obj.GetValue(ShowIconProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static void SetShowIcon(DependencyObject obj, bool? value)
		{
			obj.SetValue(ShowIconProperty, value);
		}

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static bool? GetMaximizeBox(DependencyObject obj)
		{
			return (bool?)obj.GetValue(MaximizeBoxProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static void SetMaximizeBox(DependencyObject obj, bool? value)
		{
			obj.SetValue(MaximizeBoxProperty, value);
		}

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static bool? GetMinimizeBox(DependencyObject obj)
		{
			return (bool?)obj.GetValue(MinimizeBoxProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(Window))]
		public static void SetMinimizeBox(DependencyObject obj, bool? value)
		{
			obj.SetValue(MinimizeBoxProperty, value);
		}

		static void SourceInitialized(object sender, EventArgs e)
		{
			var w = (Window)sender;
			var hWnd = new WindowInteropHelper(w).Handle;

			var windowStyle = GetWindowLong(hWnd, GWL_STYLE);
			var extendedStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

			GetMaximizeBox(w).TypeMatch((bool _) => _ ? windowStyle |= WS_MAXIMIZEBOX : windowStyle &= ~WS_MAXIMIZEBOX);
			GetMinimizeBox(w).TypeMatch((bool _) => _ ? windowStyle |= WS_MINIMIZEBOX : windowStyle &= ~WS_MINIMIZEBOX);
			GetShowIcon(w).TypeMatch((bool _) => _ ? extendedStyle &= ~WS_EX_DLGMODALFRAME : extendedStyle |= WS_EX_DLGMODALFRAME);
			windowStyle &= ~WS_MAXIMIZEBOX;
			SetWindowLong(hWnd, GWL_STYLE, windowStyle);
			SetWindowLong(hWnd, GWL_EXSTYLE, extendedStyle);
			SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
		}
	}
}
