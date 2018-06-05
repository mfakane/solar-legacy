using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Ignition.Linq;

namespace Ignition
{
	public static class Enumerables
	{
		public static IEnumerable<T> PreProcess<T>(this IEnumerable<T> self, Action action)
		{
			action();

			foreach (var i in self)
				yield return i;
		}

		public static IEnumerable<T> PostProcess<T>(this IEnumerable<T> self, Action action)
		{
			foreach (var i in self)
				yield return i;

			action();
		}

		public static bool StartsWith<T>(this IEnumerable<T> self, IEnumerable<T> seq)
		{
			return self.Take(seq.Count()).SequenceEqual(seq);
		}

		public static bool EndsWith<T>(this IEnumerable<T> self, IEnumerable<T> seq)
		{
			return self.Skip(self.Count() - seq.Count()).SequenceEqual(seq);
		}

		public static string Join<T>(this IEnumerable<T> self, string separator)
		{
			return string.Join(separator, self);
		}

		public static IEnumerable<T> Replace<T>(this IEnumerable<T> self, T from, T to)
		{
			foreach (var i in self)
				if (i.Equals(from))
					yield return to;
				else
					yield return i;
		}

		public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> self)
		{
			return self.SelectMany(_ => _);
		}

		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> self, T value)
		{
			return Enumerable.Concat(EnumerableEx.Wrap(value), self);
		}

		public static IEnumerable<T> Append<T>(this IEnumerable<T> self, T value)
		{
			return Enumerable.Concat(self, EnumerableEx.Wrap(value));
		}

		public static Collection<T> ToCollection<T>(this IEnumerable<T> self)
		{
			if (self is IList<T>)
				return new Collection<T>((IList<T>)self);
			else
				return new Collection<T>(self.Freeze());
		}

		public static IList<T> Freeze<T>(this IEnumerable<T> self)
		{
			if (self is List<T>)
				return ((List<T>)self).ToArray();
			else if (self is Queue<T>)
				return ((Queue<T>)self).ToArray();
			else if (self is Stack<T>)
				return ((Stack<T>)self).ToArray();
			else if (self is T[])
				return (T[])self;
			else
				return self.ToArray();
		}

		public static T Single<T>(this IList<T> self)
		{
			if (self.Count == 1)
				return self[0];
			else
				throw new InvalidOperationException();
		}

		public static T SingleOrDefault<T>(this IList<T> self)
		{
			if (self.Count == 0)
				return default(T);
			else if (self.Count == 1)
				return self[0];
			else
				throw new InvalidOperationException();
		}

		public static bool Any<T>(this IList<T> self)
		{
			return self.Count > 0;
		}

		public static void Run<T>(this IEnumerable<T> self, Action<T, int> action)
		{
			var idx = 0;

			foreach (var i in self)
				action(i, idx++);
		}

		public static IEnumerable<IEnumerable<T>> TakePairs<T>(this IEnumerable<T> source)
		{
			return source.TakePairs(2);
		}
		public static IEnumerable<IEnumerable<T>> TakePairs<T>(this IEnumerable<T> source, int each)
		{
			var rt = new LinkedList<T>();

			foreach (var i in source)
			{
				rt.AddLast(i);
				if (rt.Count >= each)
				{
					yield return rt;

					rt = new LinkedList<T>();
				}
			}

			if (rt.Any())
				yield return rt;
		}

		public static IEnumerable<T> Matches<T, TPattern>(this IEnumerable<T> source, TPattern pattern)
		{
			var pl = typeof(TPattern).GetProperties()
									 .Select(_ => new
									 {
										 _.Name,
										 Value = _.GetValue(pattern, null)
									 })
									 .Select(_ => new
									 {
										 _.Name,
										 Predicate = _.Value is Pattern
											 ? ((Pattern)_.Value).Matches
											 : Pattern.Equals(_.Value).Matches
									 });

			return source.Where(_ =>
			{
				var type = _.GetType();

				foreach (var i in pl)
				{
					var members = type.GetMember(i.Name);

					if (members.Any())
						try
						{
							return i.Predicate(type.InvokeMember(i.Name, BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.GetField | BindingFlags.GetProperty, null, _, null));
						}
						catch
						{
							return false;
						}
					else
						return false;
				}

				return false;
			});
		}
	}
}
