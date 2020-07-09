using LunarDoggo.StartOptions.Parsing;
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

            this.AssertWasHelpRequested(parser, "/h");
            this.AssertWasHelpRequested(parser, "&help");
            this.AssertWasHelpRequested(parser, "/?");
            this.AssertWasHelpRequested(parser, "/h", "/i", "/p", "./user.txt");
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
            Assert.Equal("i", group.ShortName);
            this.AssertStartOptionHasValue(group.GetOptionByShortName("p"), "./user.txt");
            Assert.NotNull(group.GetOptionByShortName("f"));
        }

        [Fact]
        public void TestParsedOptionGroupWithGrouplessOptions()
        {
            StartOptionParser parser = this.GetSpecialStartOptionParser();
            string[] args = new string[] { "&export", "/u", "testuser", "/p", "./user.txt", "/d", "&verbose" };

            ParsedStartOptions parsed = parser.Parse(args);

            Assert.False(parsed.WasHelpRequested);
            this.AssertHasAllGrouplessOptions(parsed);

            StartOptionGroup group = parsed.ParsedOptionGroup;
            Assert.NotNull(group);
            Assert.Equal("e", group.ShortName);
            this.AssertStartOptionHasValue(group.GetOptionByShortName("u"), "testuser");
            this.AssertStartOptionHasValue(group.GetOptionByShortName("p"), "./user.txt");
        }

        [Fact]
        public void TestParsedGrouplessOptionsWithoutGroup()
        {
            StartOptionParser parser = this.GetSpecialStartOptionParser();
            string[] args = new string[] { "/d", "&verbose" };

            ParsedStartOptions parsed = parser.Parse(args);

            Assert.False(parsed.WasHelpRequested);
            Assert.Null(parsed.ParsedOptionGroup);
            this.AssertHasAllGrouplessOptions(parsed);
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

        private StartOptionParser GetSpecialStartOptionParser()
        {
            HelpOption[] helpOptions = new HelpOption[] { new HelpOption("help", false), new HelpOption("h", true), new HelpOption("?", true) };
            return this.GetStartOptionParser(this.GetParserSettings(), helpOptions);
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
