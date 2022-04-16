using System.Collections.Generic;
using System;

namespace LunarDoggo.StartOptions.DependencyInjection
{
    public class SimpleDependencyProvider : IDependencyProvider
    {
        private readonly Dictionary<Type, object> dependencies = new Dictionary<Type, object>();
        private readonly bool throwIfNotFound;

        public SimpleDependencyProvider(bool throwExceptionIfDependencyNotFound)
        {
            this.throwIfNotFound = throwExceptionIfDependencyNotFound;
        }

        public void AddSingleton<T>(T value)
        {
            if(value == null)
            {
                throw new ArgumentNullException("Can't register \"null\" as a dependency");
            }
            if(this.dependencies.ContainsKey(typeof(T)))
            {
                throw new ArgumentException($"Can't register dependency of type \"{typeof(T).FullName}\" twice");
            }
            this.dependencies.Add(typeof(T), value);
        }

        public T GetDependency<T>()
        {
            return (T)this.GetDependency(typeof(T));
        }

        public object GetDependency(Type type)
        {
            if (this.dependencies.ContainsKey(type))
            {
                return this.dependencies[type];
            }
            else
            {
                throw new KeyNotFoundException($"Dependency of type \"{type.FullName}\" couldn't be resolved");
            }
        }
    }
}
