using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System.Linq;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionParserSpecialSettingsParsingTests : BaseStartOptionParserParsingTests
    {
        [Fact]
        public void TestWasHelpRequested()
        {
            StartOptionParser parser = this.GetSpecialStartOptionParser();

            AssertionUtility.WasHelpRequested(parser, "/h");
            AssertionUtility.WasHelpRequested(parser, "&help");
            AssertionUtility.WasHelpRequested(parser, "/?");
            AssertionUtility.WasHelpRequested(parser, "/h", "/i", "/p", "./user.txt");
        }

        [Fact]
        public void TestParsedOptionGroupWithoutGrouplessOptions()
        {
            StartOptionParser parser = this.GetSpecialStartOptionParser();
            string[] args = new string[] { "&import", "/p", "./user.txt", "/f" };
            //C# splits the start arguments when a space is encountered and it is not contained inside of quotation marks

            ParsedStartOptions parsed = parser.Parse(args);

            Assert.False(parsed.WasHelpRequested);
            Assert.Empty(parsed.ParsedGrouplessOptions);

            StartOptionGroup group = parsed.ParsedOptionGroup;
            Assert.NotNull(group);
            Assert.False(group.HasValue);
            Assert.Equal("i", group.ShortName);
            AssertionUtility.StartOptionHasValue(group.GetOptionByShortName("p"), "./user.txt");
            Assert.NotNull(group.GetOptionByShortName("f"));
        }

        [Fact]
        public void TestParsedOptionGroupWithGrouplessOptions()
        {
            StartOptionParser parser = this.GetSpecialStartOptionParser();
            string[] args = new string[] { "&export", "/u", "testuser", "/p", "./user.txt", "/d", "&verbose" };

            ParsedStartOptions parsed = parser.Parse(args);

            Assert.False(parsed.WasHelpRequested);
            AssertionUtility.HasAllGrouplessOptions(parsed);

            StartOptionGroup group = parsed.ParsedOptionGroup;
            Assert.NotNull(group);
            Assert.False(group.HasValue);
            Assert.Equal("e", group.ShortName);
            AssertionUtility.StartOptionHasValue(group.GetOptionByShortName("u"), "testuser");
            AssertionUtility.StartOptionHasValue(group.GetOptionByShortName("p"), "./user.txt");
        }

        [Fact]
        public void TestParsedGrouplessOptionsWithoutGroup()
        {
            StartOptionParser parser = this.GetSpecialStartOptionParser();
            string[] args = new string[] { "/d", "&verbose" };

            ParsedStartOptions parsed = parser.Parse(args);

            Assert.False(parsed.WasHelpRequested);
            Assert.Null(parsed.ParsedOptionGroup);
            AssertionUtility.HasAllGrouplessOptions(parsed);
        }

        [Fact]
        public void TestParsedMultiValueGrouplessOptionWithoutGroup()
        {
            StartOptionParser parser = this.GetSpecialStartOptionParser();
            string[] args = new string[] { "/n", "test1;test2;test3" };

            ParsedStartOptions parsed = parser.Parse(args);

            Assert.False(parsed.WasHelpRequested);
            Assert.Null(parsed.ParsedOptionGroup);

            StartOption option = parsed.ParsedGrouplessOptions.SingleOrDefault(_option => _option.ShortName.Equals("n"));
            object[] values = option.GetValue<object[]>();
            Assert.Equal(3, values.Length);
            Assert.Equal("test1", values[0]);
            Assert.Equal("test2", values[1]);
            Assert.Equal("test3", values[2]);
        }

        [Fact]
        public void TestParseGroupWithValues()
        {
            StartOptionParser parser = this.GetStartOptionParserWithGroupValues();
            string[] firstArgs = new string[] { "--say", "test" };

            ParsedStartOptions firstParsed = parser.Parse(firstArgs);

            Assert.False(firstParsed.WasHelpRequested);
            Assert.Empty(firstParsed.ParsedGrouplessOptions);

            StartOptionGroup firstGroup = firstParsed.ParsedOptionGroup;
            Assert.NotNull(firstGroup);
            Assert.Equal("say", firstGroup.LongName);
            Assert.True(firstGroup.HasValue);
            Assert.Equal("test", firstGroup.GetValue<string>());

            string[] secondArgs = new string[] { "--add", "1,2,3,4" };
            ParsedStartOptions secondParsed = parser.Parse(secondArgs);

            Assert.False(secondParsed.WasHelpRequested);
            Assert.Empty(secondParsed.ParsedGrouplessOptions);

            StartOptionGroup secondGroup = secondParsed.ParsedOptionGroup;
            Assert.NotNull(secondGroup);
            Assert.Equal("add", secondGroup.LongName);
            Assert.True(secondGroup.HasValue);
            int[] parsedValues = secondGroup.GetValue<object[]>().Cast<int>().ToArray();
            Assert.Equal(4, parsedValues.Length);
            for(int i = 0; i < parsedValues.Length; i++)
            {
                Assert.Equal(i + 1, parsedValues[i]);
            }
        }

        private StartOptionParser GetSpecialStartOptionParser()
        {
            HelpOption[] helpOptions = new HelpOption[] { new HelpOption("help", false), new HelpOption("h", true), new HelpOption("?", true) };
            return this.GetStartOptionParser(this.GetParserSettings(), helpOptions);
        }

        private StartOptionParser GetStartOptionParserWithGroupValues()
        {
            HelpOption[] helpOptions = new HelpOption[] { new HelpOption("help", false), new HelpOption("h", true), new HelpOption("?", true) };
            IEnumerable<StartOptionGroup> groups = new StartOptionGroup[]
            {
                new StartOptionGroupBuilder("say", "s").SetValueType(StartOptionValueType.Single).Build(),
                new StartOptionGroupBuilder("add", "a").SetValueType(StartOptionValueType.Multiple).SetValueParser(new Int32OptionValueParser()).Build()
            };

            return new StartOptionParser(new StartOptionParserSettings() { OptionValueSeparator = ' ' }, groups, new StartOption[0], helpOptions);
        }

        private StartOptionParserSettings GetParserSettings()
        {
            return new StartOptionParserSettings()
            {
                MultipleValueSeparator = ';',
                ShortOptionNamePrefix = "/",
                LongOptionNamePrefix = "&",
                OptionValueSeparator = ' '
            };
        }
    }
}
