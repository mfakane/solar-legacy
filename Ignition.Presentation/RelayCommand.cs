using System;
using System.Windows.Input;

namespace Ignition.Presentation
{
	public class RelayCommand : RelayCommand<object>
	{
		public RelayCommand(Action<object> execute)
			: this(null, execute)
		{
		}

		public RelayCommand(Func<object, bool> canExecute, Action<object> execute)
			: base(canExecute, execute)
		{
		}

		public RelayCommand(Func<object, object> execute)
			: this((Action<object>)(_ => execute(_)))
		{
		}

		public RelayCommand(Func<object, bool> canExecute, Func<object, object> execute)
			: this(_ => canExecute(_), (Action<object>)(_ => execute(_)))
		{
		}
	}

	public class RelayCommand<T> : ICommand
	{
		readonly Action<T> execute;
		readonly Func<T, bool> canExecute;

		public RelayCommand(Action<T> execute)
			: this(null, execute)
		{
		}

		public RelayCommand(Func<T, bool> canExecute, Action<T> execute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			this.execute = execute;
			this.canExecute = canExecute;
		}

		public RelayCommand(Func<T, object> execute)
			: this((Action<T>)(_ => execute(_)))
		{
		}

		public RelayCommand(Func<T, bool> canExecute, Func<T, object> execute)
			: this(_ => canExecute(_), (Action<T>)(_ => execute(_)))
		{
		}

		public bool CanExecute(object parameter)
		{
			return canExecute == null ? true : canExecute((T)parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}

		public void Execute(object parameter)
		{
			execute((T)parameter);
		}
	}
}
