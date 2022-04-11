using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions;
using System;

namespace StartOptions.Tests.Mocks.Commands
{
    public class NullValuesCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "g")]
        public NullValuesCommand([StartOption("string", "s", ValueType = StartOptionValueType.Single)]string strValue,
                                 [StartOption("int", "i", ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))]int intValue,
                                 [StartOption("byte", "by", ValueType = StartOptionValueType.Single, ParserType = typeof(ByteOptionValueParser))]byte byteValue,
                                 [StartOption("bool", "bo", ValueType = StartOptionValueType.Single, ParserType = typeof(BoolOptionValueParser))]bool boolValue,
                                 [StartOption("switch", "sw")]bool switchValue)
        {
            this.SwitchValue = switchValue;
            this.StringValue = strValue;
            this.ByteValue = byteValue;
            this.BoolValue = boolValue;
            this.IntValue = intValue;
        }

        public string StringValue { get; }
        public bool SwitchValue { get; }
        public byte ByteValue { get; }
        public bool BoolValue { get; }
        public int IntValue { get; }

        public void Execute()
        {}
    }
}
