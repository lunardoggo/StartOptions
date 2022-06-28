using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace LunarDoggo.StartOptions.HelpPages
{
    public class ConsoleHelpPrinter : IHelpPagePrinter
    {
        protected readonly ConsoleHelpPrinterSettings settings;

        public ConsoleHelpPrinter(ConsoleHelpPrinterSettings settings)
        {
            this.settings = settings;
        }

        public ConsoleHelpPrinter(string indentation, string applicationDescription, bool printValueTypes)
            : this(new ConsoleHelpPrinterSettings() { AppDescription = applicationDescription, Indentation = indentation, PrintValueTypes = printValueTypes })
        { }
        public ConsoleHelpPrinter(string indentation, string applicationDescription) : this(indentation, applicationDescription, false)
        { }

        public ConsoleHelpPrinter(string indentation) : this(indentation, null, false)
        { }

        [Obsolete("This constructor will be removed in a future update, use: new ConsoleHelpPrinter(string indentation, string applicationDescription)")]
        public ConsoleHelpPrinter(char indentationChar, string applicationDescription) : this(indentationChar.ToString(), applicationDescription, false)
        { }


        [Obsolete("This constructor will be removed in a future update, use: new ConsoleHelpPrinter(string indentation)")]
        public ConsoleHelpPrinter(char indentationChar) : this(indentationChar.ToString(), null, false)
        { }

        /// <summary>
        /// Prints the help page to the console according to the provided parameters
        /// </summary>
        public virtual void Print(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
        {
            StringBuilder builder = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(this.settings.AppDescription))
            {
                builder.AppendLine(this.settings.AppDescription);
                builder.AppendLine();
            }

            this.AppendHelpOptions(builder, settings, helpOptions);
            this.AppendGroups(builder, settings, groups);
            this.AppendOptions(builder, settings, grouplessOptions, 0);

            Console.WriteLine(builder.ToString());
        }

        protected void AppendHelpOptions(StringBuilder builder, StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions)
        {
            builder.Append("Available help options: ");
            builder.AppendLine(String.Join(",", helpOptions.Select(_option => this.GetHelpOptionName(settings, _option))));
            builder.AppendLine();
        }

        protected string GetHelpOptionName(StartOptionParserSettings settings, HelpOption option)
        {
            string prefix;
            if (option.IsShortName)
            {
                prefix = settings.ShortOptionNamePrefix;
            }
            else
            {
                prefix = settings.LongOptionNamePrefix;
            }
            return prefix + option.Name;
        }

        protected void AppendGroups(StringBuilder builder, StartOptionParserSettings settings, IEnumerable<StartOptionGroup> groups)
        {
            foreach (StartOptionGroup group in groups)
            {
                this.AppendIndentedLine(builder, 0, this.GetGroupFirstLine(group, settings));
                this.AppendIndentedLine(builder, 1, group.Description);
                if (group.ValueType != StartOptionValueType.Switch)
                {
                    this.AppendIndentedLine(builder, 1, $"value type: {group.ValueType}");
                }
                builder.AppendLine();
                this.AppendOptions(builder, settings, group.Options, 1);
                builder.AppendLine();
            }
        }

        private string GetGroupFirstLine(StartOptionGroup group, StartOptionParserSettings settings)
        {
            const string format = "{0}{2} | {1}{2}{3}(group)";


            string shortName = $"{settings.ShortOptionNamePrefix}{group.ShortName}";
            string longName = $"{settings.LongOptionNamePrefix}{group.LongName}";
            string indicator = this.GetOptionValueIndicator(group, settings, group.IsValueMandatory);

            switch(group.ValueType)
            {
                case StartOptionValueType.Switch: return String.Format(format, longName, shortName, String.Empty, String.Empty);
                case StartOptionValueType.Single:
                case StartOptionValueType.Multiple:
                    return String.Format(format, longName, shortName, indicator, " ");
                default: throw new NotImplementedException();
            }
        }

        protected void AppendOptions(StringBuilder builder, StartOptionParserSettings settings, IEnumerable<StartOption> options, int indentation)
        {
            foreach (StartOption option in options)
            {
                string valueIndicator = this.GetOptionValueIndicator(option, settings, option.IsMandatory);
                string line = $"{settings.LongOptionNamePrefix}{option.LongName}{valueIndicator} | {settings.ShortOptionNamePrefix}{option.ShortName}{valueIndicator} (option)";
                this.AppendIndentedLine(builder, indentation, line);
                this.AppendIndentedLine(builder, indentation + 1, option.Description);
                this.AppendIndentedLine(builder, indentation + 1, $"mandatory: {option.IsMandatory}");
                this.AppendIndentedLine(builder, indentation + 1, $"value type: {option.ValueType}");
                builder.AppendLine();
            }
        }

        protected string GetOptionValueIndicator(BaseStartOption option, StartOptionParserSettings settings, bool mandatory)
        {
            if (option.ValueType == StartOptionValueType.Switch)
            {
                return String.Empty;
            }
            string name = this.settings.PrintValueTypes ? (option.ValueParser == null ? "string" : option.ValueParser.ParsedType.Name.ToLower())
                                                        : "value";
            string valueFormat = mandatory ? "<{0}>" : "[<{0}>]";


            if (option.ValueType == StartOptionValueType.Single)
            {
                return $"{settings.OptionValueSeparator}{String.Format(valueFormat, name)}";
            }
            else
            {
                IEnumerable<string> names = new[] { 1, 2 }.Select(_int => name + "_" +  _int);
                return $"{settings.OptionValueSeparator}{String.Format(valueFormat, String.Join(",", names) + ",...")}";
            }
        }

        protected void AppendIndentedLine(StringBuilder builder, int indentation, string message)
        {
            if (!String.IsNullOrWhiteSpace(message))
            {
                for (int i = 0; i < indentation; i++)
                {
                    builder.Append(this.settings.Indentation);
                }
                builder.AppendLine(message);
            }
        }
    }

    public class ConsoleHelpPrinterSettings
    {
        /// <summary>
        /// Gets or sets the description of your application
        /// </summary>
        public string AppDescription { get; set; }
        /// <summary>
        /// Gets or sets the char for indenting each level of the help page
        /// </summary>
        public string Indentation { get; set; }
        /// <summary>
        /// Gets or sets whether the type of <see cref="StartOption"/>s and <see cref="StartOptionGroup"/>s should be printed to the console
        /// </summary>
        public bool PrintValueTypes { get; set; }
    }
}
