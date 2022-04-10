using LunarDoggo.StartOptions.Exceptions;
using System.Collections.Generic;
using System;

namespace LunarDoggo.StartOptions.Building
{
    public class StartOptionGroupBuilder : AbstractOptionBuilder<StartOptionGroup>
    {
        private readonly List<StartOption> options = new List<StartOption>();
        private string description;

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
            return new StartOptionGroup(this.longName, this.shortName, this.description, this.options);
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
