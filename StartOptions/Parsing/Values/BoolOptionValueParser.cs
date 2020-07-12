using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public class BoolOptionValueParser : AbstractStartOptionValueParser
    {
        public BoolOptionValueParser() : base(typeof(Boolean))
        { }

        protected override object ParseSingleValue(string value)
        {
            if(Boolean.TryParse(value, out bool boolValue))
            {
                return boolValue;
            }
            else if(Int64.TryParse(value, out long longValue))
            {
                return longValue != 0;
            }
            throw new FormatException($"\"{value}\" is neither a boolean nor a number.");
        }
    }
}
