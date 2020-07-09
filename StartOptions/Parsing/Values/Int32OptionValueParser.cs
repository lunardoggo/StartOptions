using System.Linq;
using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public class Int32OptionValueParser : IStartOptionValueParser
    {
        public object ParseSingle(string value)
        {
            if (!Int32.TryParse(value, out int intValue))
            {
                throw new ArgumentException($"Value \"{value}\" couldn't be parsed to Int32");
            }
            return intValue;
        }

        public object[] ParseMultiple(string[] values)
        {
            return values.Select(_value => this.ParseSingle(_value)).ToArray();
        }
    }
}
