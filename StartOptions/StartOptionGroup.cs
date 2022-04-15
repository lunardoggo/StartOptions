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

        public IEnumerable<StartOption> Options { get; }
        public string Description { get; }
        public string ShortName { get; }
        public string LongName { get; }

        public StartOption GetOptionByShortName(string shortName)
        {
            return this.Options.SingleOrDefault(_option => _option.ShortName.Equals(shortName));
        }

        public StartOption GetOptionByLongName(string longName)
        {
            return this.Options.SingleOrDefault(_option => _option.LongName.Equals(longName));
        }

        public StartOptionGroup Clone()
        {
            return new StartOptionGroup(this.LongName, this.ShortName, this.Description,
                                        this.Options.Select(_option => _option.Clone()));
        }
    }
}
