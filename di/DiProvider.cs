using DependencyInjection.Container;
using DependencyInjection.Container.Contract;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection
{
    public class DiProvider : IDiProvider
    {
        internal IDiContainer diContainer;

        public DiProvider()
            : this(new DiContainer())
        {
        }

        internal DiProvider(IDiContainer diContainer)
        {
            this.diContainer = diContainer;
            RegisterInstance<IDiProvider>(this);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return diContainer.ResolveAll<T>().ToList();
        }

        public T Resolve<T>()
        {
            return diContainer.Resolve<T>();
        }

        public void RegisterType<TType, TClass>() where TClass : TType
        {
            diContainer.RegisterType<TType, TClass>();
        }

        public void RegisterType<TType, TClass>(string name) where TClass : TType
        {
            diContainer.RegisterType<TType, TClass>(name);
        }

        public void RegisterTypeAsSingleton<TType, TClass>() where TClass : TType
        {
            diContainer.RegisterTypeAsSingleton<TType, TClass>();
        }

        public void RegisterTypeAsSingleton<TType, TClass>(string name) where TClass : TType
        {
            diContainer.RegisterTypeAsSingleton<TType, TClass>(name);
        }

        public void RegisterInstance<T>(T impl)
        {
            diContainer.RegisterInstance(impl);
        }

        public void RegisterInstance<T>(T impl, string name)
        {
            diContainer.RegisterInstance(impl, name);
        }

        public void RegisterTypeIfNotSet<TType, TClass>() where TClass : TType
        {
            diContainer.RegisterTypeIfNotSet<TType, TClass>();
        }

        public void RegisterTypeIfNotSet<TType, TClass>(string name) where TClass : TType
        {
            diContainer.RegisterTypeIfNotSet<TType, TClass>(name);
        }

        public void RegisterTypeAsSingletonIfNotSet<TType, TClass>() where TClass : TType
        {
            diContainer.RegisterTypeAsSingletonIfNotSet<TType, TClass>();
        }

        public void RegisterTypeAsSingletonIfNotSet<TType, TClass>(string name) where TClass : TType
        {
            diContainer.RegisterTypeAsSingletonIfNotSet<TType, TClass>(name);
        }

        public void RegisterInstanceIfNotSet<T>(T impl)
        {
            diContainer.RegisterInstanceIfNotSet(impl);
        }

        public void RegisterInstanceIfNotSet<T>(T impl, string name)
        {
            diContainer.RegisterInstanceIfNotSet(impl, name);
        }

        public IDiProvider CreateChildProvider()
        {
            var childContainer = diContainer.CreateChildContainer();
            return new DiProvider(childContainer);
        }

        public bool IsRegistered<T>()
        {
            return diContainer.IsRegistered<T>();
        }

        public bool IsRegistered<T>(string name)
        {
            return diContainer.IsRegistered<T>(name);
        }

        public void Dispose()
        {
            diContainer.Dispose();
            diContainer = null;
        }
    }
}