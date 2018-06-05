using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Ignition.Linq;

namespace Ignition
{
	public static partial class Extension
	{
		public static bool Between(this int self, int min, int max)
		{
			return self > min && self < max;
		}

		public static string Place(this string self, params object[] args)
		{
			return string.Format(self, args);
		}

		public static IEnumerable<string> EnumerateLines(this StreamReader self)
		{
			while (!self.EndOfStream)
			{
				var rt = self.ReadLine();

				if (rt == null)
					yield break;

				yield return rt;
			}
		}

		public static Dictionary<string, string> QueryToDictionary(this Uri self)
		{
			return self.Query.Split('&')
							 .Select(_ => _.Split('='))
							 .ToDictionary(_ => _.First(), _ => _.Last());
		}

		public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo self, bool inherit)
			where T : Attribute
		{
			return self.GetCustomAttributes(typeof(T), inherit).Cast<T>();
		}

		public static MemoryStream Freeze(this Stream self)
		{
			var rt = new MemoryStream();

			self.CopyTo(rt);
			rt.Seek(0, SeekOrigin.Begin);

			return rt;
		}

		public static IDisposable AcquireReaderLock(this ReaderWriterLockSlim self)
		{
			return FinallyBlock.Create(() => self.EnterReadLock(), () => self.ExitReadLock());
		}

		public static IDisposable AcquireWriterLock(this ReaderWriterLockSlim self)
		{
			return FinallyBlock.Create(() => self.EnterWriteLock(), () => self.ExitWriteLock());
		}

		[Obsolete("recommended to use ReaderWriterLockSlim instead of ReaderWriterLock")]
		public static IDisposable AcquireReaderLock(this ReaderWriterLock self)
		{
			return FinallyBlock.Create(() => self.AcquireReaderLock(Timeout.Infinite), () => self.ReleaseReaderLock());
		}

		[Obsolete("recommended to use ReaderWriterLockSlim instead of ReaderWriterLock")]
		public static IDisposable AcquireWriterLock(this ReaderWriterLock self)
		{
			return FinallyBlock.Create(() => self.AcquireWriterLock(Timeout.Infinite), () => self.ReleaseWriterLock());
		}

		public static string[] Split(this string self, params string[] separator)
		{
			return self.Split(separator, StringSplitOptions.None);
		}

		public static int Count(this string self, char c)
		{
			return self.Count(_ => _ == c);
		}

		public static bool StartsWith(this string self, params string[] any)
		{
			return any.Any(self.StartsWith);
		}

		public static bool StartsWith(this string self, StringComparison comparisonType, params string[] any)
		{
			return any.Any(_ => self.StartsWith(_, comparisonType));
		}

		public static bool EndsWith(this string self, params string[] any)
		{
			return any.Any(self.EndsWith);
		}

		public static bool EndsWith(this string self, StringComparison comparisonType, params string[] any)
		{
			return any.Any(_ => self.EndsWith(_, comparisonType));
		}

		public static void RaiseEvent(this EventHandler self, object sender, EventArgs e)
		{
			if (self != null)
				self(sender, e);
		}

		public static void RaiseEvent<T>(this EventHandler<T> self, object sender, T e)
			where T : EventArgs
		{
			if (self != null)
				self(sender, e);
		}

		public static void RaiseEvent<T>(this EventHandler<EventArgs<T>> self, object sender, T e)
		{
			if (self != null)
				self(sender, new EventArgs<T>(e));
		}

		public static T Apply<T>(this T self, params Action<T>[] values)
		{
			values.Run(self);

			return self;
		}

		public static T ApplyWhen<T>(this T self, Func<T, bool> when, params Action<T>[] values)
		{
			if (when(self))
				values.Run(self);

			return self;
		}

		public static T Apply<T>(this T self, params Func<T, T>[] values)
		{
			return values.Aggregate(self, (x, y) => y(x));
		}
	}
}
