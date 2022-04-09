using LunarDoggo.StartOptions.Parsing.Values;
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
        public string Description { get; set; } = String.Empty;
        public bool IsGrouplessOption { get; set; } = false;
        public Type ParserType { get; set; } = null;
        public bool Mandatory { get; set; } = false;

        public string ShortName { get; }
        public string LongName { get; }
    }
}
