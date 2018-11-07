using System.Collections.Generic;

namespace DependencyInjection
{
    public interface IDiProvider
    {
        IEnumerable<T> ResolveAll<T>();

        T Resolve<T>();

        void RegisterType<TType, TClass>() where TClass : TType;

        void RegisterType<TType, TClass>(string name) where TClass : TType;

        void RegisterTypeAsSingleton<TType, TClass>() where TClass : TType;

        void RegisterTypeAsSingleton<TType, TClass>(string name) where TClass : TType;

        void RegisterInstance<T>(T impl);

        void RegisterInstance<T>(T impl, string name);

        void RegisterTypeIfNotSet<TType, TClass>() where TClass : TType;

        void RegisterTypeIfNotSet<TType, TClass>(string name) where TClass : TType;

        void RegisterTypeAsSingletonIfNotSet<TType, TClass>() where TClass : TType;

        void RegisterTypeAsSingletonIfNotSet<TType, TClass>(string name) where TClass : TType;

        void RegisterInstanceIfNotSet<T>(T impl);

        void RegisterInstanceIfNotSet<T>(T impl, string name);

        IDiProvider CreateChildProvider();

        bool IsRegistered<T>();

        bool IsRegistered<T>(string name);

        void Dispose();
    }
}