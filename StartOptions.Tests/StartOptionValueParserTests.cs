using LunarDoggo.StartOptions.Parsing.Values;
using System.Globalization;
using System;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionValueParserTests
    {
        [Fact]
        public void TestParseByte()
        {
            IStartOptionValueParser parser = new ByteOptionValueParser();

            Assert.Equal((byte)255, parser.ParseValue("255"));
            Assert.Equal((byte)127, parser.ParseValue("127"));
            Assert.Equal((byte)0, parser.ParseValue("0"));

            Assert.Throws<ArgumentException>(() => parser.ParseValue(Int16.MaxValue.ToString()));
            Assert.Throws<ArgumentException>(() => parser.ParseValue("10.01"));

            this.AssertInvalidValueThrowsArgumentException(parser);
        }

        [Fact]
        public void TestParseInt16()
        {
            IStartOptionValueParser parser = new Int16OptionValueParser();

            Assert.Equal((short)187, parser.ParseValue("187"));
            Assert.Equal((short)-10, parser.ParseValue("-10"));
            Assert.Equal((short)0, parser.ParseValue("0"));

            Assert.Throws<ArgumentException>(() => parser.ParseValue(Int64.MaxValue.ToString()));
            Assert.Throws<ArgumentException>(() => parser.ParseValue("10.01"));

            this.AssertInvalidValueThrowsArgumentException(parser);
        }

        [Fact]
        public void TestParseInt32()
        {
            IStartOptionValueParser parser = new Int32OptionValueParser();

            Assert.Equal(-10, parser.ParseValue("-10"));
            Assert.Equal(10, parser.ParseValue("10"));

            Assert.Throws<ArgumentException>(() => parser.ParseValue(Int64.MaxValue.ToString()));
            Assert.Throws<ArgumentException>(() => parser.ParseValue("10.01"));

            this.AssertInvalidValueThrowsArgumentException(parser);
        }

        [Fact]
        public void TestParseInt64()
        {
            IStartOptionValueParser parser = new Int64OptionValueParser();

            Assert.Equal(Int64.MaxValue, parser.ParseValue(Int64.MaxValue.ToString()));
            Assert.Equal((long)-10, parser.ParseValue("-10"));
            Assert.Equal((long)178, parser.ParseValue("178"));

            Assert.Throws<ArgumentException>(() => parser.ParseValue("10.01"));

            this.AssertInvalidValueThrowsArgumentException(parser);
        }

        [Fact]
        public void TestSingleWithEnglishFormat()
        {
            CultureInfo culture = this.GetCulture("en-GB");
            IStartOptionValueParser parser = new FloatOptionValueParser(culture, NumberStyles.Any);

            this.AssertParseFloat(parser, culture);
            this.AssertInvalidValueThrowsArgumentException(parser);
        }

        [Fact]
        public void TestSingleWithGermanFormat()
        {
            CultureInfo culture = this.GetCulture("de-DE");
            IStartOptionValueParser parser = new FloatOptionValueParser(culture, NumberStyles.Any);

            this.AssertParseFloat(parser, culture);
            this.AssertInvalidValueThrowsArgumentException(parser);
        }

        private void AssertParseFloat(IStartOptionValueParser parser, CultureInfo culture)
        {
            Assert.Equal((float)Int64.MaxValue, parser.ParseValue(Int64.MaxValue.ToString()));
            Assert.Equal(10.01f, parser.ParseValue($"10{culture.NumberFormat.NumberDecimalSeparator}01"));
            Assert.Equal(10.0f, parser.ParseValue("10"));
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
            Assert.Equal((double)Int64.MaxValue, parser.ParseValue(Int64.MaxValue.ToString()));
            Assert.Equal(10.01d, parser.ParseValue($"10{culture.NumberFormat.NumberDecimalSeparator}01"));
            Assert.Equal(10.0d, parser.ParseValue("10"));
        }

        private CultureInfo GetCulture(string name)
        {
            return CultureInfo.GetCultureInfo(name);
        }

        [Fact]
        public void TestParseBoolean()
        {
            IStartOptionValueParser parser = new BoolOptionValueParser();

            Assert.True((bool)parser.ParseValue("true"));
            Assert.True((bool)parser.ParseValue("-126"));
            Assert.True((bool)parser.ParseValue("126"));
            Assert.True((bool)parser.ParseValue("1"));

            Assert.False((bool)parser.ParseValue("false"));
            Assert.False((bool)parser.ParseValue("0"));

            this.AssertInvalidValueThrowsArgumentException(parser);
        }

        private void AssertInvalidValueThrowsArgumentException(IStartOptionValueParser parser)
        {
            Assert.Throws<ArgumentException>(() => parser.ParseValue("test"));
            Assert.Throws<ArgumentException>(() => parser.ParseValue(null));
            Assert.Throws<ArgumentException>(() => parser.ParseValue(""));
        }
    }
}
