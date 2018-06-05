using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace Ignition.Presentation
{
	public static class ScreenHelper
	{
		public static Int32Rect GetPrimaryScreen()
		{
			var rt = Screen.PrimaryScreen;

			return new Int32Rect(rt.Bounds.Left, rt.Bounds.Top, rt.Bounds.Width, rt.Bounds.Height);
		}

		public static Int32Rect GetPrimaryWorkingArea()
		{
			var rt = Screen.PrimaryScreen;

			return new Int32Rect(rt.WorkingArea.Left, rt.WorkingArea.Top, rt.WorkingArea.Width, rt.WorkingArea.Height);
		}

		public static Int32Rect GetScreenFromWindow(Window window)
		{
			var rt = Screen.FromRectangle(new Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height));

			return new Int32Rect(rt.Bounds.Left, rt.Bounds.Top, rt.Bounds.Width, rt.Bounds.Height);
		}

		public static Int32Rect GetWorkingAreaFromWindow(Window window)
		{
			var rt = Screen.FromRectangle(new Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height));

			return new Int32Rect(rt.WorkingArea.Left, rt.WorkingArea.Top, rt.WorkingArea.Width, rt.WorkingArea.Height);
		}
	}
}
