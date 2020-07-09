using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Interfaces;

namespace LunarDoggo.StartOptions
{
    public class StartOption : IClonable<StartOption>
    {
        private readonly IStartOptionValueParser valueParser;
        private object value;

        internal StartOption(string longName, string shortName, string description, IStartOptionValueParser valueParser, StartOptionValueType valueType, bool required)
        {
            this.valueParser = valueParser;
            this.Description = description;
            this.IsRequired = required;
            this.ValueType = valueType;
            this.ShortName = shortName;
            this.LongName = longName;
        }

        public StartOptionValueType ValueType { get; }
        public string Description { get; }
        public string ShortName { get; }
        public string LongName { get; }
        public bool IsRequired { get; }

        public bool HasValue { get { return this.value != null; } }

        public StartOption Clone()
        {
            return new StartOption(this.LongName,
                                   this.ShortName,
                                   this.Description,
                                   this.valueParser,
                                   this.ValueType,
                                   this.IsRequired
                                );
        }

        public T GetValue<T>()
        {
            if (this.value != null)
            {
                return (T)this.value;
            }
            return default;
        }

        internal void ParseSingleValue(string value)
        {
            if (this.valueParser != null)
            {
                this.value = this.valueParser.ParseSingle(value);
            }
            else
            {
                this.value = value;
            }
        }

        internal void ParseMultipleValues(string[] values)
        {
            if (this.valueParser != null)
            {
                this.value = this.valueParser.ParseMultiple(values);
            }
            else
            {
                this.value = values;
            }
        }
    }

    public enum StartOptionValueType
    {
        Multiple,
        Single,
        Switch
    }
}
