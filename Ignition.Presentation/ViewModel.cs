using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Ignition.Presentation
{
	public abstract class ViewModel<TModel> : DynamicObject, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		static Dictionary<Type, Dictionary<string, PropertyReference>> properties = new Dictionary<Type, Dictionary<string, PropertyReference>>();
		Dictionary<string, object> container = new Dictionary<string, object>();

		Dictionary<string, PropertyReference> Cache
		{
			get
			{
				return properties.ContainsKey(typeof(TModel))
					? properties[typeof(TModel)]
					: properties[typeof(TModel)] = new Dictionary<string, PropertyReference>();
			}
		}

		protected TModel Model
		{
			get;
			set;
		}

		protected bool NotifyChangedValueOnly
		{
			get;
			set;
		}

		protected ViewModel()
		{
			this.NotifyChangedValueOnly = NotifyObject.DefaultNotifyChangedValueOnly;
		}

		protected ViewModel(TModel model)
			: this()
		{
			this.Model = model;
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

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		protected void OnPropertyChanged(string name)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(name));
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (this.Model == null)
			{
				result = null;

				return false;
			}

			PropertyReference pr;

			if (this.Cache.TryGetValue(binder.Name, out pr))
			{
				if (pr.Get != null)
				{
					result = pr.Get(this.Model);

					return true;
				}
			}
			else
			{
				var prop = typeof(TModel).GetProperty(binder.Name);

				if (prop != null)
				{
					CreatePropertyReference(prop);

					return TryGetMember(binder, out result);
				}
			}

			return base.TryGetMember(binder, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			PropertyReference pr;

			if (this.Cache.TryGetValue(binder.Name, out pr))
			{
				if (pr.Set != null)
				{
					pr.Set(this.Model, value);
					OnPropertyChanged(binder.Name);

					return true;
				}
			}
			else
			{
				var prop = typeof(TModel).GetProperty(binder.Name);

				if (prop != null)
				{
					CreatePropertyReference(prop);

					return TrySetMember(binder, value);
				}
			}

			return base.TrySetMember(binder, value);
		}

		void CreatePropertyReference(PropertyInfo prop)
		{
			var parameterExpression = Expression.Parameter(typeof(object));
			var valueParameterExpression = Expression.Parameter(typeof(object));
			var convertExpression = Expression.Convert(parameterExpression, typeof(TModel));
			var get = prop.GetGetMethod();
			var set = prop.GetSetMethod();

			this.Cache.Add(prop.Name, new PropertyReference
			(
				get != null ? Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Property(convertExpression, get), typeof(object)), parameterExpression).Compile() : null,
				set != null ? Expression.Lambda<Action<object, object>>(Expression.Assign(Expression.Property(convertExpression, set), Expression.Convert(parameterExpression, prop.PropertyType)), parameterExpression, valueParameterExpression).Compile() : null
			));
		}

		class PropertyReference
		{
			public PropertyReference(Func<object, object> get, Action<object, object> set)
			{
				this.Get = get;
				this.Set = set;
			}

			public Func<object, object> Get
			{
				get;
				private set;
			}

			public Action<object, object> Set
			{
				get;
				private set;
			}
		}
	}
}
