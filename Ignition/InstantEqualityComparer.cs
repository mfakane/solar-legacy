using System;
using System.Collections.Generic;

namespace Ignition
{
	public class InstantEqualityComparer<T> : IEqualityComparer<T>
	{
		Func<T, T, bool> equals;
		Func<T, int> getHashCode;

		public InstantEqualityComparer(Func<T, T, bool> equals)
			: this(equals, _ => _ == null ? 0 : _.GetHashCode())
		{
		}

		public InstantEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
		{
			this.equals = equals;
			this.getHashCode = getHashCode;
		}

		public InstantEqualityComparer(Func<T, object> selector)
			: this((x, y) =>
			{
				var a = selector(x);
				var b = selector(y);

				return a == b
					|| x != null && y != null && a.Equals(b);
			},
			_ => selector(_).GetHashCode())
		{
		}

		public bool Equals(T x, T y)
		{
			return equals(x, y);
		}

		public int GetHashCode(T obj)
		{
			return getHashCode(obj);
		}
	}
}
