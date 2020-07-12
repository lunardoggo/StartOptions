using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public class Int16OptionValueParser : AbstractStartOptionValueParser
    {
        public Int16OptionValueParser() : base(typeof(Int16))
        { }

        protected override object ParseSingleValue(string value)
        {
            return Int16.Parse(value);
        }
    }
}
