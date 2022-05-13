using LunarDoggo.StartOptions.Parsing.Values;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;

namespace LunarDoggo.StartOptions
{
    public class StartOptionGroup : BaseStartOption, IClonable<StartOptionGroup>
    {
        public StartOptionGroup(string longName, string shortName, string description, IStartOptionValueParser valueParser, StartOptionValueType valueType, IEnumerable<StartOption> options)
            : base(longName, shortName, description, valueType, valueParser)
        {
            this.Options = (options ?? new StartOption[0]).ToImmutableList();
        }

        internal IStartOptionValueParser ValueParser { get { return this.valueParser; } }

        /// <summary>
        /// Returns all <see cref="StartOption"/>s contained in the <see cref="StartOptionGroup"/>
        /// </summary>
        public IEnumerable<StartOption> Options { get; }

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
            return new StartOptionGroup(this.LongName, this.ShortName, this.Description, this.valueParser, this.ValueType,
                                        this.Options.Select(_option => _option.Clone()));
        }
    }
}
