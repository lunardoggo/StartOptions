using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Interfaces;
using LunarDoggo.StartOptions.Parsing;

namespace LunarDoggo.StartOptions
{
    public class HelpOption : IClonable<HelpOption>
    {
        public HelpOption(string name, bool isShortName)
        {
            this.IsShortName = isShortName;
            this.Name = name;

            this.CheckNameValidity();
        }

        public bool IsShortName { get; }
        public string Name { get; }

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
