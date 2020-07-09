using LunarDoggo.StartOptions.Parsing.Values;

namespace LunarDoggo.StartOptions.Building
{
    public class StartOptionBuilder : AbstractOptionBuilder<StartOption>
    {
        private IStartOptionValueParser valueParser;
        private StartOptionValueType valueType;
        private string description;
        private bool required;

        public StartOptionBuilder(string longName, string shortName) : base(longName, shortName)
        {
            this.valueType = StartOptionValueType.Switch;
        }

        public StartOptionBuilder SetValueParser(IStartOptionValueParser valueParser)
        {
            this.valueParser = valueParser;
            return this;
        }

        public StartOptionBuilder SetValueType(StartOptionValueType valueType)
        {
            this.valueType = valueType;
            return this;
        }

        public StartOptionBuilder SetDescription(string description)
        {
            this.description = description;
            return this;
        }

        public StartOptionBuilder SetRequired(bool required = true)
        {
            this.required = required;
            return this;
        }

        public override StartOption Build()
        {
            return new StartOption(this.longName, this.shortName, this.description, this.valueParser, this.valueType, this.required);
        }
    }
}
