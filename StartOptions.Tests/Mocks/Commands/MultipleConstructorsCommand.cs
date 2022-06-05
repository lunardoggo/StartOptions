using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System;

namespace StartOptions.Tests.Mocks.Commands
{
    public class MultipleConstructorsCommand : IApplicationCommand
    {
        private readonly Action action;

        [StartOptionGroup("list", "l", Description = "Lists strings stored in the list")]
        public MultipleConstructorsCommand([StartOption("inLine", "i", Description = "Displays the items in a line instead of seperate lines")] bool inLine,
                                           [GrouplessStartOption("verbose", "vb", Description = "Enable verbose output")] bool verbose)
        {
            this.action = () => throw new ListException();
        }

        [StartOptionGroup("add", "a", Description = "Adds a new value to the stored list")]
        public MultipleConstructorsCommand([StartOption("value", "v", Description = "Value to be added", IsMandatory = true, ValueType = StartOptionValueType.Single)] string value)
        {
            this.action = () => throw new AddException();
        }

        [StartOptionGroup("remove", "r", Description = "Removes a value from the stored list")]
        public MultipleConstructorsCommand([StartOption("value", "v", Description = "Value to be added", IsMandatory = true, ValueType = StartOptionValueType.Single)] string value,
                                           [StartOption("ignoreErrors", "i", Description = "Ignores the error if the value is not present in the list")] bool ignoreErrors)
        {
            this.action = () =>
            {
                throw new RemoveException();
            };
        }

        //This constructor will be ignored by the reflection helper
        public MultipleConstructorsCommand(object input)
        {
            this.action = () => Console.WriteLine("obj: {0}", input);
        }

        public void Execute()
        {
            this.action.Invoke();
        }
    }

    internal class ListException : Exception
    { }
    internal class AddException : Exception
    { }
    internal class RemoveException : Exception
    { }
}
