﻿using System.Globalization;
using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public class DoubleOptionValueParser : AbstractStartOptionValueParser
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyle;

        public DoubleOptionValueParser(IFormatProvider formatProvider, NumberStyles numberStyle) : base(typeof(Double))
        {
            this.formatProvider = formatProvider;
            this.numberStyle = numberStyle;
        }

        public DoubleOptionValueParser() : this(CultureInfo.CurrentCulture, NumberStyles.Any)
        { }

        protected override object ParseSingleValue(string value)
        {
            return Double.Parse(value, this.numberStyle, this.formatProvider);
        }
    }
}
