using LunarDoggo.StartOptions.Parsing.Values;
using System;

namespace LunarDoggo.StartOptions
{
    public abstract class BaseStartOption
    {
        protected readonly IStartOptionValueParser valueParser;
        protected object value;

        protected BaseStartOption(string longName, string shortName, string description, StartOptionValueType valueType, IStartOptionValueParser valueParser)
        {
            this.Description = description;
            this.valueParser = valueParser;
            this.ShortName = shortName;
            this.ValueType = valueType;
            this.LongName = longName;
        }

        /// <summary>
        /// Returns the description of the <see cref="StartOption"/> or <see cref="StartOptionGroup"/>
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Returns the short name of the <see cref="StartOption"/> or <see cref="StartOptionGroup"/>
        /// </summary>
        public string ShortName { get; }
        /// <summary>
        /// Returns the long name of the <see cref="StartOption"/> or <see cref="StartOptionGroup"/>
        /// </summary>
        public string LongName { get; }
        /// <summary>
        /// Returns the <see cref="StartOptionValueType"/> of the <see cref="StartOption"/>
        /// </summary>
        public StartOptionValueType ValueType { get; }


        /// <summary>
        /// Returns whether a value was parsed from the command line arguments for the <see cref="StartOption"/>
        /// </summary>
        public bool HasValue { get { return this.value != null; } }

        /// <summary>
        /// Returns the value parsed by the <see cref="StartOption"/> casted to <see cref="Type"/> <see cref="{T}"/>
        /// </summary>
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
                this.value = this.valueParser.ParseValue(value);
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
                this.value = this.valueParser.ParseValues(values);
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
