using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System.Linq;
using Xunit;

namespace StartOptions.Tests
{
    public abstract class BaseStartOptionParserParsingTests
    {
        protected void AssertWasHelpRequested(StartOptionParser parser, params string[] args)
        {
            Assert.True(parser.Parse(args).WasHelpRequested);
        }

        protected void AssertStartOptionHasValue<T>(StartOption option, T value)
        {
            Assert.True(option.HasValue);
            Assert.Equal(value, option.GetValue<T>());
        }

        protected void AssertHasAllGrouplessOptions(ParsedStartOptions parsed)
        {
            Assert.Equal(2, parsed.ParsedGrouplessOptions.Count());
            Assert.NotNull(parsed.ParsedGrouplessOptions.SingleOrDefault(_option => _option.ShortName.Equals("d")));
            Assert.NotNull(parsed.ParsedGrouplessOptions.SingleOrDefault(_option => _option.ShortName.Equals("v")));
        }

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

            groups.Add(first);
            groups.Add(second);

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
