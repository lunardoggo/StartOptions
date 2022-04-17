using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Parsing;

namespace LunarDoggo.StartOptions
{
    public class HelpOption : IClonable<HelpOption>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HelpOption"/> with the provided parameters
        /// </summary>
        /// <exception cref="InvalidNameException"></exception>
        public HelpOption(string name, bool isShortName)
        {
            this.IsShortName = isShortName;
            this.Name = name;

            this.CheckNameValidity();
        }

        /// <summary>
        /// Gets whether the <see cref="HelpOption"/> name will use <see cref="StartOptionParserSettings.ShortOptionNamePrefix"/>
        /// or <see cref="StartOptionParserSettings.LongOptionNamePrefix"/>
        /// </summary>
        public bool IsShortName { get; }
        /// <summary>
        /// Gets the name of the <see cref="HelpOption"/>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns a copied instance of this <see cref="HelpOption"/> with the same values
        /// </summary>
        public HelpOption Clone()
        {
            return new HelpOption(this.Name, this.IsShortName);
        }

        private void CheckNameValidity()
        {
            if(!StartOptionParser.ValidHelpNameRegex.IsMatch(this.Name))
            {
                throw new InvalidNameException($"Help option name \"{this.Name}\" is invalid, help options must start with a letter or \"?\" and only contain letters afterwards.");
            }
        }
    }
}
