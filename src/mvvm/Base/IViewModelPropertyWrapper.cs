using System;
using System.ComponentModel;

namespace ModelViewViewModel.Base
{
    public interface IViewModelPropertyWrapper : INotifyPropertyChanged
    {
        /// <summary>
        /// Wether the ViewModel meets all Validations or not.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Resets IsValid to true and RawValue to null.
        /// </summary>
        void Reset();

        /// <summary>
        /// Sets a Validation on the ViewModel.
        /// </summary>
        /// <param name="validation">Validation function.</param>
        void SetValidation(Func<string> validation);
    }

    public interface IViewModelPropertyWrapper<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Wether the ViewModel meets all Validations or not.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// The raw value.
        /// </summary>
        object RawValue { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        T Value { get; }
    }
}