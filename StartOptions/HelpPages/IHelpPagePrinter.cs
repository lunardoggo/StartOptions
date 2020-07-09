using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;

namespace LunarDoggo.StartOptions.HelpPages
{
    public interface IHelpPagePrinter
    {
        void Print(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions);
    }
}
