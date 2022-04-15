namespace LunarDoggo.StartOptions.DependencyInjection
{
    public interface IDependencyProvider
    {
        T GetDependency<T>();
    }
}
