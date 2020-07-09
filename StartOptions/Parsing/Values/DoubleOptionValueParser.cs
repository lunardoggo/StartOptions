using System.Globalization;
using System.Linq;
using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public class DoubleOptionValueParser : IStartOptionValueParser
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyle;

        public DoubleOptionValueParser(IFormatProvider formatProvider, NumberStyles numberStyle)
        {
            this.formatProvider = formatProvider;
            this.numberStyle = numberStyle;
        }

        public DoubleOptionValueParser() : this(CultureInfo.CurrentCulture, NumberStyles.Any)
        { }

        public object ParseSingle(string value)
        {
            if(!Double.TryParse(value, this.numberStyle, this.formatProvider, out double doubleValue))
            {
                throw new ArgumentException($"Value \"{value}\" couldn't be parsed to Double");
            }
            return doubleValue;
        }

        public object[] ParseMultiple(string[] values)
        {
            return values.Select(_value => this.ParseSingle(_value)).ToArray();
        }
    }
}
