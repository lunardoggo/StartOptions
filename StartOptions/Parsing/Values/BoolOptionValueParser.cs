using System.Linq;
using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public class BoolOptionValueParser : IStartOptionValueParser
    {
        public object ParseSingle(string value)
        {
            if (Boolean.TryParse(value, out bool boolValue))
            {
                return boolValue;
            }
            else if(Int64.TryParse(value, out long longValue))
            {
                return longValue != 0;
            }
            throw new ArgumentException($"Value \"{value}\" couldn't be parsed to Boolean");
        }

        public object[] ParseMultiple(string[] values)
        {
            return values.Select(_value => this.ParseSingle(_value)).ToArray();
        }
    }
}
