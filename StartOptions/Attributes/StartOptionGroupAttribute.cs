using System;

namespace LunarDoggo.StartOptions
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class StartOptionGroupAttribute : Attribute
    {
        public StartOptionGroupAttribute(string longName, string shortName)
        {
            this.ShortName = shortName;
            this.LongName = longName;
        }

        public string Description { get; set; }

        public string ShortName { get; }
        public string LongName { get; }
    }
}
