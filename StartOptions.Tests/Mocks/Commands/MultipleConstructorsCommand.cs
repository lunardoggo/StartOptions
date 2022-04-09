using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System;

namespace StartOptions.Tests.Mocks.Commands
{
    public class MultipleConstructorsCommand : IApplicationCommand
    {
        private readonly List<string> values = new List<string>() { "s1", "s2", "s3" };
        private readonly Action action;

        [StartOptionGroup("list", "l", Description = "Lists strings stored in the list")]
        public MultipleConstructorsCommand([StartOption("inLine", "i", Description = "Displays the items in a line instead of seperate lines")] bool inLine)
        {
            this.action = () => Console.WriteLine($"Items: {(inLine ? "" : "\n")}{String.Join(inLine ? "," : "\n", this.values)}");
        }

        [StartOptionGroup("add", "a", Description = "Adds a new value to the stored list")]
        public MultipleConstructorsCommand([StartOption("value", "v", Description = "Value to be added", Mandatory = true, ValueType = StartOptionValueType.Single)] string value)
        {
            this.action = () => this.values.Add(value);
        }

        [StartOptionGroup("remove", "r", Description = "Removes a value from the stored list")]
        public MultipleConstructorsCommand([StartOption("value", "v", Description = "Value to be added", Mandatory = true, ValueType = StartOptionValueType.Single)] string value,
                                           [StartOption("ignoreErrors", "i", Description = "Ignores the error if the value is not present in the list")] bool ignoreErrors)
        {
            this.action = () =>
            {
                if (!ignoreErrors || this.values.Contains(value))
                {
                    this.values.Remove(value);
                }
            };
        }

        public bool Execute()
        {
            this.action.Invoke();
            return true;
        }
    }
}
