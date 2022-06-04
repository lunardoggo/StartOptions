using LunarDoggo.StartOptions.Parsing.Values;
using System;

namespace LunarDoggo.StartOptions.Building
{
    public class StartOptionBuilder : AbstractOptionBuilder<StartOption>
    {
        private IStartOptionValueParser valueParser;
        private StartOptionValueType valueType;
        private string description;
        private bool mandatory;

        /// <summary>
        /// Creates a new builder for a <see cref="StartOption"/> with the provided long and short name
        /// </summary>
        public StartOptionBuilder(string longName, string shortName) : base(longName, shortName)
        {
            this.valueType = StartOptionValueType.Switch;
        }

        /// <summary>
        /// Sets the <see cref="IStartOptionValueParser"/> to be used by the <see cref="StartOption"/>
        /// </summary>
        public StartOptionBuilder SetValueParser(IStartOptionValueParser valueParser)
        {
            this.valueParser = valueParser;
            return this;
        }

        /// <summary>
        /// Sets the value type the <see cref="StartOption"/> should use (Switch, Single, Multiple)
        /// </summary>
        public StartOptionBuilder SetValueType(StartOptionValueType valueType)
        {
            this.valueType = valueType;
            return this;
        }

        /// <summary>
        /// Sets the description displayed on the help page for the <see cref="StartOption"/>
        /// </summary>
        public StartOptionBuilder SetDescription(string description)
        {
            this.description = description;
            return this;
        }

        /// <summary>
        /// Sets whether the <see cref="StartOption"/> has to be set. If this <see cref="StartOption"/> is groupless,
        /// it will always have to be set by users, if you set it to true. If it is part of a <see cref="StartOptionGroup"/>
        /// it is only required if the group is selected by the user
        /// </summary>
        [Obsolete("SetRequired(bool required) will be removed in a future version, use SetMandatory(bool mandatory) instead")]
        public StartOptionBuilder SetRequired(bool required = true)
        {
            return this.SetMandatory(required);
        }

        /// <summary>
        /// Sets whether the <see cref="StartOption"/> has to be set. If this <see cref="StartOption"/> is groupless,
        /// it will always have to be set by users, if you set it to true. If it is part of a <see cref="StartOptionGroup"/>
        /// it is only required if the group is selected by the user
        /// </summary>
        public StartOptionBuilder SetMandatory(bool mandatory = true)
        {
            this.mandatory = mandatory;
            return this;
        }

        /// <summary>
        /// Returns a new instance of <see cref="StartOption"/> with all stored values
        /// </summary>
        public override StartOption Build()
        {
            return new StartOption(this.longName, this.shortName, this.description, this.valueParser, this.valueType, this.mandatory);
        }
    }
}
