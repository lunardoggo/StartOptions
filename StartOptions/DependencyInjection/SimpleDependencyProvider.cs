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
            if(this.dependencies.ContainsKey(typeof(T)))
            {
                return (T)this.dependencies[typeof(T)];
            }
            else
            {
                throw new KeyNotFoundException($"Dependency of type \"{typeof(T).FullName}\" couldn't be resolved");
            }
        }
    }
}
