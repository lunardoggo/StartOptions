﻿using LunarDoggo.StartOptions.Parsing.Values;
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
            command.Execute();
        }

        [Fact]
        public void TestInstantiateUnsetOptions()
        {
            this.AssertNullValuesCommand(null, 0, 0, false, false, new[] { "-g", "-s" });
            this.AssertNullValuesCommand(null, 0, 0, false, false, new[] { "-g" });
            this.AssertNullValuesCommand("abc", 0, 0, false, false, new[] { "-g", "-s=abc" });
            this.AssertNullValuesCommand(null, 0, 0, true, true, new[] { "-g", "-sw", "-bo=true" });
            this.AssertNullValuesCommand(null, 4, 6, false, false, new[] { "-g", "-i=4", "-by=6" });
            this.AssertNullValuesCommand("abc", 15, 110, false, true, new[] { "-g", "-s=abc", "-sw", "-bo=false", "-i=15", "-by=110" });
            this.AssertNullValuesCommand(null, 0, 0, false, true, new[] { "-g", "-s", "-sw", "-bo", "-i", "-by" });
        }

        private void AssertNullValuesCommand(string stringValue, int intValue, byte byteValue, bool boolValue, bool switchValue, string[] args)
        {
            Tuple<ReflectionHelper, ParsedStartOptions> tuple = this.GetHelperOptionsTuple(true, typeof(NullValuesCommand), args);
            ParsedStartOptions parsedOptions = tuple.Item2;
            ReflectionHelper helper = tuple.Item1;

            IApplicationCommand command = helper.Instantiate(parsedOptions);
            Assert.NotNull(command);
            Assert.IsType<NullValuesCommand>(command);
            NullValuesCommand nullCommand = command as NullValuesCommand;

            Assert.Equal(intValue, nullCommand.IntValue);
            Assert.Equal(boolValue, nullCommand.BoolValue);
            Assert.Equal(byteValue, nullCommand.ByteValue);
            Assert.Equal(stringValue, nullCommand.StringValue);
            Assert.Equal(switchValue, nullCommand.SwitchValue);
        }

        [Fact]
        public void TestInstantiateWithoutGroup()
        {
            ReflectionHelper helper = this.GetDefaultReflectionHelper(false);
            ParsedStartOptions parsedOptions = this.GetParsedStartOptions(helper, typeof(BasicMockCommand), new string[] { "-vb" });

            Assert.Null(helper.Instantiate(parsedOptions));

            helper = this.GetDefaultReflectionHelper(true);
            Assert.Throws<OptionRequirementException>(() => this.GetParsedStartOptions(helper, typeof(BasicMockCommand), new string[] { "-vb" }));
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
