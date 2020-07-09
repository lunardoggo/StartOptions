using LunarDoggo.StartOptions.Exceptions;
using System.Collections.Generic;
using System;

namespace LunarDoggo.StartOptions.Building
{
    public class StartOptionGroupBuilder : AbstractOptionBuilder<StartOptionGroup>
    {
        private readonly List<StartOption> options = new List<StartOption>();
        private string description;

        public StartOptionGroupBuilder(string longName, string shortName) : base(longName, shortName)
        { }

        public StartOptionGroupBuilder SetDescription(string description)
        {
            this.description = description;
            return this;
        }

        public StartOptionGroupBuilder AddOption(string longName, string shortName, Action<StartOptionBuilder> buildAction)
        {
            StartOptionBuilder builder = new StartOptionBuilder(longName, shortName);
            buildAction?.Invoke(builder);
            return this.AddOption(builder.Build());
        }

        public StartOptionGroupBuilder AddOption(StartOption option)
        {
            this.CheckForNameDuplications(option);
            this.options.Add(option);
            return this;
        }

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
