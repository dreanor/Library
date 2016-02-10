using System;
using System.ComponentModel;

namespace ModelViewViewModel.Base
{
    public class ViewModelPropertyWrapper<T> : NotifyPropertyChangedBase<IViewModelPropertyWrapper<T>>, IViewModelPropertyWrapper<T>, IDataErrorInfo, IViewModelPropertyWrapper
    {
        private readonly Func<T> getter;
        private readonly Action<T> setter;
        private T backingValue;
        private Func<string> validation;
        private object rawValue;

        internal ViewModelPropertyWrapper(T initValue)
        {
            backingValue = initValue;
            getter = () => backingValue;
            setter = value => backingValue = value;
            Reset();
        }

        internal ViewModelPropertyWrapper(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
            Reset();
        }

        public T Value
        {
            get
            {
                return getter();
            }

            private set
            {
                setter(value);
                Reset();
            }
        }

        public object RawValue
        {
            get
            {
                return rawValue ?? Value;
            }
            set
            {
                rawValue = value;
                Convert();
            }
        }

        public bool IsValid
        {
            get { return Get(x => x.IsValid); }
            internal set { Set(x => x.IsValid, value); }
        }

        public string Error
        {
            get
            {
                return validation();
            }
        }

        public string this[string columnName]
        {
            get
            {
                return Error;
            }
        }

        public void SetValidation(Func<string> validation)
        {
            this.validation = validation;
        }

        public void Reset()
        {
            rawValue = null;
            IsValid = true;
        }

        private void Convert()
        {
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (typeConverter == null)
            {
                IsValid = false;
                return;
            }

            try
            {
                Value = (T)typeConverter.ConvertFrom(RawValue);
            }
            catch (Exception)
            {
                IsValid = false;
            }
        }
    }
}