using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace LunarDoggo.StartOptions.HelpPages
{
    public class ConsoleHelpPrinter : IHelpPagePrinter
    {
        private readonly char indentationChar;

        public ConsoleHelpPrinter(char indentationChar)
        {
            this.indentationChar = indentationChar;
        }

        /// <summary>
        /// Prints the help page to the console according to the provided parameters
        /// </summary>
        public void Print(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
        {
            StringBuilder builder = new StringBuilder();

            this.AppendHelpOptions(builder, settings, helpOptions);
            this.AppendGroups(builder, settings, groups);
            this.AppendOptions(builder, settings, grouplessOptions, 0);

            Console.WriteLine(builder.ToString());
        }

        private void AppendHelpOptions(StringBuilder builder, StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions)
        {
            builder.Append("Available help options: ");
            builder.AppendLine(String.Join(",", helpOptions.Select(_option => this.GetHelpOptionName(settings, _option))));
            builder.AppendLine();
        }

        private string GetHelpOptionName(StartOptionParserSettings settings, HelpOption option)
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

        private void AppendGroups(StringBuilder builder, StartOptionParserSettings settings, IEnumerable<StartOptionGroup> groups)
        {
            foreach (StartOptionGroup group in groups)
            {
                this.AppendIndentedLine(builder, 0, $"{settings.LongOptionNamePrefix}{group.LongName} | {settings.ShortOptionNamePrefix}{group.ShortName} (group)");
                this.AppendIndentedLine(builder, 1, group.Description);
                this.AppendIndentedLine(builder, 1, "type: start option group");
                builder.AppendLine();
                this.AppendOptions(builder, settings, group.Options, 1);
                builder.AppendLine();
            }
        }

        private void AppendOptions(StringBuilder builder, StartOptionParserSettings settings, IEnumerable<StartOption> options, int indentation)
        {
            foreach (StartOption option in options)
            {
                string valueIndicator = this.GetOptionValueIndicator(option, settings);
                string line = $"{settings.LongOptionNamePrefix}{option.LongName}{valueIndicator} | {settings.ShortOptionNamePrefix}{option.ShortName}{valueIndicator}";
                this.AppendIndentedLine(builder, indentation, line);
                this.AppendIndentedLine(builder, indentation + 1, option.Description);
                this.AppendIndentedLine(builder, indentation, "type: start option");
                this.AppendIndentedLine(builder, indentation + 1, $"required: {option.IsMandatory}");
                this.AppendIndentedLine(builder, indentation + 1, $"value type: {option.ValueType}");
                builder.AppendLine();
            }
        }

        private string GetOptionValueIndicator(StartOption option, StartOptionParserSettings settings)
        {
            if (option.ValueType == StartOptionValueType.Switch)
            {
                return String.Empty;
            }
            string name = option.LongName.ToLower();
            string valueFormat = option.IsMandatory ? "<{0}>" : "[<{0}>]";

            
            if (option.ValueType == StartOptionValueType.Single)
            {
                return $"{settings.OptionValueSeparator}{String.Format(valueFormat, name)}";
            }
            else
            {
                IEnumerable<string> names = new[] { 1, 2 }.Select(_int => name + _int);
                return $"{settings.OptionValueSeparator}{String.Format(valueFormat, String.Join(",", names) + ",...")}";
            }
        }

        private void AppendIndentedLine(StringBuilder builder, int indentation, string message)
        {
            for (int i = 0; i < indentation; i++)
            {
                builder.Append(this.indentationChar);
            }
            builder.AppendLine(message);
        }
    }
}
