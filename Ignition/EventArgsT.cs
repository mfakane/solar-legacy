using System;

namespace Ignition
{
	public class EventArgs<T> : EventArgs
	{
		public T Value
		{
			get;
			set;
		}

		public EventArgs(T value)
		{
			this.Value = value;
		}
	}
}
