using System;

namespace LunarDoggo.StartOptions
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class StartOptionAttribute : Attribute
    {
        public StartOptionAttribute(string longName, string shortName)
        {
            this.ShortName = shortName;
            this.LongName = longName;
        }

        public StartOptionValueType ValueType { get; set; } = StartOptionValueType.Switch;
        public bool IsGrouplessOption { get; set; } = false;
        public string Description { get; set; }
        public bool Mandatory { get; set; }

        public string ShortName { get; }
        public string LongName { get; }
    }
}
