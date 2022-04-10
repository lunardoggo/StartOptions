using LunarDoggo.StartOptions.Interfaces;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;

namespace LunarDoggo.StartOptions
{
    public class StartOptionGroup : IClonable<StartOptionGroup>
    {
        public StartOptionGroup(string longName, string shortName, string description, IEnumerable<StartOption> options)
        {
            this.Options = (options ?? new StartOption[0]).ToImmutableList();
            this.Description = description;
            this.ShortName = shortName;
            this.LongName = longName;
        }

        /// <summary>
        /// Returns all <see cref="StartOption"/>s contained in the <see cref="StartOptionGroup"/>
        /// </summary>
        public IEnumerable<StartOption> Options { get; }
        /// <summary>
        /// Returns the description of the <see cref="StartOptionGroup"/>
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Returns the short name of the <see cref="StartOptionGroup"/>
        /// </summary>
        public string ShortName { get; }
        /// <summary>
        /// Returns the long name of the <see cref="StartOptionGroup"/>
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Returns the <see cref="StartOption"/> contained in the <see cref="StartOptionGroup"/> with the provided short name
        /// </summary>
        public StartOption GetOptionByShortName(string shortName)
        {
            return this.Options.SingleOrDefault(_option => _option.ShortName.Equals(shortName));
        }

        /// <summary>
        /// Returns the <see cref="StartOption"/> contained in the <see cref="StartOptionGroup"/> with the provided long name
        /// </summary>
        public StartOption GetOptionByLongName(string longName)
        {
            return this.Options.SingleOrDefault(_option => _option.LongName.Equals(longName));
        }

        /// <summary>
        /// Returns a copy of the <see cref="StartOptionGroup"/> containing the same parameters
        /// </summary>
        public StartOptionGroup Clone()
        {
            return new StartOptionGroup(this.LongName, this.ShortName, this.Description,
                                        this.Options.Select(_option => _option.Clone()));
        }
    }
}
