using LunarDoggo.StartOptions.Parsing.Values;
using System.Globalization;
using System;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionValueParserTests
    {
        [Fact]
        public void TestParseInt32()
        {
            IStartOptionValueParser parser = new Int32OptionValueParser();

            Assert.Equal(10, parser.ParseSingle("10"));

            Assert.Throws<ArgumentException>(() => parser.ParseSingle(Int64.MaxValue.ToString()));
            Assert.Throws<ArgumentException>(() => parser.ParseSingle("10.01"));

            this.AssertInvalidValueThrowsArgumentException(parser);
        }

        [Fact]
        public void TestParseDoubleWithEnglishFormat()
        {
            CultureInfo culture = this.GetCulture("en-GB");
            IStartOptionValueParser parser = new DoubleOptionValueParser(culture, NumberStyles.Any);

            this.AssertParseDouble(parser, culture);
            this.AssertInvalidValueThrowsArgumentException(parser);
        }

        [Fact]
        public void TestParseDoubleWithGermanFormat()
        {
            CultureInfo culture = this.GetCulture("de-DE");
            IStartOptionValueParser parser = new DoubleOptionValueParser(culture, NumberStyles.Any);

            this.AssertParseDouble(parser, culture);
            this.AssertInvalidValueThrowsArgumentException(parser);
        }

        private void AssertParseDouble(IStartOptionValueParser parser, CultureInfo culture)
        {
            Assert.Equal((double)Int64.MaxValue, parser.ParseSingle(Int64.MaxValue.ToString()));
            Assert.Equal(10.01d, parser.ParseSingle($"10{culture.NumberFormat.NumberDecimalSeparator}01"));
            Assert.Equal(10.0d, parser.ParseSingle("10"));
        }

        private CultureInfo GetCulture(string name)
        {
            return CultureInfo.GetCultureInfo(name);
        }

        [Fact]
        public void TestParseBoolean()
        {
            IStartOptionValueParser parser = new BoolOptionValueParser();

            Assert.True((bool)parser.ParseSingle("true"));
            Assert.True((bool)parser.ParseSingle("126"));
            Assert.True((bool)parser.ParseSingle("1"));

            Assert.False((bool)parser.ParseSingle("false"));
            Assert.False((bool)parser.ParseSingle("0"));

            this.AssertInvalidValueThrowsArgumentException(parser);
        }

        private void AssertInvalidValueThrowsArgumentException(IStartOptionValueParser parser)
        {
            Assert.Throws<ArgumentException>(() => parser.ParseSingle("test"));
            Assert.Throws<ArgumentException>(() => parser.ParseSingle(null));
            Assert.Throws<ArgumentException>(() => parser.ParseSingle(""));
        }
    }
}
