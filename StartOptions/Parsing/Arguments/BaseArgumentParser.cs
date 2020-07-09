using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LunarDoggo.StartOptions.Parsing.Arguments
{
    internal abstract class BaseArgumentParser
    {
        protected readonly StartOptionParserSettings settings;

        public BaseArgumentParser(StartOptionParserSettings settings)
        {
            this.settings = settings;
        }

        public abstract IEnumerable<ParsedStartArgument> Parse(string[] args);
        
        protected ParsedStartArgument GetValuelessParsedStartArgument(string arg)
        {
            bool isShortName = this.IsArgumentShortName(arg);
            string name = this.GetNameWithoutPrefix(arg, isShortName);
            return new ParsedStartArgument(arg, name, null, isShortName);
        }

        protected ParsedStartArgument GetParsedStartArgumentWithValue(string fullName, string value)
        {
            bool isShortName = this.IsArgumentShortName(fullName);
            string name = this.GetNameWithoutPrefix(fullName, isShortName);
            return new ParsedStartArgument(fullName, name, value, isShortName);
        }

        protected string GetNameWithoutPrefix(string name, bool isShortName)
        {
            string prefix = this.GetPrefix(isShortName);

            if (name.StartsWith(prefix))
            {
                return name.Substring(prefix.Length, name.Length - prefix.Length);
            }
            return name;
        }

        protected string GetPrefix(bool isShortName)
        {
            if (isShortName)
            {
                return this.settings.ShortOptionNamePrefix;
            }
            else
            {
                return this.settings.LongOptionNamePrefix;
            }
        }

        protected bool IsArgumentShortName(string name)
        {
            return    this.IsArgumentShortName(name, StartOptionParser.ValidOptionNameRegex)
                   || this.IsArgumentShortName(name, StartOptionParser.ValidHelpNameRegex);
        }

        private bool IsArgumentShortName(string name, Regex regex)
        {
            string pattern = $"^\\{String.Join("\\", this.settings.ShortOptionNamePrefix.ToArray())}{regex.ToString().TrimStart('^')}";
            return Regex.IsMatch(name, pattern);
        }
    }
}
