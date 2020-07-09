using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Parsing;

namespace LunarDoggo.StartOptions.Building
{
    public abstract class AbstractOptionBuilder<T>
    {
        protected readonly string shortName;
        protected readonly string longName;

        public AbstractOptionBuilder(string longName, string shortName)
        {
            this.ValidateName(shortName);
            this.ValidateName(longName);

            this.shortName = shortName;
            this.longName = longName;
        }

        public abstract T Build();

        protected virtual void ValidateName(string name)
        {
            if(!StartOptionParser.ValidOptionNameRegex.IsMatch(name))
            {
                throw new InvalidNameException($"Invalid StartOption name \"{name}\", names must only contain letters, numbers, \"_\" and \"-\" and must start with a letter or number.");
            }
        }
    }
}
