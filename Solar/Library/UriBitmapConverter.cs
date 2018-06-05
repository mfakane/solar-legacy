using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ignition;
using Ignition.Presentation;
using Solar.Models;

namespace Solar
{
#pragma warning disable 1591

	class UriBitmapConverter : OneWayValueConverter<Uri, BitmapImage>
	{
		static readonly ConcurrentDictionary<Uri, CacheValue> images = new ConcurrentDictionary<Uri, CacheValue>();
		const int MaxImages = 2000;

		public static int CacheCount
		{
			get
			{
				return images.Count;
			}
		}

		public static void Clean()
		{
			if (images.Count > MaxImages)
			{
				lock (images)
				{
					var items = images.AsParallel().Where(_ => _.Value.LastReference < DateTime.Now - TimeSpan.FromHours(1)).OrderByDescending(x => x.Value.ReferenceCount);

					items.Take(items.Count() - MaxImages).ForAll(x => images.Remove(x.Key));
				}

				GC.Collect();
			}
		}

		public static void Clear()
		{
			images.Clear();
		}

		protected override BitmapImage ConvertFromSource(Uri value, object parameter)
		{
			if (value == null)
				return null;
			else if (images.ContainsKey(value))
				return images[value].Value;

			Clean();

			try
			{
				lock (value)
					if (!string.IsNullOrEmpty(Settings.Default.Interface.IconCacheLocation))
						using (var md5 = MD5.Create())
						{
							var dir = new DirectoryInfo(Settings.Default.Interface.IconCacheLocation);
							var name = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(value.AbsoluteUri))).Replace("-", "");
							MemoryStream ms;

							foreach (var i in Path.GetInvalidFileNameChars())
								name = name.Replace(i, '_');

							var path = Path.Combine(dir.FullName, name);

							if (!dir.Exists)
								dir.Create();

							if (!File.Exists(path))
								using (var wc = new WebClient
								{
									Headers =
									{
										{ HttpRequestHeader.UserAgent, "Solar" },
									},
								})
								using (var ns = wc.OpenRead(value))
								{
									ms = ns.Freeze();
									File.WriteAllBytes(path, ms.ToArray());
								}
							else
								using (var fs = File.OpenRead(path))
									ms = fs.Freeze();
							
							using (ms)
							{
								var rt = new BitmapImage();

								rt.BeginInit();
								rt.StreamSource = ms;
								rt.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
								rt.CacheOption = BitmapCacheOption.OnLoad;
								rt.EndInit();
								RenderOptions.SetBitmapScalingMode(rt, BitmapScalingMode.NearestNeighbor);

								if (rt.CanFreeze)
									rt.Freeze();

								return images.AddOrUpdate(value, new CacheValue(rt), (_, oldValue) => new CacheValue(rt)).Value;
							}
						}
					else

						using (var wc = new WebClient
						{
							Headers =
							{
								{ HttpRequestHeader.UserAgent, "Solar" },
							},
						})
						using (var ns = wc.OpenRead(value))
						using (var ms = ns.Freeze())
						{
							var rt = new BitmapImage();

							rt.BeginInit();
							rt.StreamSource = ms;
							rt.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
							rt.CacheOption = BitmapCacheOption.OnLoad;
							rt.EndInit();
							RenderOptions.SetBitmapScalingMode(rt, BitmapScalingMode.NearestNeighbor);

							if (rt.CanFreeze)
								rt.Freeze();

							return images.AddOrUpdate(value, new CacheValue(rt), (_, oldValue) => new CacheValue(rt)).Value;
						}
			}
			catch (Exception ex)
			{
				App.Log(ex);

				return null;
			}
		}

		class CacheValue
		{
			readonly BitmapImage value;

			public CacheValue(BitmapImage value)
			{
				this.value = value;
			}

			public int ReferenceCount
			{
				get;
				private set;
			}

			public DateTime LastReference
			{
				get;
				private set;
			}

			public BitmapImage Value
			{
				get
				{
					this.ReferenceCount++;
					this.LastReference = DateTime.Now;

					return value;
				}
			}
		}
	}

#pragma warning restore 1591
}
