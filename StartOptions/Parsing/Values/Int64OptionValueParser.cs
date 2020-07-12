using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public class Int64OptionValueParser : AbstractStartOptionValueParser
    {
        public Int64OptionValueParser() : base(typeof(Int64))
        { }

        protected override object ParseSingleValue(string value)
        {
            return Int64.Parse(value);
        }
    }
}
