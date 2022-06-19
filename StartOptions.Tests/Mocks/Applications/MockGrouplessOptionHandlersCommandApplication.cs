using LunarDoggo.StartOptions.DependencyInjection;
using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System;

namespace StartOptions.Tests.Mocks.Applications
{
    [GrouplessStartOption("numbers", "n", ValueType = StartOptionValueType.Multiple, ParserType = typeof(Int32OptionValueParser))]
    [GrouplessStartOption("messageOfTheDay", "motd", ValueType = StartOptionValueType.Single)]
    [GrouplessStartOption("words", "w", ValueType = StartOptionValueType.Multiple)]
    [GrouplessStartOption("verbose", "v")]
    public class MockGrouplessOptionHandlersCommandApplication : CommandApplication
    {
        public MockGrouplessOptionHandlersCommandApplication(HandlingMode mode)
        {
            if(mode == HandlingMode.Correct)
            {
                base.AddGlobalGrouplessStartOptionHandler<string>("messageOfTheDay", _value => this.MessageOfTheDay = _value);
                base.AddGlobalGrouplessStartOptionHandler<string>("words", _values => this.Words = _values);
                base.AddGlobalGrouplessStartOptionHandler<int>("numbers", _values => this.Numbers = _values);
                base.AddGlobalGrouplessStartOptionHandler("verbose", () => this.Verbose = true);
            }
            else if(mode == HandlingMode.Incorrect)
            {
                base.AddGlobalGrouplessStartOptionHandler<int>("messageOfTheDay", (int _value) => this.MessageOfTheDay = _value.ToString());
                base.AddGlobalGrouplessStartOptionHandler<string>("words", (string _values) => this.Words = new[] { _values });
                base.AddGlobalGrouplessStartOptionHandler<double>("numbers", (double[] _values) => this.Numbers = new int[0]);
            }
        }

        protected override Type[] GetCommandTypes()
        {
            return new Type[] { typeof(MockCommand) };
        }

        protected override IDependencyProvider GetDependencyProvider()
        {
            return null;
        }

        public string MessageOfTheDay { get; private set; }
        public string[] Words { get; private set; }
        public int[] Numbers { get; private set; }
        public bool Verbose { get; private set; }

        protected override void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
        {
            //Won't be tested
            throw new NotImplementedException();
        }

        public class MockCommand : IApplicationCommand
        {
            [StartOptionGroup("group", "g")]
            public MockCommand()
            { }

            public void Execute()
            { }
        }

        public enum HandlingMode
        {
            Incorrect,
            Correct,
            None
        }
    }
}
