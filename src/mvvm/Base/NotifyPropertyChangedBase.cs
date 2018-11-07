using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ModelViewViewModel.Base
{
    public abstract class NotifyPropertyChangedBase<T> : INotifyPropertyChanged where T : class, INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> backingDictionary;
        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected NotifyPropertyChangedBase()
        {
            backingDictionary = new Dictionary<string, object>();
        }

        protected internal virtual void InvokePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected internal virtual string GetPropertyName<TProperty>(Expression<Func<T, TProperty>> prop)
        {
            return PropertyName.For(prop);
        }

        protected internal virtual void InvokePropertyChanged<TProperty>(Expression<Func<T, TProperty>> prop)
        {
            InvokePropertyChanged(GetPropertyName(prop));
        }

        protected virtual TProperty Get<TProperty>(Expression<Func<T, TProperty>> prop)
        {
            string propertyName = GetPropertyName(prop);
            if (backingDictionary.ContainsKey(propertyName))
            {
                return (TProperty)backingDictionary[propertyName];
            }

            return default(TProperty);
        }

        protected virtual void Set<TProperty>(Expression<Func<T, TProperty>> prop, TProperty value)
        {
            string propertyName = GetPropertyName(prop);
            backingDictionary[propertyName] = value;
            InvokePropertyChanged(propertyName);
        }

        protected internal virtual void NotifyOnChange<TChangedObjekt, TChangedProperty, TPropertyToNotify>(TChangedObjekt changedObjekt, Expression<Func<TChangedObjekt, TChangedProperty>> changedProperty, Expression<Func<T, TPropertyToNotify>> propertyToNotify)
            where TChangedObjekt : class, INotifyPropertyChanged
        {
            var changedPropertyName = PropertyName.For(changedProperty);
            PropertyChangedEventHandler changedObjektOnPropertyChanged = (o, e) =>
            {
                if (e.PropertyName.Equals(changedPropertyName))
                {
                    InvokePropertyChanged(propertyToNotify);
                }
            };

            changedObjekt.PropertyChanged += changedObjektOnPropertyChanged;
        }
    }
}