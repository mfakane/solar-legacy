using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace Ignition.Presentation
{
	/// <summary>
	/// Represents a thin wrapper for <see cref="Forms.NotifyIcon"/>
	/// </summary>
	[ContentProperty("Text")]
	[DefaultEvent("MouseDoubleClick")]
	public class NotificationAreaIcon : FrameworkElement
	{
		Forms.NotifyIcon notifyIcon;

		public static readonly RoutedEvent MouseClickEvent = EventManager.RegisterRoutedEvent("MouseClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotificationAreaIcon));
		public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent("MouseDoubleClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotificationAreaIcon));
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(NotificationAreaIcon));
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(NotificationAreaIcon));
		public static readonly DependencyProperty FormsContextMenuProperty = DependencyProperty.Register("MenuItems", typeof(List<Forms.MenuItem>), typeof(NotificationAreaIcon), new PropertyMetadata(new List<Forms.MenuItem>()));

		static NotificationAreaIcon()
		{
			FrameworkElement.VisibilityProperty.OverrideMetadata(typeof(NotificationAreaIcon), new PropertyMetadata(OnVisibilityChanged));
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			notifyIcon = new Forms.NotifyIcon();
			notifyIcon.Text = Text;

			if (!DesignerProperties.GetIsInDesignMode(this))
				notifyIcon.Icon = FromImageSource(Icon);

			notifyIcon.Visible = FromVisibility(Visibility);

			if (this.MenuItems != null && this.MenuItems.Count > 0)
				notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(this.MenuItems.ToArray());

			notifyIcon.MouseDown += OnMouseDown;
			notifyIcon.MouseUp += OnMouseUp;
			notifyIcon.MouseClick += OnMouseClick;
			notifyIcon.MouseDoubleClick += OnMouseDoubleClick;

			Dispatcher.ShutdownStarted += OnDispatcherShutdownStarted;
		}

		static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var self = (NotificationAreaIcon)d;

			if (self != null &&
				self.notifyIcon != null)
				self.notifyIcon.Visible = FromVisibility((Visibility)e.NewValue);
		}

		void OnDispatcherShutdownStarted(object sender, EventArgs e)
		{
			notifyIcon.Dispose();
		}

		void OnMouseDown(object sender, Forms.MouseEventArgs e)
		{
			OnRaiseEvent(MouseDownEvent, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
		}

		void OnMouseUp(object sender, Forms.MouseEventArgs e)
		{
			OnRaiseEvent(MouseUpEvent, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
		}

		void OnMouseDoubleClick(object sender, Forms.MouseEventArgs e)
		{
			OnRaiseEvent(MouseDoubleClickEvent, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
		}

		void OnMouseClick(object sender, Forms.MouseEventArgs e)
		{
			this.ContextMenu.IsOpen = false;

			if (e.Button == Forms.MouseButtons.Right &&
				this.ContextMenu != null)
			{
				FindAncestor<Window>(this).Activate();
				this.ContextMenu.Placement = PlacementMode.MousePoint;
				this.ContextMenu.IsOpen = true;
			}
			else
				OnRaiseEvent(MouseClickEvent, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
		}

		T FindAncestor<T>(DependencyObject obj)
			where T : DependencyObject
		{
			var rt = VisualTreeHelper.GetParent(obj);

			if (rt is T)
				return (T)rt;
			else
				return FindAncestor<T>(rt);
		}

		void OnRaiseEvent(RoutedEvent handler, MouseButtonEventArgs e)
		{
			e.RoutedEvent = handler;
			RaiseEvent(e);
		}

		public List<Forms.MenuItem> MenuItems
		{
			get
			{
				return (List<Forms.MenuItem>)GetValue(FormsContextMenuProperty);
			}
			set
			{
				SetValue(FormsContextMenuProperty, value);
			}
		}

		public ImageSource Icon
		{
			get
			{
				return (ImageSource)GetValue(IconProperty);
			}
			set
			{
				SetValue(IconProperty, value);
			}
		}

		public string Text
		{
			get
			{
				return (string)GetValue(TextProperty);
			}
			set
			{
				SetValue(TextProperty, value);
			}
		}

		public event MouseButtonEventHandler MouseClick
		{
			add
			{
				AddHandler(MouseClickEvent, value);
			}
			remove
			{
				RemoveHandler(MouseClickEvent, value);
			}
		}

		public event MouseButtonEventHandler MouseDoubleClick
		{
			add
			{
				AddHandler(MouseDoubleClickEvent, value);
			}
			remove
			{
				RemoveHandler(MouseDoubleClickEvent, value);
			}
		}

		static Drawing.Icon FromImageSource(ImageSource icon)
		{
			if (icon == null)
				return null;

			return new Drawing.Icon(Application.GetResourceStream(new Uri(icon.ToString())).Stream, new Drawing.Size(16, 16));
		}

		static bool FromVisibility(Visibility visibility)
		{
			return visibility == Visibility.Visible;
		}

		MouseButton ToMouseButton(Forms.MouseButtons button)
		{
			switch (button)
			{
				case Forms.MouseButtons.Left:
					return MouseButton.Left;
				case Forms.MouseButtons.Right:
					return MouseButton.Right;
				case Forms.MouseButtons.Middle:
					return MouseButton.Middle;
				case Forms.MouseButtons.XButton1:
					return MouseButton.XButton1;
				case Forms.MouseButtons.XButton2:
					return MouseButton.XButton2;
			}

			throw new InvalidOperationException();
		}
	}
}