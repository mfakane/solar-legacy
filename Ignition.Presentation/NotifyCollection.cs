using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Ignition.Linq;

namespace Ignition.Collections
{
	//[DebuggerDisplay("Count = {Count}")]
	//[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	public class NotifyCollection<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged
	{
		readonly InternalObservableCollection list = new InternalObservableCollection();
		readonly ReaderWriterLockSlim rwl = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		int suspend = 0;
		bool changed = false;

		protected Dispatcher Dispatcher
		{
			get;
			private set;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanging;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public NotifyCollection()
		{
			this.Dispatcher = Application.Current.Dispatcher ?? Dispatcher.CurrentDispatcher;
			list.CollectionChanging += (sender, e) => this.OnCollectionChanging(e);
			list.CollectionChanged += (sender, e) => this.OnCollectionChanged(e);
			list.PublicPropertyChanged += (sender, e) => this.OnPropertyChanged(e);
		}

		public NotifyCollection(IEnumerable<T> collection)
			: this()
		{
			this.AddRange(collection);
		}

		public IDisposable SuspendNotification(bool notifyReset, bool checkRealChanges = false)
		{
			changed = false;

			return FinallyBlock.Create(Interlocked.Increment(ref suspend), _ =>
			{
				Interlocked.Decrement(ref suspend);

				if (notifyReset && (!checkRealChanges || changed))
				{
					Action action = () => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

					if (this.Dispatcher.CheckAccess())
						action();
					else
						this.Dispatcher.Invoke(action);
				}

			});
		}

		protected virtual void OnCollectionChanging(NotifyCollectionChangedEventArgs e)
		{
			changed = true;

			if (suspend <= 0 && CollectionChanging != null)
				CollectionChanging(this, e);
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (suspend <= 0 && CollectionChanged != null)
				CollectionChanged(this, e);
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		public void AddRange(IEnumerable<T> collection)
		{
			collection.Run(this.Add);
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			collection.Run((_, i) => this.Insert(index + i, _));
		}

		public int IndexOf(T item)
		{
			using (AcquireReaderLock())
				return list.IndexOf(item);
		}

		public void Move(int oldIndex, int newIndex)
		{
			if (this.Dispatcher.CheckAccess())
				using (AcquireWriterLock())
					list.Move(oldIndex, newIndex);
			else
				this.Dispatcher.Invoke((Action<int, int>)Move, oldIndex, newIndex);
		}

		public virtual void Insert(int index, T item)
		{
			if (this.Dispatcher.CheckAccess())
				using (AcquireWriterLock())
					list.Insert(index, item);
			else
				this.Dispatcher.Invoke((Action<int, T>)Insert, index, item);
		}

		public void RemoveAt(int index)
		{
			if (this.Dispatcher.CheckAccess())
				using (AcquireWriterLock())
					list.RemoveAt(index);
			else
				this.Dispatcher.Invoke((Action<int>)RemoveAt, index);
		}

		public T this[int index]
		{
			get
			{
				using (AcquireReaderLock())
					return list[index];
			}
			set
			{
				if (this.Dispatcher.CheckAccess())
					using (AcquireWriterLock())
						list[index] = value;
				else
					this.Dispatcher.Invoke((Action)(() => this[index] = value));
			}
		}

		public virtual void Add(T item)
		{
			if (this.Dispatcher.CheckAccess())
				using (AcquireWriterLock())
					list.Add(item);
			else
				this.Dispatcher.Invoke((Action<T>)Add, item);
		}

		public void Clear()
		{
			if (this.Dispatcher.CheckAccess())
				using (AcquireWriterLock())
					list.Clear();
			else
				this.Dispatcher.Invoke((Action)Clear);
		}

		public bool Contains(T item)
		{
			using (AcquireReaderLock())
				return list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			using (AcquireReaderLock())
				list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				using (AcquireReaderLock())
					return list.Count;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(T item)
		{
			if (this.Dispatcher.CheckAccess())
				using (AcquireWriterLock())
					return list.Remove(item);
			else
				return (bool)this.Dispatcher.Invoke((Func<T, bool>)Remove, item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			using (AcquireReaderLock())
				return list.Freeze().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		int IList.Add(object value)
		{
			this.Add((T)value);

			return this.IndexOf((T)value);
		}

		void IList.Clear()
		{
			this.Clear();
		}

		bool IList.Contains(object value)
		{
			return this.Contains((T)value);
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf((T)value);
		}

		void IList.Insert(int index, object value)
		{
			this.Insert(index, (T)value);
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void IList.Remove(object value)
		{
			this.Remove((T)value);
		}

		void IList.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (T)value;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.CopyTo((T[])array, index);
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				using (AcquireReaderLock())
					return ((IList)list).IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				using (AcquireReaderLock())
					return ((IList)list).SyncRoot;
			}
		}

		IDisposable AcquireReaderLock()
		{
			return rwl.AcquireReaderLock();
		}

		IDisposable AcquireWriterLock()
		{
			return rwl.AcquireWriterLock();
		}

		class InternalObservableCollection : ObservableCollection<T>
		{
			public event NotifyCollectionChangedEventHandler CollectionChanging;

			public event PropertyChangedEventHandler PublicPropertyChanged
			{
				add
				{
					PropertyChanged += value;
				}
				remove
				{
					PropertyChanged -= value;
				}
			}

			protected virtual void OnCollectionChanging(NotifyCollectionChangedEventArgs e)
			{
				if (CollectionChanging != null)
					CollectionChanging(this, e);
			}

			protected override void SetItem(int index, T item)
			{
				OnCollectionChanging(new NotifyCollectionChangedEventArgs
				(
					NotifyCollectionChangedAction.Replace,
					item,
					this.Count > index ? this[index] : default(T),
					index
				));

				base.SetItem(index, item);
			}

			protected override void ClearItems()
			{
				OnCollectionChanging(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

				base.ClearItems();
			}

			protected override void RemoveItem(int index)
			{
				OnCollectionChanging(new NotifyCollectionChangedEventArgs
				(
					NotifyCollectionChangedAction.Remove,
					this[index],
					index
				));

				base.RemoveItem(index);
			}

			protected override void MoveItem(int oldIndex, int newIndex)
			{
				OnCollectionChanging(new NotifyCollectionChangedEventArgs
				(
					NotifyCollectionChangedAction.Move,
					this[oldIndex],
					newIndex,
					oldIndex
				));

				base.MoveItem(oldIndex, newIndex);
			}

			protected override void InsertItem(int index, T item)
			{
				OnCollectionChanging(new NotifyCollectionChangedEventArgs
				(
					NotifyCollectionChangedAction.Add,
					item,
					index
				));

				base.InsertItem(index, item);
			}
		}
	}
}