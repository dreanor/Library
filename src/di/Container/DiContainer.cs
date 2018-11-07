using DependencyInjection.Container.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.Container
{
    public class DiContainer : IDiContainer
    {
        private Dictionary<IKey, Func<IDiContainer, dynamic>> diDictionary;
        private IResolver resolver;

        public DiContainer()
            : this(new Dictionary<IKey, Func<IDiContainer, dynamic>>(), new Resolver())
        {
        }

        private DiContainer(Dictionary<IKey, Func<IDiContainer, dynamic>> dictionary, IResolver resolver)
        {
            this.resolver = resolver;
            diDictionary = new Dictionary<IKey, Func<IDiContainer, dynamic>>(dictionary);
        }

        public void Dispose()
        {
            diDictionary.Clear();
            diDictionary = null;
            resolver = null;
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return ResolveAll(typeof(T)).Cast<T>();
        }

        public IEnumerable<dynamic> ResolveAll(Type type)
        {
            var keyValuePairs = diDictionary.Where(x => x.Key.Type == type).ToList();
            foreach (var keyValuePair in keyValuePairs)
            {
                yield return keyValuePair.Value(this);
            }
        }

        public T Resolve<T>()
        {
            return this.Resolve(typeof(T));
        }

        public dynamic Resolve(Type type)
        {
            if (type.IsGenericEnumerable() && AreElementsOfListRegistered(type))
            {
                var objList = ResolveAll(type.GetGenericArguments()[0]).ToList();
                var cast = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(type.GetGenericArguments()[0]);
                return cast.Invoke(null, new[] { objList });
            }

            return diDictionary[new Key(type, string.Empty)](this);
        }

        public void RegisterType<TType, TClass>() where TClass : TType
        {
            RegisterType<TType, TClass>(string.Empty);
        }

        public void RegisterType<TType, TClass>(string name) where TClass : TType
        {
            diDictionary[new Key(typeof(TType), name)] = x => this.resolver.Go<TClass>(x);
        }

        public void RegisterTypeAsSingleton<TType, TClass>() where TClass : TType
        {
            RegisterTypeAsSingleton<TType, TClass>(string.Empty);
        }

        public void RegisterTypeAsSingleton<TType, TClass>(string name) where TClass : TType
        {
            diDictionary[new Key(typeof(TType), name)] = x => { var obj = this.resolver.Go<TClass>(x); x.RegisterInstance<TType>(obj); return obj; };
        }

        public void RegisterInstance<T>(T impl)
        {
            RegisterInstance(impl, string.Empty);
        }

        public void RegisterInstance<T>(T impl, string name)
        {
            diDictionary[new Key(typeof(T), name)] = x => impl;
        }

        public void RegisterTypeIfNotSet<TType, TClass>() where TClass : TType
        {
            if (!IsRegistered<TClass>())
            {
                RegisterType<TType, TClass>();
            }
        }

        public void RegisterTypeIfNotSet<TType, TClass>(string name) where TClass : TType
        {
            if (!IsRegistered<TClass>())
            {
                RegisterType<TType, TClass>(name);
            }
        }

        public void RegisterTypeAsSingletonIfNotSet<TType, TClass>() where TClass : TType
        {
            if (!IsRegistered<TClass>())
            {
                RegisterTypeAsSingleton<TType, TClass>();
            }
        }

        public void RegisterTypeAsSingletonIfNotSet<TType, TClass>(string name) where TClass : TType
        {
            if (!IsRegistered<TClass>(name))
            {
                RegisterTypeAsSingleton<TType, TClass>(name);
            }
        }

        public void RegisterInstanceIfNotSet<T>(T impl)
        {
            if (!IsRegistered<T>())
            {
                RegisterInstance(impl);
            }
        }

        public void RegisterInstanceIfNotSet<T>(T impl, string name)
        {
            if (!IsRegistered<T>(name))
            {
                RegisterInstance(impl, name);
            }
        }

        public IDiContainer CreateChildContainer()
        {
            return new DiContainer(diDictionary, resolver);
        }

        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof(T));
        }

        public bool IsRegistered(Type type)
        {
            return IsRegistered(type, string.Empty);
        }

        public bool IsRegistered<T>(string name)
        {
            return IsRegistered(typeof(T), name);
        }

        public bool IsRegistered(Type type, string name)
        {
            return AreElementsOfListRegistered(type) || diDictionary.ContainsKey(new Key(type, name));
        }

        private bool AreElementsOfListRegistered(Type type)
        {
            return type.IsGenericEnumerable() && this.diDictionary.Any(x => x.Key.Type == type.GetGenericArguments()[0]);
        }
    }

    internal static class Ex
    {
        internal static bool IsGenericEnumerable(this Type @this)
        {
            return (@this.IsGenericType && @this.HasGenericEnumerableGetGenericTypeDefinition()) 
                || @this.GetInterfaces().Any(x => x.IsGenericType && x.HasGenericEnumerableGetGenericTypeDefinition());
        }

        internal static bool HasGenericEnumerableGetGenericTypeDefinition(this Type @this)
        {
            return @this.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }
    }
}