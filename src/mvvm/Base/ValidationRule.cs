using System;

namespace ModelViewViewModel.Base
{
    public class ValidationRule
    {
        public ValidationRule(Func<bool> validation, string errorMessage)
        {
            Validation = validation;
            ErrorMessage = errorMessage;
        }

        public Func<bool> Validation { get; }

        public string ErrorMessage { get; }
    }
}
