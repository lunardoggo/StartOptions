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

        /// <summary>
        /// Adds the new dependency of <see cref="Type"/> <see cref="{T}"/> to the cached dependencies. If the cache already contains a
        /// dependency of <see cref="Type"/> <see cref="{T}"/>, an <see cref="ArgumentException"/> is thrown
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddSingleton<T>(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("Can't register \"null\" as a dependency");
            }
            if (this.dependencies.ContainsKey(typeof(T)))
            {
                throw new ArgumentException($"Can't register dependency of type \"{typeof(T).FullName}\" twice");
            }
            this.dependencies.Add(typeof(T), value);
        }

        /// <summary>
        /// Returns the dependency of <see cref="Type"/> <see cref="{T}"/> from all cached dependencies
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        public T GetDependency<T>()
        {
            object value = this.GetDependency(typeof(T));
            if (value != null)
            {
                return (T)value;
            }
            return default(T);
        }

        /// <summary>
        /// Returns the dependency of the provided <see cref="Type"/> from all cached dependencies
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        public object GetDependency(Type type)
        {
            if (this.dependencies.ContainsKey(type))
            {
                return this.dependencies[type];
            }
            else if (this.throwIfNotFound)
            {
                throw new KeyNotFoundException($"Dependency of type \"{type.FullName}\" couldn't be resolved");
            }
            return null;
        }
    }
}
