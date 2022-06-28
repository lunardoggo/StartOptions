using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Reflection;
using StartOptions.Tests.Mocks.Commands;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System.Reflection;
using System.Linq;
using System;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionReflectionTests
    {
        [Fact]
        public void TestGetFromMultipleCommands()
        {
            ApplicationStartOptions options = this.GetDefaultReflectionHelper().GetStartOptions(typeof(BasicMockCommand), typeof(MultipleConstructorsCommand));

            Assert.Equal(4, options.StartOptionGroups.Count());
            Assert.Single(options.GrouplessStartOptions);
        }

        [Fact]
        public void TestGetStartOptions()
        {
            ApplicationStartOptions options = this.GetDefaultReflectionHelper().GetStartOptions(typeof(BasicMockCommand));

            Assert.True(options.StartOptionGroups.Count() == 1);
            StartOptionGroup group = options.StartOptionGroups.Single();
            AssertionUtility.StartOptionGroup("calculate", "c", "Executes a calculation", group);
            AssertionUtility.StartOption("number1", "n1", "First number of the calculation", true, StartOptionValueType.Single, group.GetOptionByShortName("n1"));
            AssertionUtility.StartOption("number2", "n2", "Second number of the calculation", true, StartOptionValueType.Single, group.GetOptionByShortName("n2"));
            AssertionUtility.StartOption("operation", "o", "Operation to execute", true, StartOptionValueType.Single, group.GetOptionByShortName("o"));

            Assert.True(options.GrouplessStartOptions.Count() == 1);
            AssertionUtility.StartOption("verbose", "vb", "Enable verbose output", false, StartOptionValueType.Switch, options.GrouplessStartOptions.Single());
        }

        [Fact]
        public void TestMultipleConstructors()
        {
            ApplicationStartOptions options = this.GetDefaultReflectionHelper().GetStartOptions(typeof(MultipleConstructorsCommand));

            Assert.True(options.GrouplessStartOptions.Count() == 1);
            Assert.True(options.StartOptionGroups.Count() == 3);

            StartOptionGroup list = options.StartOptionGroups.Single(_grp => _grp.LongName.Equals("list"));
            AssertionUtility.StartOptionGroup("list", "l", "Lists strings stored in the list", list);
            AssertionUtility.StartOption("inLine", "i", "Displays the items in a line instead of seperate lines", false, StartOptionValueType.Switch, list.GetOptionByShortName("i"));

            StartOptionGroup add = options.StartOptionGroups.Single(_grp => _grp.LongName.Equals("add"));
            AssertionUtility.StartOptionGroup("add", "a", "Adds a new value to the stored list", add);
            AssertionUtility.StartOption("value", "v", "Value to be added", true, StartOptionValueType.Single, add.GetOptionByShortName("v"));

            StartOptionGroup remove = options.StartOptionGroups.Single(_grp => _grp.LongName.Equals("remove"));
            AssertionUtility.StartOptionGroup("remove", "r", "Removes a value from the stored list", remove);
            AssertionUtility.StartOption("value", "v", "Value to be added", true, StartOptionValueType.Single, remove.GetOptionByShortName("v"));
            AssertionUtility.StartOption("ignoreErrors", "i", "Ignores the error if the value is not present in the list", false, StartOptionValueType.Switch, remove.GetOptionByShortName("i"));
        }

        [Fact]
        public void TestGrouplessOptionReference()
        {
            ApplicationStartOptions options = this.GetDefaultReflectionHelper().GetStartOptions(typeof(BasicMockCommand), typeof(GrouplessOptionReferenceCommand));

            Assert.True(options.GrouplessStartOptions.Count() == 1);
            Assert.True(options.StartOptionGroups.Count() == 2);

            AssertionUtility.StartOption("verbose", "vb", "Enable verbose output", false, StartOptionValueType.Switch, options.GrouplessStartOptions.Single());
        }

        [Fact]
        public void TestIsValueMandatory()
        {
            ApplicationStartOptions firstOptions = this.GetDefaultReflectionHelper().GetStartOptions(typeof(GroupValueCommand));
            ApplicationStartOptions secondOptions = this.GetDefaultReflectionHelper().GetStartOptions(typeof(SearchCommand));
            ApplicationStartOptions thirdOptions = this.GetDefaultReflectionHelper().GetStartOptions(typeof(BasicMockCommand));

            Assert.True(firstOptions.StartOptionGroups.Single().IsValueMandatory);
            Assert.False(secondOptions.StartOptionGroups.Single().IsValueMandatory);
            Assert.False(thirdOptions.StartOptionGroups.Single().IsValueMandatory);
        }

        [Fact]
        public void TestConstructorNameConflicts()
        {
            ReflectionHelper helper = this.GetDefaultReflectionHelper();

            Assert.Throws<NameConflictException>(() => helper.GetStartOptions(typeof(GroupLongNameConflictCommand)));
            Assert.Throws<NameConflictException>(() => helper.GetStartOptions(typeof(HelpOptionNameConflictCommand)));
            Assert.Throws<NameConflictException>(() => helper.GetStartOptions(typeof(GroupShortNameConflictCommand)));
            Assert.Throws<NameConflictException>(() => helper.GetStartOptions(typeof(OptionLongNameConflictCommand)));
            Assert.Throws<NameConflictException>(() => helper.GetStartOptions(typeof(OptionShortNameConflictCommand)));
            Assert.Throws<NameConflictException>(() => helper.GetStartOptions(typeof(GroupOptionLongNameConflictCommand)));
            Assert.Throws<NameConflictException>(() => helper.GetStartOptions(typeof(GroupOptionShortNameConflictCommand)));
            Assert.Throws<NameConflictException>(() => helper.GetStartOptions(typeof(GroupGrouplessOptionLongNameConflictCommand)));
            Assert.Throws<NameConflictException>(() => helper.GetStartOptions(typeof(GroupGrouplessOptionShortNameConflictCommand)));
            Assert.Throws<NameConflictException>(() => helper.GetStartOptions(typeof(OptionGrouplessOptionLongNameConflictCommand)));
            Assert.Throws<NameConflictException>(() => helper.GetStartOptions(typeof(OptionGrouplessOptionShortNameConflictCommand)));
        }

        [Fact]
        public void TestMiscellaneousExceptions()
        {
            ReflectionHelper helper = this.GetDefaultReflectionHelper();

            Assert.Throws<InvalidOperationException>(() => helper.GetStartOptions(typeof(UnrelatedConstructorParameterCommand)));
            Assert.Throws<NotSupportedException>(() => helper.GetStartOptions(typeof(GenericClassCommand<object>)));
            Assert.Throws<InvalidOperationException>(() => helper.GetStartOptions(typeof(AbstractClassCommand)));
            Assert.Throws<InvalidOperationException>(() => helper.GetStartOptions(typeof(NotACommandCommand)));
            Assert.Throws<MissingStartOptionReferenceException>(() => helper.GetStartOptions(typeof(GrouplessOptionReferenceCommand)));
            Assert.Throws<AmbiguousMatchException>(() => helper.GetStartOptions(typeof(DuplicateAttributeCommand)));
        }

        private ReflectionHelper GetDefaultReflectionHelper()
        {
            IEnumerable<HelpOption> helpOptions = new HelpOption[] { new HelpOption("help", false), new HelpOption("h", true) };
            return new ReflectionHelper(helpOptions, new StartOptionParserSettings(), null);
        }
    }
}
