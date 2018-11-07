using DependencyInjection.Container.Contract;
using System;

namespace DependencyInjection.Container
{
    public class Key : IKey
    {
        public Key(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public Type Type { get; private set; }

        public string Name { get; private set; }

        public bool Equals(IKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.Type, this.Type) && Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!typeof(IKey).IsAssignableFrom(obj.GetType()))
            {
                return false;
            }

            return Equals((IKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}