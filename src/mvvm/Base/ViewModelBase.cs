using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace ModelViewViewModel.Base
{
    public abstract class ViewModelBase<TViewModel> : NotifyPropertyChangedBase<TViewModel>, IDataErrorInfo
        where TViewModel : class, INotifyPropertyChanged
    {
        private readonly Dictionary<string, IViewModelPropertyWrapper> viewModelWrapper;
        private readonly Dictionary<string, IList<ValidationRule>> validationRules;
        private readonly Dictionary<string, Func<IDataErrorInfo>> validationPropertyFuncs;
        private readonly Dictionary<string, Func<IEnumerable<IDataErrorInfo>>> validationPropertyFuncCollection;

        protected ViewModelBase()
        {
            validationPropertyFuncCollection = new Dictionary<string, Func<IEnumerable<IDataErrorInfo>>>();
            validationPropertyFuncs = new Dictionary<string, Func<IDataErrorInfo>>();
            validationRules = new Dictionary<string, IList<ValidationRule>>();
            viewModelWrapper = new Dictionary<string, IViewModelPropertyWrapper>();
        }

        public virtual string Error
        {
            get
            {
                foreach (KeyValuePair<string, Func<IDataErrorInfo>> keyValuePair in validationPropertyFuncs)
                {
                    Func<IDataErrorInfo> function = keyValuePair.Value;
                    IDataErrorInfo dataErrorInfo = function();

                    if (dataErrorInfo != null)
                    {
                        string error = dataErrorInfo.Error;
                        if (!string.IsNullOrEmpty(error))
                        {
                            return error;
                        }
                    }
                }

                foreach (KeyValuePair<string, Func<IEnumerable<IDataErrorInfo>>> keyValuePair in validationPropertyFuncCollection)
                {
                    Func<IEnumerable<IDataErrorInfo>> functions = keyValuePair.Value;
                    IEnumerable<IDataErrorInfo> dataErrorInfos = functions();

                    if (dataErrorInfos != null)
                    {
                        foreach (IDataErrorInfo dataErrorInfo in dataErrorInfos)
                        {
                            if (dataErrorInfo != null)
                            {
                                string error = dataErrorInfo.Error;
                                if (!string.IsNullOrEmpty(error))
                                {
                                    return error;
                                }
                            }
                        }
                    }
                }

                return validationRules.Any(x => x.Value.Any(r => !r.Validation())) ? "Error" : string.Empty;
            }
        }

        public virtual string this[string columnName]
        {
            get
            {
                if (validationRules.ContainsKey(columnName))
                {
                    foreach (var item in validationRules[columnName])
                    {
                        if (!item.Validation())
                        {
                            return item.ErrorMessage;
                        }
                    }
                }

                return string.Empty;
            }
        }

        protected virtual void RegisterChildViewModel<TProperty>(Expression<Func<TViewModel, TProperty>> viewmodelProperty)
            where TProperty : IDataErrorInfo
        {
            string viewModelPropertyName = PropertyName.For(viewmodelProperty);
            if (validationPropertyFuncs.ContainsKey(viewModelPropertyName))
            {
                throw new NotSupportedException(string.Format("Validation for the Property '{0}' is already registered!", viewModelPropertyName));
            }

            validationPropertyFuncs[viewModelPropertyName] = () => Get(viewmodelProperty);
        }

        protected virtual void RegisterChildViewModelCollection<TProperty>(Expression<Func<TViewModel, TProperty>> viewmodelProperty)
            where TProperty : IEnumerable<IDataErrorInfo>
        {
            string viewModelPropertyName = PropertyName.For(viewmodelProperty);
            if (validationPropertyFuncCollection.ContainsKey(viewModelPropertyName))
            {
                throw new NotSupportedException(string.Format("Validation for the Property '{0}' is already registered!", viewModelPropertyName));
            }

            validationPropertyFuncCollection[viewModelPropertyName] = () => Get(viewmodelProperty);
        }

        internal void MapViewModelPropertyWrapper<TWrapperProperty>(Expression<Func<TViewModel, TWrapperProperty>> viewmodelProperty, IViewModelPropertyWrapper viewModelPropertyWrapper)
        {
            string viewmodelPropertyName = PropertyName.For(viewmodelProperty);
            if (viewModelWrapper.ContainsKey(viewmodelPropertyName))
            {
                throw new NotSupportedException("Wrapper is already setup.");
            }

            viewModelWrapper[viewmodelPropertyName] = viewModelPropertyWrapper;
            NotifyOnChange(viewModelPropertyWrapper, x => x.IsValid, viewmodelProperty);
            AddValidationRule(viewmodelProperty, new ValidationRule(() => viewModelPropertyWrapper.IsValid, "Error"));
            viewModelPropertyWrapper.SetValidation(() => this[viewmodelPropertyName]);
        }

        protected override TProperty Get<TProperty>(Expression<Func<TViewModel, TProperty>> prop)
        {
            var viewModelPropertyName = PropertyName.For(prop);
            if (PropertyIsMappedWrapper(viewModelPropertyName))
            {
                return (TProperty)viewModelWrapper[viewModelPropertyName];
            }

            return base.Get(prop);
        }

        protected override void Set<TProperty>(Expression<Func<TViewModel, TProperty>> prop, TProperty value)
        {
            var viewModelPropertyName = PropertyName.For(prop);
            if (PropertyIsMappedWrapper(viewModelPropertyName))
            {
                throw new NotSupportedException("Wrapper cannot be set.");
            }

            base.Set(prop, value);
        }

        protected virtual void SetViewModelPropertyWrapper<TWrapperProperty, TWrappedType>(Expression<Func<TViewModel, TWrapperProperty>> viewmodelWrapperProperty)
            where TWrapperProperty : IViewModelPropertyWrapper<TWrappedType>
        {
            SetViewModelPropertyWrapper(viewmodelWrapperProperty, default(TWrappedType));
        }

        protected virtual void SetViewModelPropertyWrapper<TWrapperProperty, TWrappedType>(Expression<Func<TViewModel, TWrapperProperty>> viewmodelProperty, TWrappedType initValue)
            where TWrapperProperty : IViewModelPropertyWrapper<TWrappedType>
        {
            ViewModelPropertyWrapper<TWrappedType> viewModelPropertyWrapper = new ViewModelPropertyWrapper<TWrappedType>(initValue);
            MapViewModelPropertyWrapper(viewmodelProperty, viewModelPropertyWrapper);
        }

        protected virtual void AddValidationRule<T>(Expression<Func<TViewModel, T>> viewmodelProperty, ValidationRule validationRule)
        {
            var viewModelPropertyName = PropertyName.For(viewmodelProperty);
            if (!validationRules.ContainsKey(viewModelPropertyName))
            {
                validationRules[viewModelPropertyName] = new List<ValidationRule>();
            }

            validationRules[viewModelPropertyName].Add(validationRule);
        }

        private bool PropertyIsMappedWrapper(string viewModelPropertyName)
        {
            return viewModelWrapper.ContainsKey(viewModelPropertyName);
        }
    }
}