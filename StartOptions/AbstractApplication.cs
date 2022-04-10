using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;

namespace LunarDoggo.StartOptions
{
    public abstract class AbstractApplication
    {
        internal readonly ApplicationStartOptions options;

        public AbstractApplication()
        {
            this.options = this.GetApplicationStartOptions();
        }

        public virtual void Run(string[] args)
        {
            StartOptionParser parser = new StartOptionParser(this.options.StartOptionParserSettings, this.options.StartOptionGroups,
                                                             this.options.GrouplessStartOptions, this.options.HelpOptions);
            ParsedStartOptions parsed = parser.Parse(args);

            if(parsed.WasHelpRequested)
            {
                this.PrintHelpPage(parsed);
            }
            else
            {
                this.Run(parsed.ParsedOptionGroup, parsed.ParsedGrouplessOptions);
            }
        }

        protected virtual void PrintHelpPage(ParsedStartOptions parsed)
        {
            IEnumerable<StartOptionGroup> groups = parsed.ParsedOptionGroup == null
                    ? this.options.StartOptionGroups : new[] { parsed.ParsedOptionGroup };
            IEnumerable<StartOption> options = parsed.ParsedGrouplessOptions ?? this.options.GrouplessStartOptions;

            this.PrintHelpPage(this.options.StartOptionParserSettings, this.options.HelpOptions, groups, options);
        }

        protected abstract void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions);
        protected abstract void Run(StartOptionGroup selectedGroup, IEnumerable<StartOption> selectedGrouplessOptions);

        protected abstract ApplicationStartOptions GetApplicationStartOptions();
    }

    public class ApplicationStartOptions
    {
        public ApplicationStartOptions(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions, IEnumerable<HelpOption> helpOptions, StartOptionParserSettings parserSettings)
        {
            this.StartOptionParserSettings = parserSettings;
            this.GrouplessStartOptions = grouplessOptions;
            this.StartOptionGroups = groups;
            this.HelpOptions = helpOptions;
        }

        public ApplicationStartOptions(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions, IEnumerable<HelpOption> helpOptions)
            : this(groups, grouplessOptions, helpOptions, new StartOptionParserSettings())
        { }

        public ApplicationStartOptions(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
            : this(groups, grouplessOptions, StartOptionParser.DefaultHelpOptions, new StartOptionParserSettings())
        { }

        public StartOptionParserSettings StartOptionParserSettings { get; }
        public IEnumerable<StartOptionGroup> StartOptionGroups { get; }
        public IEnumerable<StartOption> GrouplessStartOptions { get; }
        public IEnumerable<HelpOption> HelpOptions { get; }
    }
}
