using System.Collections.Generic;

namespace Ignition.Collections
{
	public class SortedNotifyCollection<T> : NotifyCollection<T>
	{
		public IComparer<T> Comparer
		{
			get;
			set;
		}

		public SortedNotifyCollection()
			: this(null)
		{
		}

		public SortedNotifyCollection(IComparer<T> comparer)
		{
			this.Comparer = comparer ?? Comparer<T>.Default;
		}

		public override void Add(T item)
		{
			base.Insert(BinarySearch(0, this.Count, item).Apply(_ => _ < 0 ? ~_ : _), item);
		}

		int BinarySearch(int index, int length, T value)
		{
			var i = index;
			var max = index + length - 1;
			var comparer = this.Comparer;

			while (i <= max)
			{
				var m = i + (max - i >> 1);
				var rt = comparer.Compare(this[m], value);

				if (rt == 0)
					return m;
				else if (rt < 0)
					i = m + 1;
				else
					max = m - 1;
			}

			return ~i;
		}
	}
}
