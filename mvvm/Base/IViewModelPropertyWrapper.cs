using System;
using System.ComponentModel;

namespace ModelViewViewModel.Base
{
    public interface IViewModelPropertyWrapper : INotifyPropertyChanged
    {
        bool IsValid { get; }

        void Reset();

        void SetValidation(Func<string> validation);
    }

    public interface IViewModelPropertyWrapper<T> : INotifyPropertyChanged
    {
        bool IsValid { get; }

        object RawValue { get; set; }

        T Value { get; }
    }
}