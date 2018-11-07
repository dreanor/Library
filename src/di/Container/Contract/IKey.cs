using System;

namespace DependencyInjection.Container.Contract
{
    public interface IKey : IEquatable<IKey>
    {
        Type Type { get; }

        string Name { get; }
    }
}