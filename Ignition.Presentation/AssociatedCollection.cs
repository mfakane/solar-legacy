using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Ignition.Linq;

namespace Ignition.Collections
{
	public class AssociatedCollection<T, TAssociated> : NotifyCollection<T>, IDisposable
	{
		readonly ICollection<TAssociated> collection;
		readonly Func<TAssociated, T> selector;

		public AssociatedCollection(ICollection<TAssociated> collection, Func<TAssociated, T> selector)
		{
			if (!(collection is INotifyCollectionChanged))
				throw new ArgumentException("collection must implement INotifyCollectionChanged", "collection");

			this.collection = collection;
			this.selector = selector;
			((INotifyCollectionChanged)collection).CollectionChanged += AssociatedCollection_CollectionChanged;
			this.AddRange(collection.Select(selector));
		}

		void AssociatedCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					this.InsertRange(e.NewStartingIndex, e.NewItems.Cast<TAssociated>().Select(selector));

					break;
				case NotifyCollectionChangedAction.Move:
					var items = this.Skip(e.OldStartingIndex).Take(e.OldItems.Count).Freeze();

					this.InsertRange(e.NewStartingIndex, items);
					Enumerable.Range(e.OldStartingIndex + (e.NewStartingIndex < e.OldStartingIndex ? items.Count : 0), items.Count)
							  .Reverse()
							  .Run(this.RemoveAt);

					break;
				case NotifyCollectionChangedAction.Remove:
					Enumerable.Range(e.OldStartingIndex, e.OldItems.Count)
							  .Reverse()
							  .Run(this.RemoveAt);

					break;
				case NotifyCollectionChangedAction.Replace:
					e.NewItems.Cast<TAssociated>().Select(selector).Run((_, idx) => this[idx + e.OldStartingIndex] = _);

					break;
				case NotifyCollectionChangedAction.Reset:
					this.Clear();

					break;
			}
		}

		public void Dispose()
		{
			((INotifyCollectionChanged)collection).CollectionChanged -= AssociatedCollection_CollectionChanged;

			GC.SuppressFinalize(this);
		}

		~AssociatedCollection()
		{
			Dispose();
		}
	}
}
