using System;
using System.Linq;

namespace Ignition
{
	public class Pattern
	{
		Pattern(Func<object, bool> matches)
		{
			this.Matches = matches;
		}

		public static Pattern Equals<T>(T value)
		{
			return new Pattern(_ => _ == null && value == null
								 || _ != null && _.Equals(value)
								 || value.Equals(_));
		}

		public static Pattern Any<T>()
		{
			return new Pattern(_ => _.GetType().IsSubclassOf(typeof(T)));
		}

		public static Pattern Any<T>(params T[] any)
		{
			return new Pattern(_ => any.Contains((T)_));
		}

		public Func<object, bool> Matches
		{
			get;
			private set;
		}
	}
}
