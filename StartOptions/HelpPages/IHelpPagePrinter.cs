using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;

namespace LunarDoggo.StartOptions.HelpPages
{
    public interface IHelpPagePrinter
    {
        /// <summary>
        /// Prints the help page according to the provided parameters
        /// </summary>
        void Print(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions);
    }
}
