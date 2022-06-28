using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Exceptions;
using System.Collections.Generic;
using System;

namespace LunarDoggo.StartOptions.Building
{
    public class StartOptionGroupBuilder : AbstractOptionBuilder<StartOptionGroup>
    {
        private readonly List<StartOption> options = new List<StartOption>();
        private IStartOptionValueParser parser = StartOption.DefaultValueParser;
        private StartOptionValueType valueType = StartOption.DefaultValueType;
        private string description = StartOption.DefaultDescription;
        private bool isValueMandatory;

        /// <summary>
        /// Creates a new builder for a <see cref="StartOptionGroup"/> with the provided long and short name
        /// </summary>
        public StartOptionGroupBuilder(string longName, string shortName) : base(longName, shortName)
        { }

        /// <summary>
        /// Sets the description of the <see cref="StartOptionGroup"/> displayed on the help page
        /// </summary>
        public StartOptionGroupBuilder SetDescription(string description)
        {
            this.description = description;
            return this;
        }

        /// <summary>
        /// Sets the value type of the <see cref="StartOptionGroup"/>. Default is <see cref="StartOptionValueType.Switch"/> (i.e. no value)
        /// </summary>
        public StartOptionGroupBuilder SetValueType(StartOptionValueType valueType)
        {
            this.valueType = valueType;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IStartOptionValueParser"/> to be used to parser provided values. Make sure to
        /// also set <see cref="SetValueType(StartOptionValueType)"/> when using this method
        /// </summary>
        public StartOptionGroupBuilder SetValueParser(IStartOptionValueParser parser)
        {
            this.parser = parser;
            return this;
        }

        /// <summary>
        /// Gets or seths whether the <see cref="StartOptionGroup"/> must be provided with a value.
        /// Note: this property only takes effect, if (<see cref="ValueType"/> != <see cref="StartOptionValueType.Switch"/>)
        /// </summary>
        public StartOptionGroupBuilder SetValueMandatory(bool mandatory = true)
        {
            this.isValueMandatory = mandatory;
            return this;
        }

        /// <summary>
        /// Adds a new <see cref="StartOption"/> to the <see cref="StartOptionGroup"/> with the provided long name, short name and builder
        /// </summary>
        public StartOptionGroupBuilder AddOption(string longName, string shortName, Action<StartOptionBuilder> buildAction)
        {
            StartOptionBuilder builder = new StartOptionBuilder(longName, shortName);
            buildAction?.Invoke(builder);
            return this.AddOption(builder.Build());
        }

        /// <summary>
        /// Adds the provided <see cref="StartOption"/> to the <see cref="StartOptionGroup"/>
        /// </summary>
        public StartOptionGroupBuilder AddOption(StartOption option)
        {
            this.CheckForNameDuplications(option);
            this.options.Add(option);
            return this;
        }

        /// <summary>
        /// Returns a new instance of <see cref="StartOptionGroup"/> with all stored values
        /// </summary>
        public override StartOptionGroup Build()
        {
            return new StartOptionGroup(this.longName, this.shortName, this.description, this.parser, this.valueType, this.options, this.isValueMandatory);
        }

        private void CheckForNameDuplications(StartOption newOption)
        {
            if (this.longName.Equals(newOption.LongName))
            {
                throw new NameConflictException($"Long option name \"{newOption.LongName}\" conflicts with the groups long name.");
            }
            else if (this.shortName.Equals(newOption.ShortName))
            {
                throw new NameConflictException($"Short option name \"{newOption.ShortName}\" conflicts with the groups short name.");
            }

            this.CheckOptionsNameConflicts(newOption);
        }

        private void CheckOptionsNameConflicts(StartOption newOption)
        {
            foreach (StartOption option in this.options)
            {
                if (option.LongName.Equals(newOption.LongName))
                {
                    throw new NameConflictException($"Long option name \"{newOption.LongName}\" was already added to group \"{this.longName}\".");
                }
                else if (option.ShortName.Equals(newOption.ShortName))
                {
                    throw new NameConflictException($"Short option name \"{newOption.ShortName}\" was already added to group \"{this.longName}\".");
                }
            }
        }
    }
}
