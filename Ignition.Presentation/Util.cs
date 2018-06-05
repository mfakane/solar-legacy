using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ignition.Presentation
{
	public static partial class Util
	{
		public static Uri CreateComponentResourceUri(string path)
		{
			return CreateComponentResourceUri(path, Assembly.GetEntryAssembly());
		}

		public static Uri CreateComponentResourceUri(string path, Assembly assembly)
		{
			return new Uri("pack://application:,,,/" + assembly.GetName().Name + ";component/" + path);
		}

		public static IEnumerable<T> FindDescendant<T>(this DependencyObject self)
			where T : DependencyObject
		{
			return Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(self))
							 .SelectMany(_ => FindDescendant<T>(VisualTreeHelper.GetChild(self, _)))
							 .Prepend(self)
							 .OfType<T>();
		}

		public static IEnumerable<T> FindLogicalDescendant<T>(this DependencyObject self)
			where T : DependencyObject
		{
			return LogicalTreeHelper.GetChildren(self)
							 .OfType<DependencyObject>()
							 .SelectMany(_ => FindLogicalDescendant<T>(_))
							 .Prepend(self)
							 .OfType<T>();
		}

		public static T FindAncestor<T>(this DependencyObject self)
			where T : DependencyObject
		{
			return EnumerateAncestors<T>(self).FirstOrDefault();
		}

		public static IEnumerable<T> EnumerateAncestors<T>(this DependencyObject self)
			where T : DependencyObject
		{
			var d = self;

			while ((d = VisualTreeHelper.GetParent(d)) != null)
				if (d is T)
					yield return (T)d;
		}

		public static T FindLogicalAncestor<T>(this DependencyObject self)
			where T : DependencyObject
		{
			return EnumerateLogicalAncestors<T>(self).FirstOrDefault();
		}

		public static IEnumerable<T> EnumerateLogicalAncestors<T>(this DependencyObject self)
			where T : DependencyObject
		{
			var d = self;

			while ((d = (d is Popup ? ((Popup)d).PlacementTarget : null) ?? LogicalTreeHelper.GetParent(d)) != null)
				if (d is T)
					yield return (T)d;
		}

		public static void DoEvents(this Dispatcher dispatcher)
		{
			var frame = new DispatcherFrame();
			var operation = dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(arg =>
			{
				((DispatcherFrame)arg).Continue = false;

				return null;
			}), frame);

			Dispatcher.PushFrame(frame);

			if (operation.Status != DispatcherOperationStatus.Completed)
				operation.Abort();
		}

		public static void ResizeInScreen(this Window self, double x, double y, double width, double height)
		{
			var scr = ScreenHelper.GetScreenFromWindow(self);

			if (width > scr.Width)
				width = scr.Width;

			if (height > scr.Height)
				height = scr.Height;

			if (x < scr.X)
				x = scr.X;
			else if (x + width > scr.X + scr.Width)
				x = scr.X + scr.Width - width;

			if (y < scr.Y)
				y = scr.Y;
			else if (y + height > scr.Y + scr.Height)
				y = scr.Y + scr.Height - height;

			self.Left = x;
			self.Top = y;
			self.Width = width;
			self.Height = height;
		}

		public static void AddRange<T>(this ObservableCollection<T> self, IEnumerable<T> collection)
		{
			foreach (var i in collection)
				self.Add(i);
		}

		public static void AddRangeAsync<T>(this ObservableCollection<T> self, IEnumerable<T> collection)
		{
			Task.Factory.StartNew(() =>
			{
				foreach (var i in collection)
					Application.Current.Dispatcher.Invoke((Action)(() => self.Add(i)));
			});
		}

		public static void DispatchEvent(this EventHandler self, object sender, EventArgs e)
		{
			if (Application.Current == null ||
				Application.Current.Dispatcher == null)
				self.RaiseEvent(sender, e);
			else
				Application.Current.Dispatcher.Invoke((Action)delegate
				{
					if (self != null)
						self(sender, e);
				});
		}

		public static void DispatchEvent<T>(this EventHandler<T> self, object sender, T e)
			where T : EventArgs
		{
			if (Application.Current == null ||
				Application.Current.Dispatcher == null)
				self.RaiseEvent(sender, e);
			else
				Application.Current.Dispatcher.Invoke((Action)delegate
				{
					if (self != null)
						self(sender, e);
				});
		}
	}
}
