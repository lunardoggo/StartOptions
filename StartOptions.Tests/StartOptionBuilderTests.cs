using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionBuilderTests
    {
        [Fact]
        public void TestBuildStartOptionWithDefaultValues()
        {
            StartOptionBuilder builder = new StartOptionBuilder("long", "s");

            StartOption option = builder.Build();

            Assert.Equal(StartOptionValueType.Switch, option.ValueType);
            Assert.Equal("s", option.ShortName);
            Assert.Equal("long", option.LongName);
            Assert.Null(option.Description);

            Assert.False(option.IsMandatory);
            Assert.False(option.HasValue);
        }

        [Fact]
        public void TestBuildStartOption()
        {
            StartOptionBuilder builder = new StartOptionBuilder("long", "s")
                .SetValueType(StartOptionValueType.Single)
                .SetDescription("description")
                .SetMandatory();

            StartOption option = builder.Build();

            Assert.Equal(StartOptionValueType.Single, option.ValueType);
            Assert.Equal("description", option.Description);
            Assert.Equal("s", option.ShortName);
            Assert.Equal("long", option.LongName);

            Assert.True(option.IsMandatory);

            Assert.False(option.HasValue);
        }

        [Fact]
        public void TestInvalidOptionNames()
        {
            //Start with "-"
            Assert.Throws<InvalidNameException>(() => new StartOptionBuilder("-long", "short"));
            Assert.Throws<InvalidNameException>(() => new StartOptionBuilder("long", "-short"));

            //Contains invalid spaces
            Assert.Throws<InvalidNameException>(() => new StartOptionBuilder("long option", "short"));
            Assert.Throws<InvalidNameException>(() => new StartOptionBuilder("long", "short option"));

            //Contains invalid chars
            Assert.Throws<InvalidNameException>(() => new StartOptionBuilder("long!option", "short"));
            Assert.Throws<InvalidNameException>(() => new StartOptionBuilder("long", "short§option"));
        }

        [Fact]
        public void TestValidOptionNames()
        {
            new StartOptionBuilder("long_option", "s");
            new StartOptionBuilder("long", "short_option");

            new StartOptionBuilder("long-option", "s");
            new StartOptionBuilder("long", "short-option");
        }
    }
}
