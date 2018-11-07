using System.ComponentModel;

namespace ModelViewViewModel.Base
{
    public interface IViewModelBase<TModel> : INotifyPropertyChanged where TModel : class
    {
        /// <summary>
        /// Checks wether the matchingModel is equal to the current model.
        /// </summary>
        /// <param name="matchingModel">Other Model.</param>
        /// <returns>Wether the matchingModel is equal to the current model or not</returns>
        bool IsFor(TModel matchingModel);
    }
}