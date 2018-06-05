using System;
using System.Linq;
using System.Reflection;

namespace Ignition
{
	public static class Equatable
	{
		public static int GetMemberwiseHashCode(object obj)
		{
			return obj.GetType().GetProperties()
								.Where(_ => _.CanRead && !_.GetIndexParameters().Any())
								.Select(_ => _.GetValue(obj, null))
								.Select(_ => _ == null ? 0 : _.GetHashCode())
								.Aggregate((from, i) => from ^ i);
		}

		public static bool MemberwiseEquals<T>(this T self, T target, Func<PropertyInfo, bool> where)
		{
			var props = self.GetType()
							.GetProperties()
							.Where(where)
							.Where(_ => _.CanRead && !_.GetIndexParameters().Any());

			return target != null
				&& self.GetType().IsAssignableFrom(target.GetType())
				&& props.Select(_ => _.GetValue(self, null))
						.SequenceEqual(props.Where(where).Select(_ => _.GetValue(target, null)));
		}

		public static bool MemberwiseEquals<T>(this T self, T target)
		{
			return self.MemberwiseEquals(target, _ => true);
		}
	}
}