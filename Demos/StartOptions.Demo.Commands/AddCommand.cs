using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions;
using System;

namespace StartOptions.Demo
{
    public class AddCommand : IApplicationCommand
    {
        private readonly int firstValue, secondValue;

        [StartOptionGroup("add", "a", Description = "Adds two integers together")]
        public AddCommand([StartOption("value-1", "1", Description = "First value", IsMandatory = true, ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))] int firstValue,
                          [StartOption("value-2", "2", Description = "Second value", IsMandatory = true, ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))] int secondValue)
        {
            this.secondValue = secondValue;
            this.firstValue = firstValue;
        }

        public void Execute()
        {
            Console.WriteLine("Enable verbose output: " + Program.Verbose);
            Console.WriteLine("{0} + {1} = {2}", this.firstValue, this.secondValue, this.firstValue + this.secondValue);
        }
    }
}
