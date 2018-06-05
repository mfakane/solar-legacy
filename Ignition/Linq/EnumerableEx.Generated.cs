using System;
using System.Collections.Generic;

namespace Ignition.Linq
{
	partial class EnumerableEx
	{
	
		public static void Run<T1>(this IEnumerable<Action<T1>> self, T1 arg1)
		{
			self.Run(_ => _(arg1));
		}
	
		public static void Run<T1, T2>(this IEnumerable<Action<T1, T2>> self, T1 arg1, T2 arg2)
		{
			self.Run(_ => _(arg1, arg2));
		}
	
		public static void Run<T1, T2, T3>(this IEnumerable<Action<T1, T2, T3>> self, T1 arg1, T2 arg2, T3 arg3)
		{
			self.Run(_ => _(arg1, arg2, arg3));
		}
	
		public static void Run<T1, T2, T3, T4>(this IEnumerable<Action<T1, T2, T3, T4>> self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			self.Run(_ => _(arg1, arg2, arg3, arg4));
		}
	
		public static void Run<T1, T2, T3, T4, T5>(this IEnumerable<Action<T1, T2, T3, T4, T5>> self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			self.Run(_ => _(arg1, arg2, arg3, arg4, arg5));
		}
	
		public static void Run<T1, T2, T3, T4, T5, T6>(this IEnumerable<Action<T1, T2, T3, T4, T5, T6>> self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			self.Run(_ => _(arg1, arg2, arg3, arg4, arg5, arg6));
		}
	
		public static void Run<T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<Action<T1, T2, T3, T4, T5, T6, T7>> self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			self.Run(_ => _(arg1, arg2, arg3, arg4, arg5, arg6, arg7));
		}
	
		public static void Run<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnumerable<Action<T1, T2, T3, T4, T5, T6, T7, T8>> self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			self.Run(_ => _(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));
		}
	}
}