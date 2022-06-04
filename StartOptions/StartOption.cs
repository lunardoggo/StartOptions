using LunarDoggo.StartOptions.Parsing.Values;
using System;

namespace LunarDoggo.StartOptions
{
    public class StartOption : BaseStartOption, IClonable<StartOption>
    {
        internal StartOption(string longName, string shortName, string description, IStartOptionValueParser valueParser, StartOptionValueType valueType, bool mandatory)
            : base(longName, shortName, description, valueType, valueParser)
        {
            this.IsMandatory = mandatory;
        }

        internal Type ParserType { get { return this.valueParser?.GetType(); } }

        
        /// <summary>
        /// Returns whether the <see cref="StartOption"/> has to be set by the user. If the value is set to true and this <see cref="StartOption"/> is groupless,
        /// it will always have to be set by users. If it is part of a <see cref="StartOptionGroup"/>
        /// it is only required if the group is selected by the user
        /// </summary>
        [Obsolete("IsRequired will be removed in a future version, use IsMandatory instead")]
        public bool IsRequired { get => this.IsMandatory; }

        /// <summary>
        /// Returns whether the <see cref="StartOption"/> has to be set by the user. If the value is set to true and this <see cref="StartOption"/> is groupless,
        /// it will always have to be set by users. If it is part of a <see cref="StartOptionGroup"/>
        /// it is only required if the group is selected by the user
        /// </summary>
        public bool IsMandatory { get; }
        

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
                                   this.IsMandatory
                                );
        }
    }
}
