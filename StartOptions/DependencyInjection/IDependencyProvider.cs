using System;

namespace LunarDoggo.StartOptions.DependencyInjection
{
    public interface IDependencyProvider
    {
        /// <summary>
        /// Returns the dependency of the provided <see cref="Type"/>
        /// </summary>
        object GetDependency(Type type);
        /// <summary>
        /// Returns the dependency of <see cref="Type"/> <see cref="{T}"/>
        /// </summary>
        T GetDependency<T>();
    }
}
