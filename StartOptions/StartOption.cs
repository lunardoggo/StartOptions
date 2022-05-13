using LunarDoggo.StartOptions.Parsing.Values;
using System;

namespace LunarDoggo.StartOptions
{
    public class StartOption : BaseStartOption, IClonable<StartOption>
    {
        internal StartOption(string longName, string shortName, string description, IStartOptionValueParser valueParser, StartOptionValueType valueType, bool required)
            : base(longName, shortName, description, valueType, valueParser)
        {
            this.IsRequired = required;
        }

        internal Type ParserType { get { return this.valueParser?.GetType(); } }

        
        /// <summary>
        /// Returns whether the <see cref="StartOption"/> has to be set by the user. If the value is set to true and this <see cref="StartOption"/> is groupless,
        /// it will always have to be set by users. If it is part of a <see cref="StartOptionGroup"/>
        /// it is only required if the group is selected by the user
        /// </summary>
        public bool IsRequired { get; }

        

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
    }
}
