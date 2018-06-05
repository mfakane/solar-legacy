using System.Windows.Forms;
using System.Windows.Interop;

namespace Ignition.Presentation
{
	public static class MessageBoxEx
	{
		static System.Windows.MessageBoxResult Convert(DialogResult dialogResult)
		{
			switch (dialogResult)
			{
				case DialogResult.Abort:
					return System.Windows.MessageBoxResult.Cancel;
				case DialogResult.Cancel:
					return System.Windows.MessageBoxResult.Cancel;
				case DialogResult.Ignore:
					return System.Windows.MessageBoxResult.OK;
				case DialogResult.No:
					return System.Windows.MessageBoxResult.No;
				case DialogResult.None:
					return System.Windows.MessageBoxResult.None;
				case DialogResult.OK:
					return System.Windows.MessageBoxResult.OK;
				case DialogResult.Retry:
					return System.Windows.MessageBoxResult.Yes;
				case DialogResult.Yes:
					return System.Windows.MessageBoxResult.Yes;
				default:
					return System.Windows.MessageBoxResult.None;
			}
		}

		static MessageBoxButtons Convert(System.Windows.MessageBoxButton button)
		{
			switch (button)
			{
				case System.Windows.MessageBoxButton.OK:
					return MessageBoxButtons.OK;
				case System.Windows.MessageBoxButton.OKCancel:
					return MessageBoxButtons.OKCancel;
				case System.Windows.MessageBoxButton.YesNo:
					return MessageBoxButtons.YesNo;
				case System.Windows.MessageBoxButton.YesNoCancel:
					return MessageBoxButtons.YesNoCancel;
				default:
					return MessageBoxButtons.OK;
			}
		}

		static MessageBoxIcon Convert(System.Windows.MessageBoxImage icon)
		{
			switch (icon)
			{
				case System.Windows.MessageBoxImage.Error:
					return MessageBoxIcon.Error;
				case System.Windows.MessageBoxImage.Warning:
					return MessageBoxIcon.Warning;
				case System.Windows.MessageBoxImage.Information:
					return MessageBoxIcon.Information;
				case System.Windows.MessageBoxImage.None:
					return MessageBoxIcon.None;
				case System.Windows.MessageBoxImage.Question:
					return MessageBoxIcon.Question;
				default:
					return MessageBoxIcon.None;
			}
		}

		static MessageBoxDefaultButton Convert(System.Windows.MessageBoxResult defaultResult)
		{
			switch (defaultResult)
			{
				case System.Windows.MessageBoxResult.Cancel:
					return MessageBoxDefaultButton.Button2;
				case System.Windows.MessageBoxResult.No:
					return MessageBoxDefaultButton.Button2;
				case System.Windows.MessageBoxResult.None:
					return MessageBoxDefaultButton.Button1;
				case System.Windows.MessageBoxResult.OK:
					return MessageBoxDefaultButton.Button1;
				case System.Windows.MessageBoxResult.Yes:
					return MessageBoxDefaultButton.Button1;
				default:
					return MessageBoxDefaultButton.Button1;
			}
		}

		static MessageBoxOptions Convert(System.Windows.MessageBoxOptions options)
		{
			switch (options)
			{
				case System.Windows.MessageBoxOptions.DefaultDesktopOnly:
					return MessageBoxOptions.DefaultDesktopOnly;
				case System.Windows.MessageBoxOptions.None:
					return (MessageBoxOptions)0;
				case System.Windows.MessageBoxOptions.RightAlign:
					return MessageBoxOptions.RightAlign;
				case System.Windows.MessageBoxOptions.RtlReading:
					return MessageBoxOptions.RtlReading;
				case System.Windows.MessageBoxOptions.ServiceNotification:
					return MessageBoxOptions.ServiceNotification;
				default:
					return (MessageBoxOptions)0;
			}
		}

		static System.Windows.Forms.IWin32Window Convert(System.Windows.Window owner)
		{
			return owner == null ? null : NativeWindow.FromHandle(new WindowInteropHelper(owner).Handle);
		}

		public static System.Windows.MessageBoxResult Show(string messageBoxText)
		{
			return Convert(MessageBox.Show(messageBoxText));
		}

		public static System.Windows.MessageBoxResult Show(string messageBoxText, string caption)
		{
			return Convert(MessageBox.Show(messageBoxText, caption));
		}

		public static System.Windows.MessageBoxResult Show(System.Windows.Window owner, string messageBoxText)
		{
			return Convert(MessageBox.Show(Convert(owner), messageBoxText));
		}

		public static System.Windows.MessageBoxResult Show(string messageBoxText, string caption, System.Windows.MessageBoxButton button)
		{
			return Convert(MessageBox.Show(messageBoxText, caption, Convert(button)));
		}

		public static System.Windows.MessageBoxResult Show(System.Windows.Window owner, string messageBoxText, string caption)
		{
			return Convert(MessageBox.Show(Convert(owner), messageBoxText, caption));
		}

		public static System.Windows.MessageBoxResult Show(string messageBoxText, string caption, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage icon)
		{
			return Convert(MessageBox.Show(messageBoxText, caption, Convert(button), Convert(icon)));
		}

		public static System.Windows.MessageBoxResult Show(System.Windows.Window owner, string messageBoxText, string caption, System.Windows.MessageBoxButton button)
		{
			return Convert(MessageBox.Show(Convert(owner), messageBoxText, caption, Convert(button)));
		}

		public static System.Windows.MessageBoxResult Show(string messageBoxText, string caption, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage icon, System.Windows.MessageBoxResult defaultResult)
		{
			return Convert(MessageBox.Show(messageBoxText, caption, Convert(button), Convert(icon), Convert(defaultResult)));
		}

		public static System.Windows.MessageBoxResult Show(System.Windows.Window owner, string messageBoxText, string caption, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage icon)
		{
			return Convert(MessageBox.Show(Convert(owner), messageBoxText, caption, Convert(button), Convert(icon)));
		}

		public static System.Windows.MessageBoxResult Show(string messageBoxText, string caption, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage icon, System.Windows.MessageBoxResult defaultResult, System.Windows.MessageBoxOptions options)
		{
			return Convert(MessageBox.Show(messageBoxText, caption, Convert(button), Convert(icon), Convert(defaultResult), Convert(options)));
		}

		public static System.Windows.MessageBoxResult Show(System.Windows.Window owner, string messageBoxText, string caption, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage icon, System.Windows.MessageBoxResult defaultResult)
		{
			return Convert(MessageBox.Show(Convert(owner), messageBoxText, caption, Convert(button), Convert(icon), Convert(defaultResult)));
		}

		public static System.Windows.MessageBoxResult Show(System.Windows.Window owner, string messageBoxText, string caption, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage icon, System.Windows.MessageBoxResult defaultResult, System.Windows.MessageBoxOptions options)
		{
			return Convert(MessageBox.Show(Convert(owner), messageBoxText, caption, Convert(button), Convert(icon), Convert(defaultResult), Convert(options)));
		}
	}
}
