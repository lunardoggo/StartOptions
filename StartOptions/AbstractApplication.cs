using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using System.Linq;

namespace LunarDoggo.StartOptions
{
    public abstract class AbstractApplication
    {
        internal readonly ApplicationStartOptions options;

        public AbstractApplication()
        {
            this.options = this.GetApplicationStartOptions();
        }

        /// <summary>
        /// Runs the <see cref="AbstractApplication"/>. If the user used a <see cref="HelpOption"/>, the help page will be displayed,
        /// otherwise the application will be executed
        /// </summary>
        /// <param name="args">Command line arguments provided by the user</param>
        public virtual void Run(string[] args)
        {
            StartOptionParser parser = new StartOptionParser(this.options.StartOptionParserSettings, this.options.StartOptionGroups,
                                                             this.options.GrouplessStartOptions, this.options.HelpOptions);
            ParsedStartOptions parsed = parser.Parse(args);

            if (parsed.WasHelpRequested)
            {
                this.PrintHelpPage(parsed);
            }
            else
            {
                this.Run(parsed.ParsedOptionGroup, parsed.ParsedGrouplessOptions);
            }
        }

        /// <summary>
        /// Prints the help page
        /// </summary>
        protected virtual void PrintHelpPage(ParsedStartOptions parsed)
        {
            IEnumerable<StartOption> options = parsed.ParsedOptionGroup != null
                    ? parsed.ParsedGrouplessOptions : this.options.GrouplessStartOptions;
            IEnumerable<StartOptionGroup> groups = null;
            if (parsed.ParsedOptionGroup == null)
            {
                groups = parsed.ParsedGrouplessOptions?.Count() > 0 ? new StartOptionGroup[0] : this.options.StartOptionGroups;
            }
            else
            {
                groups = new[] { this.options.StartOptionGroups.Single(_grp => _grp.LongName.Equals(parsed.ParsedOptionGroup.LongName)) };
            }
            
            this.PrintHelpPage(this.options.StartOptionParserSettings, this.options.HelpOptions, groups, options);
        }

        /// <summary>
        /// Prints the help page with the provided parameters
        /// </summary>
        protected abstract void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions);
        /// <summary>
        /// Executes the application. Here you have to provide the seperate execution paths for each <see cref="StartOptionGroup"/>
        /// </summary>
        protected abstract void Run(StartOptionGroup selectedGroup, IEnumerable<StartOption> selectedGrouplessOptions);
        /// <summary>
        /// Returns the <see cref="ApplicationStartOptions"/> your application should use, e.g. all
        /// <see cref="StartOptionGroup"/>s and grupless <see cref="StartOption"/>s and <see cref="StartOptionParserSettings"/>
        /// </summary>
        protected abstract ApplicationStartOptions GetApplicationStartOptions();
    }

    public class ApplicationStartOptions : IClonable<ApplicationStartOptions>
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

        /// <summary>
        /// Returns the <see cref="Parsing.StartOptionParserSettings"/> the application should use
        /// </summary>
        public StartOptionParserSettings StartOptionParserSettings { get; }
        /// <summary>
        /// Returns all <see cref="StartOptionGroup"/>s the application supports
        /// </summary>
        public IEnumerable<StartOptionGroup> StartOptionGroups { get; }
        /// <summary>
        /// Returns all groupless <see cref="StartOption"/>s the application supports
        /// </summary>
        public IEnumerable<StartOption> GrouplessStartOptions { get; }
        /// <summary>
        /// Returns all <see cref="HelpOption"/>s the application supports
        /// </summary>
        public IEnumerable<HelpOption> HelpOptions { get; }

        public ApplicationStartOptions Clone()
        {
            IEnumerable<StartOption> grouplessOptions = this.GrouplessStartOptions?.ToArray() ?? new StartOption[0];
            IEnumerable<StartOptionGroup> groups = this.StartOptionGroups?.ToArray() ?? new StartOptionGroup[0];
            IEnumerable<HelpOption> helpOptions = this.HelpOptions?.ToArray() ?? new HelpOption[0];

            return new ApplicationStartOptions(groups, grouplessOptions, helpOptions, this.StartOptionParserSettings.Clone());
        }
    }
}
