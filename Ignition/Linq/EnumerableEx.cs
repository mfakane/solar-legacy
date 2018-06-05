using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Ignition.Linq
{
	public static partial class EnumerableEx
	{
		public static IEnumerable<T> Repeat<T>(T element)
		{
			while (true)
				yield return element;
		}

		public static IEnumerable<T> Wrap<T>(T value)
		{
			return Enumerable.Repeat(value, 1);
		}

		public static IEnumerable<T> Do<T>(this IEnumerable<T> self, Action<T> action)
		{
			return self.Select(_ =>
			{
				action(_);

				return _;
			});
		}

		public static IEnumerable<T> Do<T>(this IEnumerable<T> self, Action<T, int> action)
		{
			return self.Select((_, i) =>
			{
				action(_, i);

				return _;
			});
		}

		public static void Run<T>(this IEnumerable<T> self)
		{
			self.RunWhile(_ => true);
		}
		public static void Run<T>(this IEnumerable<T> self, Action<T> action)
		{
			foreach (var i in self)
				action(i);
		}

		public static bool RunWhile<T>(this IEnumerable<T> self, Func<T, bool> predicate)
		{
			foreach (var i in self)
				if (!predicate(i))
					return false;

			return true;
		}
	}
}
