using LunarDoggo.StartOptions.Parsing;
using LunarDoggo.StartOptions;
using System.Linq;
using Xunit;

namespace StartOptions.Tests
{
    public static class AssertionUtility
    {
        public static void WasHelpRequested(StartOptionParser parser, params string[] args)
        {
            Assert.True(parser.Parse(args).WasHelpRequested);
        }

        public static void StartOptionHasValue<T>(StartOption option, T value)
        {
            Assert.True(option.HasValue);
            Assert.Equal(value, option.GetValue<T>());
        }

        public static void HasAllGrouplessOptions(ParsedStartOptions parsed)
        {
            Assert.Equal(2, parsed.ParsedGrouplessOptions.Count());
            Assert.NotNull(parsed.ParsedGrouplessOptions.SingleOrDefault(_option => _option.ShortName.Equals("d")));
            Assert.NotNull(parsed.ParsedGrouplessOptions.SingleOrDefault(_option => _option.ShortName.Equals("v")));
        }

        public static void StartOptionGroup(string longName, string shortName, string description, StartOptionGroup group)
        {
            Assert.Equal(longName, group.LongName);
            Assert.Equal(shortName, group.ShortName);
            Assert.Equal(description, group.Description);
        }

        public static void StartOption(string longName, string shortName, string description, bool mandatory, StartOptionValueType valueType, StartOption option)
        {
            Assert.Equal(longName, option.LongName);
            Assert.Equal(shortName, option.ShortName);
            Assert.Equal(description, option.Description);
            Assert.Equal(mandatory, option.IsMandatory);
            Assert.Equal(valueType, option.ValueType);
        }
    }
}