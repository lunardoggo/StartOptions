using System;

namespace LunarDoggo.StartOptions
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class StartOptionGroupValueAttribute : Attribute
    { }
}
