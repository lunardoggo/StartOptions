using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions.Parsing;
using LunarDoggo.StartOptions;
using System.Linq;
using System;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionParserParsingTests : BaseStartOptionParserParsingTests
    {
        private static readonly HelpOption[] HelpOptions = new HelpOption[] { new HelpOption("help", false), new HelpOption("h", true), new HelpOption("?", true) };

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

            Assert.False(group.HasValue);
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

            Assert.False(group.HasValue);
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

        [Fact]
        public void TestParsedWithRequiredOptions()
        {
            StartOptionParser parser = this.GetOptionParserWithRequiredOptions();

            Assert.Throws<OptionRequirementException>(() => parser.Parse(new string[] { "-g", "-gr", "-o" }));
            Assert.Throws<OptionRequirementException>(() => parser.Parse(new string[] { "-g", "-go", "-r" }));
            Assert.Throws<OptionRequirementException>(() => parser.Parse(new string[] { "-g", "-go", "-o" }));

            ParsedStartOptions firstOptions = parser.Parse(new string[] { "-g", "-gr", "-r" });
            Assert.NotNull(firstOptions.ParsedOptionGroup);
            Assert.False(firstOptions.ParsedOptionGroup.HasValue);
            Assert.Contains(firstOptions.ParsedOptionGroup.Options, _option => _option.ShortName.Equals("gr"));
            Assert.Contains(firstOptions.ParsedGrouplessOptions, _option => _option.ShortName.Equals("r"));

            ParsedStartOptions secondOptions = parser.Parse(new string[] { "-g", "-gr", "-go", "-r", "-o" });
            Assert.NotNull(secondOptions.ParsedOptionGroup);
            Assert.False(secondOptions.ParsedOptionGroup.HasValue);
            Assert.Contains(secondOptions.ParsedOptionGroup.Options, _option => _option.ShortName.Equals("gr"));
            Assert.Contains(secondOptions.ParsedOptionGroup.Options, _option => _option.ShortName.Equals("go"));
            Assert.Contains(secondOptions.ParsedGrouplessOptions, _option => _option.ShortName.Equals("r"));
            Assert.Contains(secondOptions.ParsedGrouplessOptions, _option => _option.ShortName.Equals("o"));

            //Make sure that a request for the help-page ignores the option requirements
            parser.Parse(new string[] { "-h" });
        }

        [Fact]
        public void TestRequireStartOptionGroup()
        {
            StartOptionParser parser = this.GetOptionParserWithRequiredOptions();

            Assert.Throws<OptionRequirementException>(() => parser.Parse(new string[] { "-o", "-r" }));
            Assert.Throws<OptionRequirementException>(() => parser.Parse(new string[] { "-r" }));

            ParsedStartOptions firstOptions = parser.Parse(new string[] { "-g", "-gr", "-r" });
            Assert.NotNull(firstOptions.ParsedOptionGroup);
            Assert.Contains(firstOptions.ParsedOptionGroup.Options, _option => _option.ShortName.Equals("gr"));

            //Make sure that a request for the help-page ignores the option group requirements
            parser.Parse(new string[] { "-h" });
        }

        [Fact]
        public void TestParseStartOptionGroupWithStringValue()
        {
            StartOptionParser parser = this.GetOptionParserWithRequiredOptions(StartOptionValueType.Single, null);

            ParsedStartOptions firstOptions = parser.Parse(new string[] { "-g=test", "-gr", "-r" });
            StartOptionGroup group = firstOptions.ParsedOptionGroup;
            Assert.NotNull(group);
            Assert.True(group.HasValue);
            Assert.Equal("test", group.GetValue<string>());
            Assert.Contains(firstOptions.ParsedOptionGroup.Options, _option => _option.ShortName.Equals("gr"));
        }

        [Fact]
        public void TestParseStartOptionGroupWithIntValue()
        {
            StartOptionParser parser = this.GetOptionParserWithRequiredOptions(StartOptionValueType.Single, new Int32OptionValueParser());

            ParsedStartOptions firstOptions = parser.Parse(new string[] { "-g=42", "-gr", "-r" });
            StartOptionGroup group = firstOptions.ParsedOptionGroup;
            Assert.NotNull(group);
            Assert.True(group.HasValue);
            Assert.Equal(42, group.GetValue<Int32>());
            Assert.Contains(firstOptions.ParsedOptionGroup.Options, _option => _option.ShortName.Equals("gr"));
        }

        private StartOptionParser GetOptionParserWithRequiredOptions(StartOptionValueType groupValueType = StartOptionValueType.Switch, IStartOptionValueParser groupValueParser = null)
        {
            StartOptionGroup[] groups = new StartOptionGroup[]
            {
                new StartOptionGroupBuilder("group", "g")
                    .SetValueParser(groupValueParser)
                    .SetValueType(groupValueType)
                    .AddOption("groupOptional", "go", _builder => _builder.SetMandatory(false))
                    .AddOption("groupRequired", "gr", _builder => _builder.SetMandatory(true))
                    .Build()
            };

            StartOption[] grouplessOptions = new StartOption[]
            {
                new StartOptionBuilder("optional", "o").SetMandatory(false).Build(),
                new StartOptionBuilder("required", "r").SetMandatory(true).Build()
            };

            StartOptionParserSettings settings = new StartOptionParserSettings()
            {
                RequireStartOptionGroup = true
            };

            return new StartOptionParser(settings, groups, grouplessOptions, StartOptionParserParsingTests.HelpOptions);
        }

        private StartOptionParser GetDefaultStartOptionParser()
        {
            return base.GetStartOptionParser(new StartOptionParserSettings(), StartOptionParserParsingTests.HelpOptions);
        }
    }
}
