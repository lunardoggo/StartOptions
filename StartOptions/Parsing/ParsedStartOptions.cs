using System.Collections.Generic;

namespace LunarDoggo.StartOptions.Parsing
{
    public class ParsedStartOptions
    {
        public ParsedStartOptions(StartOptionGroup parsedGroup, IEnumerable<StartOption> grouplessOptions, bool helpRequested)
        {
            this.ParsedGrouplessOptions = grouplessOptions;
            this.WasHelpRequested = helpRequested;
            this.ParsedOptionGroup = parsedGroup;
        }

        public IEnumerable<StartOption> ParsedGrouplessOptions { get; }
        public StartOptionGroup ParsedOptionGroup { get; }
        public bool WasHelpRequested { get; }
    }
}
