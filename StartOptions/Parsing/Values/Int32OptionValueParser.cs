﻿using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public class Int32OptionValueParser : AbstractStartOptionValueParser
    {
        public Int32OptionValueParser() : base(typeof(Int32))
        { }

        protected override object ParseSingleValue(string value)
        {
            return Int32.Parse(value);
        }
    }
}
