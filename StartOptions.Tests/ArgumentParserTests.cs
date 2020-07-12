using LunarDoggo.StartOptions.Parsing.Arguments;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace StartOptions.Tests
{
    public class ArgumentParserTests
    {
        [Fact]
        public void TestArrayBasedArgumentParser()
        {
            BaseArgumentParser parser = new ArrayBasedArgumentParser(new StartOptionParserSettings());
            string[] args = new string[] { "-a", "--arg", "-v=test" };

            this.ExecuteIArgumentParserTests(parser, args);
        }

        [Fact]
        public void TestSeparatorBasedArgumentParser()
        {
            BaseArgumentParser parser = new SeparatorBasedArgumentParser(new StartOptionParserSettings());
            string[] args = new string[] { "-a", "--arg", "-v", "test" };

            this.ExecuteIArgumentParserTests(parser, args);
        }

        private void ExecuteIArgumentParserTests(BaseArgumentParser parser, string[] args)
        {
            List<ParsedStartArgument> parsed = parser.Parse(args).ToList();

            this.AssertParsedArgumentValues(parsed.Single(_arg => _arg.Name.Equals("a")),
                "-a", true, null);
            this.AssertParsedArgumentValues(parsed.Single(_arg => _arg.Name.Equals("arg")),
                "--arg", false, null);
            this.AssertParsedArgumentValues(parsed.Single(_arg => _arg.Name.Equals("v")),
                "-v", true, "test");
        }

        private void AssertParsedArgumentValues(ParsedStartArgument argument, string assertedFullName, 
                                                bool isShortName, object value)
        {
            Assert.Equal(assertedFullName, argument.NameWithPrefix);
            Assert.Equal(isShortName, argument.IsShortName);
            Assert.Equal(value != null, argument.HasValue);
            Assert.Equal(value, argument.Value);
        }
    }
}
