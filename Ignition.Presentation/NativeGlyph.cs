using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace Ignition.Presentation
{
	public class NativeGlyph : Grid
	{
		Brush brush;

		public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Color), typeof(NativeGlyph), new UIPropertyMetadata(SystemColors.ControlTextColor, UpdateBackground));
		public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(GlyphType), typeof(NativeGlyph), new UIPropertyMetadata(GlyphType.Arrow, UpdateBackground));

		public GlyphType Glyph
		{
			get
			{
				return (GlyphType)GetValue(GlyphProperty);
			}
			set
			{
				SetValue(GlyphProperty, value);
			}
		}

		public Color Foreground
		{
			get
			{
				return (Color)GetValue(ForegroundProperty);
			}
			set
			{
				SetValue(ForegroundProperty, value);
			}
		}

		Brush Render(Action<Drawing.Graphics, Drawing.Rectangle, Drawing.Color> render)
		{
			var width = (int)this.ActualWidth;
			var height = (int)this.ActualHeight;

			if (width <= 0 || height <= 0)
				return Brushes.Transparent;

			var rect = new Drawing.Rectangle(0, 0, width, height);

			using (var b = new Drawing.Bitmap(width, height))
			using (var g = Drawing.Graphics.FromImage(b))
			{
				render(g, rect, Drawing.Color.FromArgb(this.Foreground.A, this.Foreground.R, this.Foreground.G, this.Foreground.B));

				using (var bits = new LockBits(b, rect, Drawing.Imaging.ImageLockMode.ReadOnly, b.PixelFormat))
					return new ImageBrush(BitmapSource.Create(b.Width, b.Height, g.DpiX, g.DpiY, PixelFormats.Bgra32, null, bits.Data.Scan0, bits.Data.Stride * b.Height, bits.Data.Stride));
			}
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			this.brush = null;
		}

		static void UpdateBackground(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ng = (NativeGlyph)d;

			ng.brush = null;
			ng.InvalidateVisual();
		}

		protected override void OnRender(DrawingContext dc)
		{
			if (brush == null)
				brush = Render((g, r, c) =>
				{
					switch (this.Glyph)
					{
						case GlyphType.Arrow:
							Forms.ControlPaint.DrawMenuGlyph(g, r, Forms.MenuGlyph.Arrow, c, Drawing.Color.Transparent);

							break;
						case GlyphType.Check:
							Forms.ControlPaint.DrawMenuGlyph(g, r, Forms.MenuGlyph.Checkmark, c, Drawing.Color.Transparent);

							break;
						case GlyphType.Bullet:
							Forms.ControlPaint.DrawMenuGlyph(g, r, Forms.MenuGlyph.Bullet, c, Drawing.Color.Transparent);

							break;
					}
				});

			dc.DrawRectangle(brush, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
			base.OnRender(dc);
		}

		public enum GlyphType
		{
			Arrow,
			Check,
			Bullet,
		}
	}
}
