using LunarDoggo.StartOptions.Parsing.Arguments;
using LunarDoggo.StartOptions.Exceptions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System;

[assembly: InternalsVisibleTo("StartOptions.Tests")]
namespace LunarDoggo.StartOptions.Parsing
{
    public class StartOptionParser
    {
        public static readonly ImmutableArray<HelpOption> DefaultHelpOptions;
        internal static readonly Regex ValidHelpNameRegex = new Regex(@"^[a-zA-Z\?][a-zA-Z]*$");
        internal static readonly Regex ValidOptionNameRegex = new Regex(@"^[\w\d][\-\w\d_]*$");

        static StartOptionParser()
        {
            HelpOption[] helpOptions = new HelpOption[] { new HelpOption("help", false), new HelpOption("h", true), new HelpOption("?", true) };
            StartOptionParser.DefaultHelpOptions = helpOptions.ToImmutableArray();
        }

        private readonly IEnumerable<StartOptionGroup> optionGroups;
        private readonly IEnumerable<StartOption> grouplessOptions;
        private readonly StartOptionParserValidator validator;
        private readonly StartOptionParserSettings settings;
        private readonly HelpOption[] helpOptions;

        /// <summary>
        /// Creates a new instance of a <see cref="StartOptionParser"/> with the provided parameters
        /// </summary>
        /// <exception cref="NameConflictException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public StartOptionParser(StartOptionParserSettings settings, IEnumerable<StartOptionGroup> groups,
                                 IEnumerable<StartOption> grouplessOptions, IEnumerable<HelpOption> helpOptions,
                                 bool validateNameConflics = true)
        {
            this.settings = settings.Clone();

            this.grouplessOptions = this.GetCopiedOptions(grouplessOptions);
            this.helpOptions = this.GetCopiedHelpOptions(helpOptions);
            this.optionGroups = this.GetCopiedOptionGroups(groups);
            this.validator = this.GetValidator();

            if (validateNameConflics)
            {
                this.validator.CheckNameConflicts();
            }
        }

        private StartOptionParserValidator GetValidator()
        {
            return new StartOptionParserValidator(this.settings, this.optionGroups, this.grouplessOptions, this.helpOptions);
        }

        private StartOptionGroup[] GetCopiedOptionGroups(IEnumerable<StartOptionGroup> groups)
        {
            if (groups != null)
            {
                return groups.Select(_group => _group.Clone()).ToArray();
            }
            return new StartOptionGroup[0];
        }

        private StartOption[] GetCopiedOptions(IEnumerable<StartOption> options)
        {
            if (options != null)
            {
                return options.Select(_option => _option.Clone()).ToArray();
            }
            return new StartOption[0];
        }

        private HelpOption[] GetCopiedHelpOptions(IEnumerable<HelpOption> helpOptions)
        {
            return helpOptions.Select(_help => _help.Clone()).ToArray();
        }

        /// <summary>
        /// Creates a new instance of a <see cref="StartOptionParser"/> with the provided parameters
        /// </summary>
        /// <exception cref="NameConflictException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public StartOptionParser(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions, IEnumerable<HelpOption> helpOptions)
            : this(new StartOptionParserSettings(), groups, grouplessOptions, helpOptions)
        { }

        /// <summary>
        /// Creates a new instance of a <see cref="StartOptionParser"/> with the provided parameters
        /// </summary>
        /// <exception cref="NameConflictException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public StartOptionParser(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
            : this(new StartOptionParserSettings(), groups, grouplessOptions, StartOptionParser.DefaultHelpOptions)
        { }

        /// <summary>
        /// Parses the provided command line arguments and turns them into a <see cref="StartOptionGroup"/> and <see cref="StartOption"/>s.
        /// The output only contains parameters that were contained in the command line arguments
        /// </summary>
        /// <exception cref="OptionRequirementException"></exception>
        /// <exception cref="UnknownOptionNameException"></exception>
        public ParsedStartOptions Parse(string[] args)
        {
            List<ParsedStartArgument> parsedOptions = this.GetParsedStartOptions(args).ToList();
            bool wasHelpRequested = this.GetWasHelpRequested(ref parsedOptions);

            StartOptionGroup parsedGroup = this.GetStartOptionGroup(ref parsedOptions);
            IEnumerable<StartOption> parsedGrouplessOptions = this.GetStartOptions(this.grouplessOptions, ref parsedOptions);

            this.validator.CheckUnknownStartOptions(parsedOptions);
            ParsedStartOptions output = new ParsedStartOptions(parsedGroup, parsedGrouplessOptions, wasHelpRequested);
            if (!wasHelpRequested)
            {
                this.validator.CheckOptionRequirements(output);
            }
            return output;
        }

        private IEnumerable<ParsedStartArgument> GetParsedStartOptions(string[] args)
        {
            return ArgumentParserFactory.Instance.GetParser(this.settings).Parse(args);
        }

        private bool GetWasHelpRequested(ref List<ParsedStartArgument> parsedOptions)
        {
            bool helpRequested = false;
            for (int i = parsedOptions.Count - 1; i >= 0; i--)
            {
                if (this.IsHelpOption(parsedOptions[i]))
                {
                    helpRequested = true;
                    parsedOptions.RemoveAt(i);
                }
            }
            return helpRequested;
        }

        private bool IsHelpOption(ParsedStartArgument parsedArgument)
        {
            return this.helpOptions.Any(_option => _option.Name.Equals(parsedArgument.Name) && _option.IsShortName == parsedArgument.IsShortName);
        }

        private StartOptionGroup GetStartOptionGroup(ref List<ParsedStartArgument> parsedOptions)
        {
            Dictionary<StartOptionGroup, ParsedStartArgument> groups = new Dictionary<StartOptionGroup, ParsedStartArgument>();
            for (int i = parsedOptions.Count - 1; i >= 0; i--)
            {
                ParsedStartArgument current = parsedOptions[i];
                StartOptionGroup found = this.FindStartOptionGroupForParsedStartOption(current);
                if (found != null)
                {
                    groups.Add(found, current);
                    parsedOptions.RemoveAt(i);
                }
            }

            if (groups.Any())
            {
                KeyValuePair<StartOptionGroup, ParsedStartArgument> pair = groups.Single();
                StartOptionGroup group = pair.Key;
                IEnumerable<StartOption> options = this.GetStartOptions(group.Options, ref parsedOptions);
                StartOptionGroup output = new StartOptionGroup(group.LongName, group.ShortName, group.Description, group.ValueParser, group.ValueType, options);
                this.ParseOptionValue(output, pair.Value.Value);
                return output;
            }
            return null;
        }

        private StartOptionGroup FindStartOptionGroupForParsedStartOption(ParsedStartArgument option)
        {
            if (option.IsShortName)
            {
                return this.optionGroups.SingleOrDefault(_group => _group.ShortName.Equals(option.Name));
            }
            else
            {
                return this.optionGroups.SingleOrDefault(_group => _group.LongName.Equals(option.Name));
            }
        }

        private IEnumerable<StartOption> GetStartOptions(IEnumerable<StartOption> allOptions, ref List<ParsedStartArgument> parsedOptions)
        {
            List<StartOption> output = new List<StartOption>();
            for (int i = parsedOptions.Count - 1; i >= 0; i--)
            {
                ParsedStartArgument parsed = parsedOptions[i];
                StartOption option = this.FindStartOptionForParsedStartOption(allOptions, parsed);
                if (option != null)
                {
                    if (parsed.HasValue)
                    {
                        this.ParseOptionValue(option, parsed.Value);
                    }

                    parsedOptions.RemoveAt(i);
                    output.Add(option);
                }
            }
            return output;
        }

        private void ParseOptionValue(BaseStartOption option, string value)
        {
            //TODO: Throw Exception if option has value type mismatch (i.e. Switch or is single but got multiple values)
            if (option.ValueType != StartOptionValueType.Switch)
            {
                if (value.Contains(this.settings.MultipleValueSeparator))
                {
                    option.ParseMultipleValues(value.Split(new char[] { this.settings.MultipleValueSeparator }, StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    option.ParseSingleValue(value);
                }
            }
        }

        private StartOption FindStartOptionForParsedStartOption(IEnumerable<StartOption> allOptions, ParsedStartArgument option)
        {
            if (option.IsShortName)
            {
                return allOptions.SingleOrDefault(_group => _group.ShortName.Equals(option.Name));
            }
            else
            {
                return allOptions.SingleOrDefault(_group => _group.LongName.Equals(option.Name));
            }
        }
    }
}
