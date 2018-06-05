using System;
using System.Collections.Concurrent;

namespace Ignition
{
	public static class ConcurrentEx
	{
		public static void Add<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> self, TKey key, TValue value)
		{
			self.AddOrUpdate(key, value, (k, v) => value);
		}

		public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> self, TKey key)
		{
			TValue value;

			return self.TryRemove(key, out value);
		}

		public static T Pop<T>(this ConcurrentStack<T> self)
		{
			T value;

			if (self.TryPop(out value))
				return value;
			else
				throw new InvalidOperationException("attempt to pop an empty stack");
		}
	}
}
