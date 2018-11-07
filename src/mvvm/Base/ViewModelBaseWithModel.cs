using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ModelViewViewModel.Base
{
    public abstract class ViewModelBase<TViewModel, TModel> : ViewModelBase<TViewModel>, IViewModelBase<TModel>
        where TViewModel : class, INotifyPropertyChanged
        where TModel : class
    {
        protected TModel model;
        protected Dictionary<string, ModelMapping<TModel>> backingModel;
        protected Dictionary<string, IList<string>> notifyOnChangeDic;
        protected Dictionary<string, IList<Action>> executeOnChangeDic;
        private readonly bool modelCanNotifyPropertyChanged;

        protected ViewModelBase(TModel model)
        {
            this.model = model;
            backingModel = new Dictionary<string, ModelMapping<TModel>>();
            notifyOnChangeDic = new Dictionary<string, IList<string>>();
            executeOnChangeDic = new Dictionary<string, IList<Action>>();
            modelCanNotifyPropertyChanged = typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(TModel));

            if (modelCanNotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)this.model).PropertyChanged += (s, e) => PropagateModelChanges(e.PropertyName);
            }
        }

        /// <summary>
        /// Maps the ViewModel Property to that of the Model.
        /// </summary>
        /// <typeparam name="TProperty">TypeProperty.</typeparam>
        /// <param name="viewmodelProperty">Property of the ViewModel.</param>
        /// <param name="modelProperty">Property of the Model.</param>
        protected virtual void Map<TProperty>(Expression<Func<TViewModel, TProperty>> viewmodelProperty, Expression<Func<TModel, TProperty>> modelProperty)
        {
            backingModel[PropertyName.For(viewmodelProperty)] = new ModelMapping<TModel, TProperty>(model, modelProperty);
            NotifyOnChange(modelProperty, viewmodelProperty);
        }

        protected override T Get<T>(Expression<Func<TViewModel, T>> viewmodelProperty)
        {
            var viewmodelPropertyName = PropertyName.For(viewmodelProperty);
            if (ViewModelPropertyIsMapped(viewmodelPropertyName))
            {
                return (T)backingModel[viewmodelPropertyName].Property;
            }

            return base.Get(viewmodelProperty);
        }

        private bool ViewModelPropertyIsMapped(string viewmodelPropertyName)
        {
            return backingModel.ContainsKey(viewmodelPropertyName);
        }

        protected override void Set<T>(Expression<Func<TViewModel, T>> viewmodelProperty, T value)
        {
            var viewmodelPropertyName = PropertyName.For(viewmodelProperty);

            if (ViewModelPropertyIsMapped(viewmodelPropertyName))
            {
                var modelProperty = backingModel[viewmodelPropertyName];
                modelProperty.Property = value;
                if (!modelCanNotifyPropertyChanged)
                {
                    PropagateModelChanges(modelProperty.Name);
                }
            }
            else
            {
                base.Set(viewmodelProperty, value);
            }
        }

        protected virtual void NotifyOnChange<TModelProperty, TViewModelProperty>(Expression<Func<TModel, TModelProperty>> modelProperty, Expression<Func<TViewModel, TViewModelProperty>> viewmodelProperty)
        {
            var modelPropertyName = PropertyName.For(modelProperty);
            if (!notifyOnChangeDic.ContainsKey(modelPropertyName))
            {
                notifyOnChangeDic[modelPropertyName] = new List<string>();
            }

            notifyOnChangeDic[modelPropertyName].Add(PropertyName.For(viewmodelProperty));
        }

        protected virtual void ExecuteOnChange<TProperty>(Expression<Func<TModel, TProperty>> modelProperty, Action executeAction)
        {
            var modelPropertyName = PropertyName.For(modelProperty);
            if (!executeOnChangeDic.ContainsKey(modelPropertyName))
            {
                executeOnChangeDic[modelPropertyName] = new List<Action>();
            }

            executeOnChangeDic[modelPropertyName].Add(executeAction);
        }

        protected virtual void MapViewModelPropertyWrapper<TWrapperProperty, TWrappedType>(Expression<Func<TViewModel, TWrapperProperty>> viewmodelProperty, Expression<Func<TModel, TWrappedType>> modelProperty)
            where TWrapperProperty : IViewModelPropertyWrapper<TWrappedType>
        {
            var modelMappingObject = new ModelMapping<TModel, TWrappedType>(model, modelProperty);
            var viewModelPropertyWrapper = new ViewModelPropertyWrapper<TWrappedType>(() => (TWrappedType)modelMappingObject.Property, value => modelMappingObject.Property = value);
            ExecuteOnChange(modelProperty, viewModelPropertyWrapper.Reset);

            MapViewModelPropertyWrapper(viewmodelProperty, viewModelPropertyWrapper);
        }

        internal virtual void PropagateModelChanges(string modelPropertyName)
        {
            if (executeOnChangeDic.ContainsKey(modelPropertyName))
            {
                foreach (var action in executeOnChangeDic[modelPropertyName])
                {
                    action();
                }
            }

            if (notifyOnChangeDic.ContainsKey(modelPropertyName))
            {
                foreach (var name in notifyOnChangeDic[modelPropertyName])
                {
                    InvokePropertyChanged(name);
                }
            }
        }

        /// <summary>
        /// Checks wether the matchingModel is equal to the current model.
        /// </summary>
        /// <param name="matchingModel">Other Model.</param>
        /// <returns>Wether the matchingModel is equal to the current model or not.</returns>
        public virtual bool IsFor(TModel matchingModel)
        {
            return model.Equals(matchingModel);
        }
    }
}