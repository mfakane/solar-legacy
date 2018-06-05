using System;

namespace Ignition.Presentation
{
	public class DisposableEventHandler<T> : IDisposable
	{
		Action<T> add;
		Action<T> remove;
		T handler;

		public DisposableEventHandler(Action<T> add, Action<T> remove, T handler)
		{
			add(handler);
			this.add = add;
			this.remove = remove;
			this.handler = handler;
		}

		public void Dispose()
		{
			if (remove != null && handler != null)
				remove(handler);

			GC.SuppressFinalize(this);
		}

		~DisposableEventHandler()
		{
			Dispose();
		}
	}

	public static class DisposableEventHandler
	{
		public static DisposableEventHandler<T> Create<T>(Action<T> add, Action<T> remove, T handler)
		{
			return new DisposableEventHandler<T>(add, remove, handler);
		}

		public static DisposableEventHandler<EventHandler<TEventArgs>> Create<TEventArgs>(Action<EventHandler<TEventArgs>> add, Action<EventHandler<TEventArgs>> remove, EventHandler<TEventArgs> handler)
			where TEventArgs : EventArgs
		{
			return new DisposableEventHandler<EventHandler<TEventArgs>>(add, remove, handler);
		}
	}
}
