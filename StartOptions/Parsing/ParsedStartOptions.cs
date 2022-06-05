using System.Collections.Generic;

namespace LunarDoggo.StartOptions.Parsing
{
    public class ParsedStartOptions
    {
        public ParsedStartOptions(ApplicationStartOptions applicationStartOptions, StartOptionGroup parsedGroup, IEnumerable<StartOption> grouplessOptions, bool helpRequested)
        {
            this.ApplicationStartOptions = applicationStartOptions;
            this.ParsedGrouplessOptions = grouplessOptions;
            this.WasHelpRequested = helpRequested;
            this.ParsedOptionGroup = parsedGroup;
        }

        public ApplicationStartOptions ApplicationStartOptions { get; }
        public IEnumerable<StartOption> ParsedGrouplessOptions { get; }
        public StartOptionGroup ParsedOptionGroup { get; }
        public bool WasHelpRequested { get; }
    }
}
