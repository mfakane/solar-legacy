using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using VisualStyles = System.Windows.Forms.VisualStyles;

namespace Ignition.Presentation
{
	public class VisualStylesPanel : Grid
	{
		Dictionary<Tuple<string, int, int>, Brush> brush = new Dictionary<Tuple<string, int, int>, Brush>();

		public static readonly DependencyProperty ClassProperty = DependencyProperty.Register("Class", typeof(string), typeof(VisualStylesPanel), new UIPropertyMetadata(null, UpdateBackground));
		public static readonly DependencyProperty PartProperty = DependencyProperty.Register("Part", typeof(int), typeof(VisualStylesPanel), new UIPropertyMetadata(1, UpdateBackground));
		public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(int), typeof(VisualStylesPanel), new UIPropertyMetadata(1, UpdateBackground));

		public int State
		{
			get
			{
				return (int)GetValue(StateProperty);
			}
			set
			{
				SetValue(StateProperty, value);
			}
		}

		public int Part
		{
			get
			{
				return (int)GetValue(PartProperty);
			}
			set
			{
				SetValue(PartProperty, value);
			}
		}

		public string Class
		{
			get
			{
				return (string)GetValue(ClassProperty);
			}
			set
			{
				SetValue(ClassProperty, value);
			}
		}

		Brush Render(Action<VisualStyles.VisualStyleRenderer, Drawing.Graphics, Drawing.Rectangle> render)
		{
			var width = (int)this.ActualWidth;
			var height = (int)this.ActualHeight;

			if (width <= 0 || height <= 0)
				return Brushes.Transparent;

			var rect = new Drawing.Rectangle(0, 0, width, height);
			Drawing.Bitmap black;

			using (var b = new Drawing.Bitmap(width, height))
			using (var g = Drawing.Graphics.FromImage(b))
			{
				using (var bg = Drawing.BufferedGraphicsManager.Current.Allocate(g, rect))
				{
					bg.Graphics.Clear(Drawing.Color.Black);
					render(new VisualStyles.VisualStyleRenderer(this.Class, this.Part, this.State), bg.Graphics, rect);
					bg.Render();
					black = (Drawing.Bitmap)b.Clone();
					bg.Graphics.Clear(Drawing.Color.White);
					render(new VisualStyles.VisualStyleRenderer(this.Class, this.Part, this.State), bg.Graphics, rect);
					bg.Render();
				}

				using (var bb = new LockBits(black, rect, Drawing.Imaging.ImageLockMode.ReadOnly, b.PixelFormat))
				using (var wb = new LockBits(b, rect, Drawing.Imaging.ImageLockMode.ReadWrite, b.PixelFormat))
					unsafe
					{
						for (int y = 0; y < wb.Data.Height * wb.Data.Stride; y += wb.Data.Stride)
							for (int x = 0; x < wb.Data.Stride; x += 4)
							{
								var bp = (byte*)bb.Data.Scan0.ToPointer() + x + y;
								var B0 = bp[0];
								var G0 = bp[1];
								var R0 = bp[2];
								var wp = (byte*)wb.Data.Scan0.ToPointer() + x + y;
								var B1 = &wp[0];
								var G1 = &wp[1];
								var R1 = &wp[2];
								var A1 = &wp[3];

								*A1 = (byte)(R0 - *R1 + 255);

								if (*A1 != 0)
								{
									*R1 = (byte)(R0 * 255 / *A1);
									*G1 = (byte)(G0 * 255 / *A1);
									*B1 = (byte)(B0 * 255 / *A1);
								}
							}
					}

				black.Dispose();

				using (var bits = new LockBits(b, rect, Drawing.Imaging.ImageLockMode.ReadOnly, b.PixelFormat))
					return new ImageBrush(BitmapSource.Create(b.Width, b.Height, g.DpiX, g.DpiY, PixelFormats.Bgra32, null, bits.Data.Scan0, bits.Data.Stride * b.Height, bits.Data.Stride));
			}
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			brush.Clear();
			base.OnRenderSizeChanged(sizeInfo);
		}

		static void UpdateBackground(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var vsp = (VisualStylesPanel)d;

			vsp.InvalidateVisual();
		}

		protected override void OnRender(DrawingContext dc)
		{
			var key = Tuple.Create(this.Class, this.Part, this.State);
			var b = brush.ContainsKey(key)
				? brush[key]
				: brush[key] = Render((vsr, g, rect) => vsr.DrawBackground(g, rect));

			dc.DrawRectangle(b, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
			base.OnRender(dc);
		}
	}
}
