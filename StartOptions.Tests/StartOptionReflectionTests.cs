using LunarDoggo.StartOptions.Reflection;
using StartOptions.Tests.Mocks.Commands;
using LunarDoggo.StartOptions;
using System.Linq;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionReflectionTests
    {
        [Fact]
        public void TestGetStartOptions()
        {
            ApplicationStartOptions options = ReflectionHelper.GetStartOptions(typeof(BasicMockCommand));

            Assert.True(options.StartOptionGroups.Count() == 1);
            StartOptionGroup group = options.StartOptionGroups.Single();
            this.AssertStartOptionGroup("calculate", "c", "Executes a calculation", group);
            this.AssertStartOption("number1", "n1", "First number of the calculation", true, StartOptionValueType.Single, group.GetOptionByShortName("n1"));
            this.AssertStartOption("number2", "n2", "Second number of the calculation", true, StartOptionValueType.Single, group.GetOptionByShortName("n2"));
            this.AssertStartOption("operation", "o", "Operation to execute", true, StartOptionValueType.Single, group.GetOptionByShortName("o"));

            Assert.True(options.GrouplessStartOptions.Count() == 1);
            this.AssertStartOption("verbose", "v", "Enable verbose output", false, StartOptionValueType.Switch, options.GrouplessStartOptions.Single());
        }

        [Fact]
        public void TestMultipleConstructors()
        {
            ApplicationStartOptions options = ReflectionHelper.GetStartOptions(typeof(MultipleConstructorsCommand));

            Assert.True(options.GrouplessStartOptions.Count() == 0);
            Assert.True(options.StartOptionGroups.Count() == 3);

            StartOptionGroup list = options.StartOptionGroups.Single(_grp => _grp.LongName.Equals("list"));
            this.AssertStartOptionGroup("list", "l", "Lists strings stored in the list", list);
            this.AssertStartOption("inLine", "i", "Displays the items in a line instead of seperate lines", false, StartOptionValueType.Switch, list.GetOptionByShortName("i"));

            StartOptionGroup add = options.StartOptionGroups.Single(_grp => _grp.LongName.Equals("add"));
            this.AssertStartOptionGroup("add", "a", "Adds a new value to the stored list", add);
            this.AssertStartOption("value", "v", "Value to be added", true, StartOptionValueType.Single, add.GetOptionByShortName("v"));

            StartOptionGroup remove = options.StartOptionGroups.Single(_grp => _grp.LongName.Equals("remove"));
            this.AssertStartOptionGroup("remove", "r", "Removes a value from the stored list", remove);
            this.AssertStartOption("value", "v", "Value to be added", true, StartOptionValueType.Single, remove.GetOptionByShortName("v"));
            this.AssertStartOption("ignoreErrors", "i", "Ignores the error if the value is not present in the list", false, StartOptionValueType.Switch, remove.GetOptionByShortName("i"));
        }

        private void AssertStartOptionGroup(string longName, string shortName, string description, StartOptionGroup group)
        {
            Assert.Equal(longName, group.LongName);
            Assert.Equal(shortName, group.ShortName);
            Assert.Equal(description, group.Description);
        }

        private void AssertStartOption(string longName, string shortName, string description, bool mandatory, StartOptionValueType valueType, StartOption option)
        {
            Assert.Equal(longName, option.LongName);
            Assert.Equal(shortName, option.ShortName);
            Assert.Equal(description, option.Description);
            Assert.Equal(mandatory, option.IsRequired);
            Assert.Equal(valueType, option.ValueType);
        }
    }
}
