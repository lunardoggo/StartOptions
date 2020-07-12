using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public class ByteOptionValueParser : AbstractStartOptionValueParser
    {
        public ByteOptionValueParser() : base(typeof(Byte))
        { }

        protected override object ParseSingleValue(string value)
        {
            return Byte.Parse(value);
        }
    }
}
