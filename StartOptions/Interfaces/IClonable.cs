namespace LunarDoggo.StartOptions.Interfaces
{
    public interface IClonable<T>
    {
        /// <summary>
        /// Returns a cloned version of this <see cref="object"/> instance of <see cref="System.Type"/> <see cref="{T}"/>
        /// </summary>
        T Clone();
    }
}
