using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Reflection;
using StartOptions.Tests.Mocks.Commands;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System.Linq;
using System;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionReflectionInstantiationTests
    {
        static StartOptionReflectionInstantiationTests()
        {
            StartOptionValueParserRegistry.Register(new CalculationOperationValueParser());
        }

        [Fact]
        public void TestInstantiateBasicCommand()
        {
            Tuple<ReflectionHelper, ParsedStartOptions> tuple = this.GetHelperOptionsTuple(true, typeof(BasicMockCommand), new string[] { "-c", "--number1=4", "-n2=1", "-o=Add" });
            ParsedStartOptions parsedOptions = tuple.Item2;
            ReflectionHelper helper = tuple.Item1;

            IApplicationCommand command = helper.Instantiate(parsedOptions);
            Assert.NotNull(command);
            Assert.IsType<BasicMockCommand>(command);
            Assert.True(command.Execute());
        }

        [Fact]
        public void TestInstantiateWithoutGroup()
        {
            ReflectionHelper helper = this.GetDefaultReflectionHelper(false);
            ParsedStartOptions parsedOptions = this.GetParsedStartOptions(helper, typeof(BasicMockCommand), new string[] { "-v" });

            Assert.Null(helper.Instantiate(parsedOptions));

            helper = this.GetDefaultReflectionHelper(true);
            Assert.Throws<OptionRequirementException>(() => this.GetParsedStartOptions(helper, typeof(BasicMockCommand), new string[] { "-v" }));
        }

        [Fact]
        public void TestInstantiateMultipleConstructor()
        {
            ReflectionHelper helper = this.GetDefaultReflectionHelper(false);

            ParsedStartOptions parsedOptions = this.GetParsedStartOptions(helper, typeof(MultipleConstructorsCommand), new string[] { "-l" });
            Assert.Throws<ListException>(() => helper.Instantiate(parsedOptions).Execute());

            parsedOptions = this.GetParsedStartOptions(helper, typeof(MultipleConstructorsCommand), new string[] { "-a", "-v=s4" });
            Assert.Throws<AddException>(() => helper.Instantiate(parsedOptions).Execute());

            parsedOptions = this.GetParsedStartOptions(helper, typeof(MultipleConstructorsCommand), new string[] { "-r", "-v=s3" });
            Assert.Throws<RemoveException>(() => helper.Instantiate(parsedOptions).Execute());
        }

        private Tuple<ReflectionHelper, ParsedStartOptions> GetHelperOptionsTuple(bool requireGroup, Type commandType, string[] args)
        {
            ReflectionHelper helper = this.GetDefaultReflectionHelper(requireGroup);
            ParsedStartOptions options = this.GetParsedStartOptions(helper, commandType, args);

            return new Tuple<ReflectionHelper, ParsedStartOptions>(helper, options);
        }

        private ParsedStartOptions GetParsedStartOptions(ReflectionHelper helper, Type commandType, string[] args)
        {
            ApplicationStartOptions options = helper.GetStartOptions(commandType);

            StartOptionParser parser = new StartOptionParser(options.StartOptionParserSettings, options.StartOptionGroups, options.GrouplessStartOptions, options.HelpOptions);
            return parser.Parse(args);
        }

        private ReflectionHelper GetDefaultReflectionHelper(bool requireGroup)
        {
            IEnumerable<HelpOption> helpOptions = new HelpOption[] { new HelpOption("help", false), new HelpOption("h", true) };
            StartOptionParserSettings settings = new StartOptionParserSettings() { RequireStartOptionGroup = requireGroup };

            return new ReflectionHelper(helpOptions, settings);
        }
    }

    internal class CalculationOperationValueParser : IStartOptionValueParser
    {
        public object ParseValue(string value)
        {
            if (Int32.TryParse(value, out int result))
            {
                return (CalculationOperation)result;
            }
            return Enum.GetValues(typeof(CalculationOperation)).Cast<CalculationOperation>().Single(_value => _value.ToString().Equals(value));
        }

        public object[] ParseValues(string[] values)
        {
            return values.Select(_value => this.ParseValue(_value)).ToArray();
        }
    }
}
