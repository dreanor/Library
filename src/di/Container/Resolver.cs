using DependencyInjection.Container.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjection.Container
{
    internal class Resolver : IResolver
    {
        public T Go<T>(IDiContainer diContainer)
        {
            var typeT = typeof(T);
            var parameterJeConstructor = this.GetParameterJeConstructor(typeT);
            foreach (var parameter in parameterJeConstructor.OrderByDescending(cp => cp.Length))
            {
                if (parameter.Length == 0)
                {
                    return (T)Activator.CreateInstance(typeT);
                }

                if (this.AlleParameterBekannt(parameter, diContainer))
                {
                    return (T)Activator.CreateInstance(typeT, this.GetInstanzenFür(parameter, diContainer));
                }
            }

            throw new ArgumentException(string.Format("Objekt vom Typ \"{0}\" konnte nicht instanziiert werden", typeT) + this.GetDetails(parameterJeConstructor, diContainer));
        }

        internal virtual string GetDetails(IEnumerable<ParameterInfo[]> parameterJeConstructor, IDiContainer diContainer)
        {
            var trennlinie = Environment.NewLine +
                             "---------------------------------------------------------------------------" +
                             Environment.NewLine;
            if (parameterJeConstructor.Count() > 1)
            {
                return trennlinie + "Objekt hat mehr als ein Konstruktor. Fehlende (Parameter)-Registrierungen sind nicht bekannt.";
            }

            return
                parameterJeConstructor.First()
                .Where(p => !diContainer.IsRegistered(p.ParameterType))
                .Aggregate(trennlinie + "Fehlende (Parameter)-Registrierungen:", (s, p) => s + Environment.NewLine + "- " + p.ParameterType.ToString());
        }
        
        internal virtual IEnumerable<ParameterInfo[]> GetParameterJeConstructor(Type typeT)
        {
            return typeT.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Select(ctor => ctor.GetParameters());
        }

        internal virtual object[] GetInstanzenFür(IEnumerable<ParameterInfo> parameterInfos, IDiContainer diContainer)
        {
            List<object> parameter = new List<object>();
            foreach (var parameterInfo in parameterInfos)
            {
                parameter.Add(diContainer.Resolve(parameterInfo.ParameterType));
            }

            return parameter.ToArray();
        }

        internal virtual bool AlleParameterBekannt(IEnumerable<ParameterInfo> parameterInfos, IDiContainer diContainer)
        {
            return parameterInfos.AsParallel().All(t => diContainer.IsRegistered(t.ParameterType));
        }
    }
}