using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ModelViewViewModel.Base
{
    public class ModelMapping<TModel>
    {
        private readonly TModel model;
        private readonly PropertyInfo property;

        protected ModelMapping(TModel model, string propertyName)
        {
            this.model = model;
            Name = propertyName;
            property = this.model.GetType().GetProperty(Name);
        }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Propertyinfo.
        /// </summary>
        public object Property
        {
            get
            {
                return property.GetValue(model, null);
            }
            set
            {
                property.SetValue(model, value, null);
            }
        }
    }

    public class ModelMapping<TModel, TProperty> : ModelMapping<TModel>
        where TModel : class 
    {
        public ModelMapping(TModel model, Expression<Func<TModel, TProperty>> property)
            : base(model, PropertyName.For(property))
        {
        }
    }
}