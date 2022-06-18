using LunarDoggo.StartOptions.DependencyInjection;
using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System;
using StartOptions.Tests.Mocks.Applications;

namespace StartOptions.Tests.Mocks
{
    [GrouplessStartOption("verbose", "v")]
    [GrouplessStartOption("debug", "d")]
    public class MockCommandApplication : CommandApplication, IMockApplication
    {
        private readonly bool requireGroup;

        public MockCommandApplication(bool requireGroup)
        {
            this.requireGroup = requireGroup;
        }

        protected override Type[] GetCommandTypes()
        {
            return new Type[] { typeof(AddCommand), typeof(SubtractCommand) };
        }

        protected override StartOptionParserSettings GetParserSettings()
        {
            return new StartOptionParserSettings()
            {
                RequireStartOptionGroup = this.requireGroup,
                ShortOptionNamePrefix = "-",
                LongOptionNamePrefix = "--",
                OptionValueSeparator = ' '
            };
        }

        protected override IDependencyProvider GetDependencyProvider()
        {
            return null;
        }

        protected override void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
        {
            //Won't be tested
            throw new NotImplementedException();
        }

        protected override void Run(StartOptionGroup selectedGroup, IEnumerable<StartOption> selectedGrouplessOptions)
        {
            this.GrouplessStartOptions = selectedGrouplessOptions;
            this.Group = selectedGroup;
            
            base.Run(selectedGroup, selectedGrouplessOptions);
        }

        public IEnumerable<StartOption> GrouplessStartOptions { get; private set; }
        public StartOptionGroup Group { get; private set; }

        public class AddCommand : IApplicationCommand
        {
            [StartOptionGroup("add", "a")]
            public AddCommand([StartOption("number1", "1", ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))]int num1,
                              [StartOption("number2", "2", ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))]int num2,
                              [GrouplessStartOptionReference("verbose")]bool verbose,
                              [GrouplessStartOptionReference("debug")]bool debug)
            { }

            public void Execute()
            { }
        }

        public class SubtractCommand : IApplicationCommand
        {
            [StartOptionGroup("subtract", "s")]
            public SubtractCommand([StartOption("number1", "1", ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))]int num1,
                                   [StartOption("number2", "2", ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))]int num2,
                                   [GrouplessStartOptionReference("verbose")]bool verbose,
                                   [GrouplessStartOptionReference("debug")]bool debug)
            { }

            public void Execute()
            { }
        }
    }
}