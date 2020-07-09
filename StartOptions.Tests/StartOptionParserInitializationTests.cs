using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System.Linq;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionParserInitializationTests
    {
        [Fact]
        public void TestGrouplessStartOptionShortNameConflict()
        {
            StartOption[] options = new StartOption[]
            {
                new StartOptionBuilder("port", "p").Build(),
                new StartOptionBuilder("path", "p").Build()
            };

            Assert.Throws<NameConflictException>(() => new StartOptionParser(null, options, StartOptionParser.DefaultHelpOptions));
        }

        [Fact]
        public void TestGrouplessStartOptionLongNameConflict()
        {
            StartOption[] options = new StartOption[]
            {
                new StartOptionBuilder("path", "p1").Build(),
                new StartOptionBuilder("path", "p2").Build()
            };

            Assert.Throws<NameConflictException>(() => new StartOptionParser(null, options, StartOptionParser.DefaultHelpOptions));
        }

        [Fact]
        public void TestStartOptionGroupShortNameConflict()
        {
            StartOptionGroup[] groups = new StartOptionGroup[]
            {
                new StartOptionGroupBuilder("group1", "g").AddOption("option", "o", (_builder) => { }).Build(),
                new StartOptionGroupBuilder("group2", "g").AddOption("option", "o", (_builder) => { }).Build(),
            };

            Assert.Throws<NameConflictException>(() => new StartOptionParser(groups, null, StartOptionParser.DefaultHelpOptions));
        }

        [Fact]
        public void TestStartOptionGroupLongNameConflict()
        {
            StartOptionGroup[] groups = new StartOptionGroup[]
            {
                new StartOptionGroupBuilder("group", "g1").AddOption("option", "o", (_builder) => { }).Build(),
                new StartOptionGroupBuilder("group", "g2").AddOption("option", "o", (_builder) => { }).Build(),
            };

            Assert.Throws<NameConflictException>(() => new StartOptionParser(groups, null, StartOptionParser.DefaultHelpOptions));
        }

        [Fact]
        public void TestStartOptionParserUnknownOption()
        {
            StartOption[] options = new StartOption[]
            {
                new StartOptionBuilder("debug", "d").Build()
            };
            StartOptionParser ignoreUnknownOptions = this.GetUnknownOptionsIgnoringParser(null, options);
            StartOptionParser throwOnUnknownOption = this.GetDefaultStartOptionParser(null, options);
            string[] args = new string[] { "-d", "-n" };

            Assert.Throws<UnknownOptionNameException>(() => throwOnUnknownOption.Parse(args));

            ParsedStartOptions parsed = ignoreUnknownOptions.Parse(args);

            Assert.Null(parsed.ParsedOptionGroup);
            Assert.Single(parsed.ParsedGrouplessOptions);
            Assert.Equal("d", parsed.ParsedGrouplessOptions.Single().ShortName);
        }

        [Fact]
        public void TestGrouplessOptionOptionGroupLongNameConflict()
        {
            StartOptionGroup group = new StartOptionGroupBuilder("verify", "v").AddOption("name", "n", (_option) => { }).Build();
            StartOption option = new StartOptionBuilder("verify", "v1").Build();

            Assert.Throws<NameConflictException>(() => new StartOptionParser(new[] { group }, new[] { option }, StartOptionParser.DefaultHelpOptions));
        }

        [Fact]
        public void TestGrouplessOptionOptionGroupShortNameConflict()
        {
            StartOptionGroup group = new StartOptionGroupBuilder("verify", "v").AddOption("name", "n", (_option) => { }).Build();
            StartOption option = new StartOptionBuilder("verbose", "v").Build();

            Assert.Throws<NameConflictException>(() => new StartOptionParser(new[] { group }, new[] { option }, StartOptionParser.DefaultHelpOptions));
        }

        [Fact]
        public void TestGrouplessOptionOptionGroupOptionLongNameConflict()
        {
            StartOptionGroup group = new StartOptionGroupBuilder("verify", "v").AddOption("name", "n", (_option) => { }).Build();
            StartOption option = new StartOptionBuilder("name", "n1").Build();

            Assert.Throws<NameConflictException>(() => new StartOptionParser(new[] { group }, new[] { option }, StartOptionParser.DefaultHelpOptions));
        }

        [Fact]
        public void TestGrouplessOptionOptionGroupOptionShortNameConflict()
        {
            StartOptionGroup group = new StartOptionGroupBuilder("verify", "v").AddOption("name", "n", (_option) => { }).Build();
            StartOption option = new StartOptionBuilder("new", "n").Build();

            Assert.Throws<NameConflictException>(() => new StartOptionParser(new[] { group }, new[] { option }, StartOptionParser.DefaultHelpOptions));
        }

        [Fact]
        public void TestGrouplessOptionHelpNameConflict()
        {
            StartOption option = new StartOptionBuilder("home", "h").Build();

            Assert.Throws<NameConflictException>(() => new StartOptionParser(null, new[] { option }, StartOptionParser.DefaultHelpOptions));
        }

        [Fact]
        public void TestOptionGroupOptionNameHelpConflict()
        {
            StartOptionGroup group = new StartOptionGroupBuilder("group", "g").AddOption("home", "h", (_option) => { }).Build();

            Assert.Throws<NameConflictException>(() => new StartOptionParser(new[] { group }, null, StartOptionParser.DefaultHelpOptions));
        }

        [Fact]
        public void TestOptionGroupNameHelpConflict()
        {
            StartOptionGroup group = new StartOptionGroupBuilder("home", "h").AddOption("opt", "o", (_option) => { }).Build();

            Assert.Throws<NameConflictException>(() => new StartOptionParser(new[] { group }, null, StartOptionParser.DefaultHelpOptions));
        }

        private StartOptionParser GetUnknownOptionsIgnoringParser(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
        {
            StartOptionParserSettings settings = new StartOptionParserSettings()
            {
                ThrowErrorOnUnknownOption = false
            };
            return new StartOptionParser(settings, groups, grouplessOptions, StartOptionParser.DefaultHelpOptions);
        }

        private StartOptionParser GetDefaultStartOptionParser(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
        {
            return new StartOptionParser(groups, grouplessOptions, StartOptionParser.DefaultHelpOptions);
        }
    }
}
