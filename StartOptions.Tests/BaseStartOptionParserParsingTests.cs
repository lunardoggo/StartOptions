using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;

namespace StartOptions.Tests
{
    public abstract class BaseStartOptionParserParsingTests
    {
        protected StartOptionParser GetStartOptionParser(StartOptionParserSettings settings, HelpOption[] helpOptions)
        {
            IEnumerable<StartOptionGroup> groups = this.GetStartOptionGroups();
            IEnumerable<StartOption> options = this.GetGrouplessStartOptions();

            return new StartOptionParser(settings, groups, options, helpOptions);
        }

        private IEnumerable<StartOptionGroup> GetStartOptionGroups()
        {
            List<StartOptionGroup> groups = new List<StartOptionGroup>();

            StartOptionGroup first = new StartOptionGroupBuilder("export", "e").SetDescription("Exports an user to a file")
                .AddOption("path", "p", (_builder) => _builder.SetDescription("Output file path").SetMandatory().SetValueType(StartOptionValueType.Single))
                .AddOption("user", "u", (_builder) => _builder.SetDescription("User to export").SetMandatory().SetValueType(StartOptionValueType.Single))
                .Build();

            StartOptionGroup second = new StartOptionGroupBuilder("import", "i").SetDescription("Imports an user from a file")
                .AddOption("path", "p", (_builder) => _builder.SetDescription("Input file path").SetMandatory().SetValueType(StartOptionValueType.Single))
                .AddOption("force", "f", (_builder) => _builder.SetDescription("Force a pre-existing user record to be overwritten"))
                .Build();

            StartOptionGroup third = new StartOptionGroupBuilder("sum", "s").SetDescription("Sums up all provided numbers")
                .SetValueParser(new Int32OptionValueParser()).SetValueType(StartOptionValueType.Multiple).Build();

            groups.Add(first);
            groups.Add(second);
            groups.Add(third);

            return groups;
        }

        private IEnumerable<StartOption> GetGrouplessStartOptions()
        {
            return new List<StartOption>
            {
                new StartOptionBuilder("names", "n").SetDescription("Sets names").SetValueType(StartOptionValueType.Multiple).Build(),
                new StartOptionBuilder("debug", "d").SetDescription("Waits for debugger on application start").Build(),
                new StartOptionBuilder("verbose", "v").SetDescription("Adds more verbosity to output").Build()
            };
        }
    }
}
