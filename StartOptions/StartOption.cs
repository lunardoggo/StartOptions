using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Interfaces;
using System;

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

        internal Type ParserType { get { return this.valueParser?.GetType(); } }

        /// <summary>
        /// Returns the <see cref="StartOptionValueType"/> of the <see cref="StartOption"/>
        /// </summary>
        public StartOptionValueType ValueType { get; }
        /// <summary>
        /// Returns the description of the <see cref="StartOption"/>
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Returns the short name of the <see cref="StartOption"/>
        /// </summary>
        public string ShortName { get; }
        /// <summary>
        /// Returns the long name of the <see cref="StartOption"/>
        /// </summary>
        public string LongName { get; }
        /// <summary>
        /// Returns whether the <see cref="StartOption"/> has to be set by the user. If the value is true and this <see cref="StartOption"/> is groupless,
        /// it will always have to be set by users. If it is part of a <see cref="StartOptionGroup"/>
        /// it is only required if the group is selected by the user
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// Returns whether a value was parsed from the command line arguments for the <see cref="StartOption"/>
        /// </summary>
        public bool HasValue { get { return this.value != null; } }

        /// <summary>
        /// Returns a copy of the <see cref="StartOption"/> containing the same parameters. Parsed values are not copied.
        /// </summary>
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
