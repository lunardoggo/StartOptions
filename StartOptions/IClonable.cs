namespace LunarDoggo.StartOptions
{
    public interface IClonable<T>
    {
        /// <summary>
        /// Returns a cloned version of this <see cref="object"/> instance of <see cref="System.Type"/> <see cref="{T}"/>
        /// </summary>
        T Clone();
    }
}
