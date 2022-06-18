using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;

namespace StartOptions.Tests.Mocks.Applications
{
    public class MockBuilderApplication : AbstractApplication, IMockApplication
    {
        private readonly bool requireGroup;

        public MockBuilderApplication(bool requireGroup)
        {
            this.requireGroup = requireGroup;
        }

        protected override ApplicationStartOptions GetApplicationStartOptions()
        {
            StartOptionParserSettings settings =  new StartOptionParserSettings()
            {
                RequireStartOptionGroup = this.requireGroup,
                ShortOptionNamePrefix = "-",
                LongOptionNamePrefix = "--",
                OptionValueSeparator = ' '
            };
            List<StartOptionGroup> groups = new List<StartOptionGroup>()
            {
                new StartOptionGroupBuilder("add", "a")
                    .AddOption("number1", "1", _builder => _builder.SetMandatory().SetValueType(StartOptionValueType.Single).SetValueParser(new Int32OptionValueParser()))
                    .AddOption("number2", "2", _builder => _builder.SetMandatory().SetValueType(StartOptionValueType.Single).SetValueParser(new Int32OptionValueParser()))
                    .Build(),
                new StartOptionGroupBuilder("subtract", "s")
                    .AddOption("number1", "1", _builder => _builder.SetMandatory().SetValueType(StartOptionValueType.Single).SetValueParser(new Int32OptionValueParser()))
                    .AddOption("number2", "2", _builder => _builder.SetMandatory().SetValueType(StartOptionValueType.Single).SetValueParser(new Int32OptionValueParser()))
                    .Build()
            };
            List<StartOption> grouplessOptions = new List<StartOption>()
            {
                new StartOptionBuilder("verbose", "v").Build(),
                new StartOptionBuilder("debug", "d").Build()
            };

            return new ApplicationStartOptions(groups, grouplessOptions, settings);
        }

        protected override void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
        {
            //Won't be tested
            throw new System.NotImplementedException();
        }

        protected override void Run(StartOptionGroup selectedGroup, IEnumerable<StartOption> selectedGrouplessOptions)
        {
            this.GrouplessStartOptions = selectedGrouplessOptions;
            this.Group = selectedGroup;
        }

        public IEnumerable<StartOption> GrouplessStartOptions { get; private set; }
        public StartOptionGroup Group { get; private set; }
    }
}