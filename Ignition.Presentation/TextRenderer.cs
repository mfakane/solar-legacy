using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ignition.Presentation
{
	public class TextRenderer : Grid
	{
		System.Windows.Media.Brush brush = null;
		int brushWidth;
		int brushHeight;

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(object), typeof(TextRenderer), new UIPropertyMetadata(null, Update));
		public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(System.Windows.Media.Color), typeof(TextRenderer), new UIPropertyMetadata(System.Windows.SystemColors.MenuTextColor, Update));

		public System.Windows.Media.Color Foreground
		{
			get
			{
				return (System.Windows.Media.Color)GetValue(ForegroundProperty);
			}
			set
			{
				SetValue(ForegroundProperty, value);
			}
		}

		public object Text
		{
			get
			{
				return GetValue(TextProperty);
			}
			set
			{
				SetValue(TextProperty, value);
			}
		}

		Font GetFont()
		{
			return System.Drawing.SystemFonts.DialogFont;
		}

		System.Windows.Media.Brush CreateBrush()
		{
			var width = (int)this.ActualWidth;
			var height = (int)this.ActualHeight;

			if (width <= 0 || height <= 0)
				return System.Windows.Media.Brushes.Transparent;

			var rect = new System.Drawing.Rectangle(0, 0, width, height);

			using (var b = new System.Drawing.Bitmap(width, height))
			using (var g = System.Drawing.Graphics.FromImage(b))
			{
				System.Windows.Forms.TextRenderer.DrawText(g, GetText(), GetFont(), rect, System.Drawing.Color.FromArgb(this.Foreground.A, this.Foreground.R, this.Foreground.G, this.Foreground.B), TextFormatFlags.Left | TextFormatFlags.NoPadding);

				using (var bits = new LockBits(b, rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, b.PixelFormat))
					return new ImageBrush(BitmapSource.Create(brushWidth = b.Width, brushHeight = b.Height, g.DpiX, g.DpiY, PixelFormats.Bgra32, null, bits.Data.Scan0, bits.Data.Stride * b.Height, bits.Data.Stride));
			}
		}

		string GetText()
		{
			if (this.Text == null)
				return string.Empty;
			else if (this.Text is string)
				return this.Text.ToString().Replace("&", "&&").Replace("_", "&").Replace("&&", "_");
			else if (this.Text is AccessText)
				return ((AccessText)this.Text).Text.Replace("_", "&").Replace("&&", "_");
			else if (this.Text is TextBlock)
				return ((TextBlock)this.Text).Text;
			else
				return this.Text.ToString();
		}

		protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
		{
			var rt = System.Windows.Forms.TextRenderer.MeasureText(GetText(), GetFont(), new System.Drawing.Size((int)constraint.Width, (int)constraint.Height), TextFormatFlags.Left | TextFormatFlags.NoPadding);
			var rt2 = System.Windows.Forms.TextRenderer.MeasureText(GetText(), GetFont());

			rt2.Width -= 4;

			if (rt2.Width <= 0)
				rt2.Width = rt.Width;

			return new System.Windows.Size(Math.Min(rt.Width, rt2.Width), rt.Height);
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			this.brush = null;
		}

		protected override void OnRender(DrawingContext dc)
		{
			base.OnRender(dc);
			dc.DrawRectangle(brush ?? (brush = CreateBrush()), null, new Rect(0, 0, brushWidth, brushHeight));
		}

		static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var obj = (TextRenderer)d;

			obj.brush = null;
			obj.InvalidateVisual();
		}
	}
}
