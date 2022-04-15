using System;

namespace LunarDoggo.StartOptions.DependencyInjection
{
    public class SimpleDependencyProvider : IDependencyProvider
    {
        private readonly bool throwIfNotFound;

        public SimpleDependencyProvider(bool throwExceptionIfDependencyNotFound)
        {
            this.throwIfNotFound = throwExceptionIfDependencyNotFound;
        }

        public void AddSingleton(object value)
        {
            throw new NotImplementedException();
        }

        public void AddSingleton<T, U>(U value) where U : T
        {
            throw new NotImplementedException();
        }

        public T GetDependency<T>()
        {
            throw new NotImplementedException();
        }
    }
}
