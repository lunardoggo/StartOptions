using LunarDoggo.StartOptions.Parsing;
using LunarDoggo.StartOptions;
using System.Linq;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionParserDefaultSettingsParsingTests : BaseStartOptionParserParsingTests
    {
        [Fact]
        public void TestWasHelpRequested()
        {
            StartOptionParser parser = this.GetDefaultStartOptionParser();

            this.AssertWasHelpRequested(parser, "-h");
            this.AssertWasHelpRequested(parser, "--help");
            this.AssertWasHelpRequested(parser, "-?");
            this.AssertWasHelpRequested(parser, "-h", "-i", "-p=./user.txt");
        }

        [Fact]
        public void TestParsedOptionGroupWithoutGrouplessOptions()
        {
            StartOptionParser parser = this.GetDefaultStartOptionParser();
            string[] args = new string[] { "-e", "-u=testuser", "-p=./user.txt" };

            ParsedStartOptions parsed = parser.Parse(args);

            Assert.False(parsed.WasHelpRequested);
            Assert.Empty(parsed.ParsedGrouplessOptions);

            Assert.NotNull(parsed.ParsedOptionGroup);
            StartOptionGroup group = parsed.ParsedOptionGroup;

            Assert.Equal("e", group.ShortName);
            this.AssertStartOptionHasValue(group.GetOptionByShortName("u"), "testuser");
            this.AssertStartOptionHasValue(group.GetOptionByShortName("p"), "./user.txt");
        }

        [Fact]
        public void TestParsedOptionGroupWithGrouplessOptions()
        {
            StartOptionParser parser = this.GetDefaultStartOptionParser();
            string[] args = new string[] { "-i", "-p=./user.txt", "-f", "-d", "-v" };

            ParsedStartOptions parsed = parser.Parse(args);

            Assert.False(parsed.WasHelpRequested);
            this.AssertHasAllGrouplessOptions(parsed);

            Assert.NotNull(parsed.ParsedOptionGroup);
            StartOptionGroup group = parsed.ParsedOptionGroup;

            Assert.Equal("i", group.ShortName);
            this.AssertStartOptionHasValue(group.GetOptionByShortName("p"), "./user.txt");
            Assert.NotNull(group.GetOptionByShortName("f"));
        }

        [Fact]
        public void TestParsedGrouplessOptionsWithoutGroup()
        {
            StartOptionParser parser = this.GetDefaultStartOptionParser();
            string[] args = new string[] { "-d", "-v" };

            ParsedStartOptions parsed = parser.Parse(args);

            Assert.False(parsed.WasHelpRequested);
            Assert.Null(parsed.ParsedOptionGroup);
            this.AssertHasAllGrouplessOptions(parsed);
        }

        [Fact]
        public void TestParsedMultiValueGrouplessOptionWithoutGroup()
        {
            StartOptionParser parser = this.GetDefaultStartOptionParser();
            string[] args = new string[] { "-n=test1,test2,test3" };

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

        private StartOptionParser GetDefaultStartOptionParser()
        {
            HelpOption[] helpOptions = new HelpOption[] { new HelpOption("help", false), new HelpOption("h", true), new HelpOption("?", true) };
            return this.GetStartOptionParser(new StartOptionParserSettings(), helpOptions);
        }
    }
}
