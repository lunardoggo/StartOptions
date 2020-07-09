using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions;
using System.Globalization;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionValueParsingTests
    {
        [Fact]
        public void TestInt32OptionValueParser()
        {
            StartOption option = this.GetStartOptionWithValueParser(new Int32OptionValueParser());

            option.ParseSingleValue("10");

            this.AssertOptionWasParsedAndHasValue(option, 10);
        }

        [Fact]
        public void TestDoubleOptionValueParser()
        {
            CultureInfo culture = CultureInfo.GetCultureInfo("en-GB");
            IStartOptionValueParser parser = new DoubleOptionValueParser(culture, NumberStyles.Any);
            StartOption option = this.GetStartOptionWithValueParser(parser);

            option.ParseSingleValue("10.5");

            this.AssertOptionWasParsedAndHasValue(option, 10.5d);
        }

        [Fact]
        public void TestBoolOptionValueParser()
        {
            StartOption option = this.GetStartOptionWithValueParser(new BoolOptionValueParser());

            option.ParseSingleValue("true");

            this.AssertOptionWasParsedAndHasValue(option, true);
        }

        [Fact]
        public void TestInt32ArrayOptionValueParser()
        {
            StartOption option = this.GetStartOptionWithValueParser(new Int32OptionValueParser());

            option.ParseMultipleValues(new string[] { "1", "4", "6" });

            this.AssertOptionWasParsedAndHasValue(option, new object[] { 1, 4, 6 });
        }

        private void AssertOptionWasParsedAndHasValue<T>(StartOption option, T assertedValue)
        {
            Assert.True(option.HasValue);
            Assert.Equal(assertedValue, option.GetValue<T>());
        }

        private StartOption GetStartOptionWithValueParser(IStartOptionValueParser parser)
        {
            StartOptionBuilder builder = new StartOptionBuilder("long", "s")
                .SetValueParser(parser);
            return builder.Build();
        }
    }
}
