using System.ComponentModel;

namespace ModelViewViewModel.Base
{
    public interface IViewModelBase<TModel> : INotifyPropertyChanged where TModel : class
    {
        bool IsFor(TModel matchingModel);
    }
}