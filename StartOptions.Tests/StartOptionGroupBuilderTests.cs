using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions;
using System.Linq;
using System;
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

            Assert.Equal(StartOptionValueType.Switch, group.ValueType);
            Assert.Equal("long", group.LongName);
            Assert.False(group.IsValueMandatory);
            Assert.Equal("s", group.ShortName);
            Assert.Null(group.Description);
            Assert.False(group.HasValue);
            Assert.Empty(group.Options);
        }

        [Fact]
        public void TestBuildStartOptionGroupWithOptions()
        {
            StartOption option = new StartOptionBuilder("first-option", "fo").SetDescription("First option").Build();
            StartOptionGroupBuilder builder = new StartOptionGroupBuilder("long", "s")
                .AddOption("second-option", "so", (_builder) => _builder.SetDescription("Second option"))
                .SetDescription("Group description")
                .SetValueMandatory()
                .AddOption(option);

            StartOptionGroup group = builder.Build();

            Assert.Equal(StartOptionValueType.Switch, group.ValueType);
            Assert.Equal("long", group.LongName);
            Assert.Equal("s", group.ShortName);
            Assert.Equal(2, group.Options.Count());
            Assert.True(group.IsValueMandatory);
            Assert.False(group.HasValue);

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

        [Fact]
        public void TestBuildWithValueParser()
        {
            StartOptionGroup group1 = new StartOptionGroupBuilder("group1", "g1").SetValueType(StartOptionValueType.Single).Build();
            StartOptionGroup group2 = new StartOptionGroupBuilder("group2", "g2").SetValueType(StartOptionValueType.Multiple).Build();
            StartOptionGroup group3 = new StartOptionGroupBuilder("group3", "g3").SetValueType(StartOptionValueType.Single).SetValueParser(new Int32OptionValueParser()).Build();
            StartOptionGroup group4 = new StartOptionGroupBuilder("group4", "g4").Build();

            Assert.Equal(StartOptionValueType.Single, group1.ValueType);
            
            Assert.Equal(StartOptionValueType.Multiple, group2.ValueType);

            Assert.Equal(StartOptionValueType.Single, group3.ValueType);
            group3.ParseSingleValue("1");
            Assert.True(group3.HasValue);
            Assert.Equal(1, group3.GetValue<Int32>());

            Assert.Equal(StartOptionValueType.Switch, group4.ValueType);
        }
    }
}
