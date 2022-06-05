using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions;
using System;

namespace StartOptions.Tests.Mocks.Commands
{
    public class BasicMockCommand : IApplicationCommand
    {
        private readonly double firstNumber, secondNumber;
        private readonly CalculationOperation operation;
        private readonly bool verbose;

        [StartOptionGroup("calculate", "c", Description = "Executes a calculation")]
        public BasicMockCommand([StartOption("number1", "n1", Description = "First number of the calculation", IsMandatory = true, ValueType = StartOptionValueType.Single, ParserType = typeof(DoubleOptionValueParser))]double firstNumber,
                                [StartOption("number2", "n2", Description = "Second number of the calculation", IsMandatory = true, ValueType = StartOptionValueType.Single, ParserType = typeof(DoubleOptionValueParser))]double secondNumber,
                                [StartOption("operation", "o", Description = "Operation to execute", IsMandatory = true, ValueType = StartOptionValueType.Single, ParserType = typeof(CalculationOperationValueParser))] CalculationOperation operation,
                                [GrouplessStartOption("verbose", "vb", Description = "Enable verbose output")]bool verbose)
        {
            this.secondNumber = secondNumber;
            this.firstNumber = firstNumber;
            this.operation = operation;
            this.verbose = verbose;
        }

        public void Execute()
        {
            switch(this.operation)
            {
                case CalculationOperation.Subtract:
                    Console.WriteLine("{0} - {1} = {2}; verbose output: {3}", this.firstNumber, this.secondNumber, this.firstNumber - this.secondNumber, this.verbose);
                    break;
                case CalculationOperation.Multiply:
                    Console.WriteLine("{0} * {1} = {2}; verbose output: {3}", this.firstNumber, this.secondNumber, this.firstNumber * this.secondNumber, this.verbose);
                    break;
                case CalculationOperation.Divide:
                    Console.WriteLine("{0} / {1} = {2}; verbose output: {3}", this.firstNumber, this.secondNumber, this.firstNumber / this.secondNumber, this.verbose);
                    break;
                case CalculationOperation.Add:
                    Console.WriteLine("{0} + {1} = {2}; verbose output: {3}", this.firstNumber, this.secondNumber, this.firstNumber + this.secondNumber, this.verbose);
                    break;
            }
        }
    }

    public enum CalculationOperation
    {
        Subtract,
        Multiply,
        Divide,
        Add
    }
}
