using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Ignition.Presentation
{
	class LockBits : IDisposable
	{
		Bitmap bitmap;

		public BitmapData Data
		{
			get;
			private set;
		}

		public LockBits(Bitmap bitmap, Rectangle rect, ImageLockMode mode, PixelFormat format)
		{
			this.bitmap = bitmap;
			this.Data = this.bitmap.LockBits(rect, mode, format);
		}

		public void Dispose()
		{
			bitmap.UnlockBits(this.Data);
			GC.SuppressFinalize(this);
		}

		~LockBits()
		{
			Dispose();
		}
	}
}
