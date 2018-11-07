using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace ModelViewViewModel.list
{

    public class ObservableCollectionMvvm<T> : ObservableCollection<T>
    {
        public ObservableCollectionMvvm(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public ObservableCollectionMvvm(List<T> collection)
            : base(collection)
        {
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            using (BlockReentrancy())
            {
                NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;
                if (eventHandler != null)
                {
                    Delegate[] delegates = eventHandler.GetInvocationList();
                    foreach (NotifyCollectionChangedEventHandler handler in delegates)
                    {
                        var dispatcherObject = handler.Target as DispatcherObject;
                        if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                        {
                            dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
                        }
                        else
                        {
                            handler(this, e);
                        }
                    }
                }
            }
        }
    }
}