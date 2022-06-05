﻿using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions;
using System.Linq;
using System;

namespace StartOptions.Demo
{
    public class SumCommand : IApplicationCommand
    {
        private readonly double[] values;
        private bool verbose;

        [StartOptionGroup("sum", "s", Description = "Outputs the sum of all provided values", ParserType = typeof(DoubleOptionValueParser), ValueType = StartOptionValueType.Multiple)]
        public SumCommand([StartOptionGroupValue]object[] values,
                          [GrouplessStartOptionReference("verbose")]bool verbose)
        {
            this.values = values.Cast<double>().ToArray();
            this.verbose = verbose;
        }

        public void Execute()
        {
            Console.WriteLine("Enable verbose output: " + this.verbose);
            if (this.values?.Length > 1)
            {
                Console.WriteLine($"{String.Join(" + ", this.values)} = {this.values.Sum()}");
            }
            else
            {
                Console.WriteLine("Please provide at least two values to be summed up");
            }
        }
    }
}