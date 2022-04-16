using System;

namespace LunarDoggo.StartOptions.DependencyInjection
{
    public interface IDependencyProvider
    {
        object GetDependency(Type type);
        T GetDependency<T>();
    }
}
