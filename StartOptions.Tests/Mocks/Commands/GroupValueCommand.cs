using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions;
using System.Linq;
using System;

namespace StartOptions.Tests.Mocks.Commands
{
    public class GroupValueCommand : IApplicationCommand
    {
        [StartOptionGroup("sum", "s", ValueType = StartOptionValueType.Multiple, IsValueMandatory = true, ParserType = typeof(DoubleOptionValueParser))]
        public GroupValueCommand([StartOptionGroupValue]object[] values)
        {
            this.Values = values.Cast<double>().ToArray();
        }

        public double[] Values { get; }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
