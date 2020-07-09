using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions;
using System.Linq;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionGroupBuilderTests
    {
        [Fact]
        public void TestBuildStartOptionGroupWithDefaultValues()
        {
            StartOptionGroupBuilder builder = new StartOptionGroupBuilder("long", "s");

            StartOptionGroup group = builder.Build();

            Assert.Equal("s", group.ShortName);
            Assert.Equal("long", group.LongName);
            Assert.Null(group.Description);
            Assert.Empty(group.Options);
        }

        [Fact]
        public void TestBuildStartOptionGroupWithOptions()
        {
            StartOption option = new StartOptionBuilder("first-option", "fo").SetDescription("First option").Build();
            StartOptionGroupBuilder builder = new StartOptionGroupBuilder("long", "s")
                .AddOption("second-option", "so", (_builder) => _builder.SetDescription("Second option"))
                .SetDescription("Group description")
                .AddOption(option);

            StartOptionGroup group = builder.Build();

            Assert.Equal("long", group.LongName);
            Assert.Equal("s", group.ShortName);
            Assert.Equal(2, group.Options.Count());
            this.AssertHasOption(group, "fo");
            this.AssertHasOption(group, "so");
        }

        private void AssertHasOption(StartOptionGroup group, string shortName)
        {
            Assert.NotNull(group.GetOptionByShortName(shortName));
        }

        [Fact]
        public void TestInvalidOptionNames()
        {
            //Start with "-"
            Assert.Throws<InvalidNameException>(() => new StartOptionGroupBuilder("-long", "short"));
            Assert.Throws<InvalidNameException>(() => new StartOptionGroupBuilder("long", "-short"));

            //Contains invalid spaces
            Assert.Throws<InvalidNameException>(() => new StartOptionGroupBuilder("long option", "short"));
            Assert.Throws<InvalidNameException>(() => new StartOptionGroupBuilder("long", "short option"));

            //Contains invalid chars
            Assert.Throws<InvalidNameException>(() => new StartOptionGroupBuilder("long!option", "short"));
            Assert.Throws<InvalidNameException>(() => new StartOptionGroupBuilder("long", "short§option"));
        }

        [Fact]
        public void TestValidOptionGroupNames()
        {
            new StartOptionGroupBuilder("long_option", "s");
            new StartOptionGroupBuilder("long", "short_option");

            new StartOptionGroupBuilder("long-option", "s");
            new StartOptionGroupBuilder("long", "short-option");
        }

        [Fact]
        public void TestGroupOptionNameConflict()
        {
            StartOptionGroupBuilder builder = new StartOptionGroupBuilder("long", "s");

            Assert.Throws<NameConflictException>(() => builder.AddOption("long", "so", (_builder) => { }));
            Assert.Throws<NameConflictException>(() => builder.AddOption("option", "s", (_builder) => { }));
        }

        [Fact]
        public void TestDuplicateOptionName()
        {
            StartOptionGroupBuilder builder = new StartOptionGroupBuilder("long", "s")
                .AddOption("option", "o", (_builder) => { });

            Assert.Throws<NameConflictException>(() => builder.AddOption("option", "opt", (_builder) => { }));
            Assert.Throws<NameConflictException>(() => builder.AddOption("option1", "o", (_builder) => { }));
        }
    }
}
