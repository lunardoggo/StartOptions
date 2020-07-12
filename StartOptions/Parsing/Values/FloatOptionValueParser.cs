using System.Globalization;
using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public class FloatOptionValueParser : AbstractStartOptionValueParser
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyle;

        public FloatOptionValueParser(IFormatProvider formatProvider, NumberStyles numberStyle) : base(typeof(Double))
        {
            this.formatProvider = formatProvider;
            this.numberStyle = numberStyle;
        }

        public FloatOptionValueParser() : this(CultureInfo.CurrentCulture, NumberStyles.Any)
        { }

        protected override object ParseSingleValue(string value)
        {
            return Single.Parse(value, this.numberStyle, this.formatProvider);
        }
    }
}
