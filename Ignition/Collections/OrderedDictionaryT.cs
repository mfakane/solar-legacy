using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Ignition.Linq;

namespace Ignition.Collections
{
	public class OrderedDictionary<TKey, TValue> : OrderedDictionary, IDictionary<TKey, TValue>, IList<KeyValuePair<TKey, TValue>>
	{
		public void Add(TKey key, TValue value)
		{
			base.Add(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			return base.Contains(key);
		}

		public new ICollection<TKey> Keys
		{
			get
			{
				return base.Keys.Cast<TKey>()
								.Freeze();
			}
		}

		public bool Remove(TKey key)
		{
			if (this.Contains(key))
			{
				base.Remove(key);

				return true;
			}
			else
				return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (this.Contains(key))
			{
				value = this[key];

				return true;
			}
			else
			{
				value = default(TValue);

				return false;
			}
		}

		public new ICollection<TValue> Values
		{
			get
			{
				return base.Values.Cast<TValue>()
								  .Freeze();
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				return (TValue)base[key];
			}
			set
			{
				base[key] = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public new object this[object key]
		{
			get
			{
				return (TValue)base[key];
			}
			set
			{
				base[key] = value;
			}
		}

		public new TValue this[int index]
		{
			get
			{
				return (TValue)base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			base.Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			if (base.Contains(item.Key))
			{
				var val = base[item.Key];

				return val == null && item.Value == null
					|| val != null && val.Equals(item.Value)
					|| item.Value != null && item.Equals(val);
			}
			else
				return false;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			Enumerable.Range(0, this.Count)
					  .Run(_ => array[arrayIndex + _] = new KeyValuePair<TKey, TValue>(this.Keys.ElementAt(_), this[_]));
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			if (((ICollection<KeyValuePair<TKey, TValue>>)this).Contains(item))
				return this.Remove(item.Key);
			else
				return false;
		}

		public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.Keys.Select(_ => new KeyValuePair<TKey, TValue>(_, this[_]))
							.GetEnumerator();
		}

		int IList<KeyValuePair<TKey, TValue>>.IndexOf(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		void IList<KeyValuePair<TKey, TValue>>.Insert(int index, KeyValuePair<TKey, TValue> item)
		{
			base.Insert(index, item.Key, item.Value);
		}

		KeyValuePair<TKey, TValue> IList<KeyValuePair<TKey, TValue>>.this[int index]
		{
			get
			{
				return new KeyValuePair<TKey, TValue>(this.Keys.ElementAt(index), this.Values.ElementAt(index));
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
