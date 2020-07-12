using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions;
using Xunit;

namespace StartOptions.Tests
{
    public class HelpOptionBuilderTests
    {
        [Fact]
        public void TestBuildHelpOptionWithShortName()
        {
            HelpOption option = new HelpOptionBuilder("h").SetIsShortName().Build();

            Assert.Equal("h", option.Name);
            Assert.True(option.IsShortName);
        }

        [Fact]
        public void TestBuildHelpOptionWithLongName()
        {
            HelpOption option = new HelpOptionBuilder("help").SetIsLongName().Build();

            Assert.Equal("help", option.Name);
            Assert.False(option.IsShortName);
        }

        [Fact]
        public void TestHelpOptionInvalidNames()
        {
            Assert.Throws<InvalidNameException>(() => new HelpOptionBuilder("-test").Build());
            Assert.Throws<InvalidNameException>(() => new HelpOptionBuilder("123").Build());
            Assert.Throws<InvalidNameException>(() => new HelpOptionBuilder("help1").Build());
            Assert.Throws<InvalidNameException>(() => new HelpOptionBuilder("h?").Build());
            Assert.Throws<InvalidNameException>(() => new HelpOptionBuilder("?1").Build());
        }

        [Fact]
        public void TestHelpOptionValidNames()
        {
            new HelpOptionBuilder("test").Build();
            new HelpOptionBuilder("help").Build();
            new HelpOptionBuilder("h").Build();
            new HelpOptionBuilder("?").Build();
        }
    }
}
