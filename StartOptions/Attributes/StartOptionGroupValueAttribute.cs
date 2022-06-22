using System;

namespace LunarDoggo.StartOptions
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class StartOptionGroupValueAttribute : Attribute
    { 
        /// <summary>
        /// This attribute has to be used to decorate a constructor parameter of one
        /// of your classes implementing <see cref="IApplicationCommand"/> and defines
        /// a reference between the <see cref="StartOptionGroupAttribute"/> placed above
        /// the constructor in question and a constructor parameter
        /// </summary>
        public StartOptionGroupValueAttribute()
        { }
    }
}
