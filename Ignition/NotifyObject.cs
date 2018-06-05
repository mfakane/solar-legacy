using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Ignition
{
	public class NotifyObject : INotifyPropertyChanged
	{
		Dictionary<string, object> container = new Dictionary<string, object>();

		public static bool DefaultNotifyChangedValueOnly;

		public event PropertyChangedEventHandler PropertyChanged;

		protected bool NotifyChangedValueOnly
		{
			get;
			set;
		}

		public NotifyObject()
		{
			this.NotifyChangedValueOnly = DefaultNotifyChangedValueOnly;
		}

		protected void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		protected void OnPropertyChanged<T>(string propertyName, T oldValue, T newValue)
		{
			if (!this.NotifyChangedValueOnly ||
				oldValue != null && !oldValue.Equals(newValue) ||
				newValue != null && !newValue.Equals(oldValue))
				OnPropertyChanged(propertyName);
		}

		protected T GetValue<T>(Expression<Func<T>> property)
		{
			var expr = property.Body.NodeType == ExpressionType.Convert
				? (MemberExpression)((UnaryExpression)property.Body).Operand
				: (MemberExpression)property.Body;
			var member = expr.Member.Name;

			return container.ContainsKey(member)
				? (T)container[member]
				: default(T);
		}

		protected T GetValue<T>(string name)
		{
			return container.ContainsKey(name)
				? (T)container[name]
				: default(T);
		}

		protected void SetValue<T>(Expression<Func<T>> property, T value)
		{
			SetValue(property, value, this.NotifyChangedValueOnly);
		}

		protected void SetValue<T>(Expression<Func<T>> property, T value, bool notifyChangedValueOnly)
		{
			var expr = property.Body.NodeType == ExpressionType.Convert
				? (MemberExpression)((UnaryExpression)property.Body).Operand
				: (MemberExpression)property.Body;
			var member = expr.Member.Name;

			if (notifyChangedValueOnly &&
				container.ContainsKey(member) && (
				container[member] == null && value == null ||
				container[member] != null && container[member].Equals(value) ||
				value != null && value.Equals(container[member])
				))
				return;

			container[member] = value;
			OnPropertyChanged(member);
		}

		protected void SetValue<T>(string name, T value)
		{
			container[name] = value;
			OnPropertyChanged(name);
		}
	}
}
