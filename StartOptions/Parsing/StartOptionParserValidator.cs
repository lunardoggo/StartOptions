﻿using LunarDoggo.StartOptions.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LunarDoggo.StartOptions.Parsing
{
    internal class StartOptionParserValidator
    {
        private readonly IEnumerable<StartOptionGroup> optionGroups;
        private readonly IEnumerable<StartOption> grouplessOptions;
        private readonly HelpOption[] helpOptions;

        internal StartOptionParserValidator(StartOptionParserSettings settings, IEnumerable<StartOptionGroup> groups,
                                 IEnumerable<StartOption> grouplessOptions, HelpOption[] helpOptions)
        {
            this.grouplessOptions = grouplessOptions;
            this.helpOptions = helpOptions;
            this.optionGroups = groups;
            
            if(settings == null)
            {
                throw new ArgumentException("StartOptionParserSettings can't be null");
            }

            this.CheckNameConflicts();
        }

        public void CheckNameConflicts()
        {
            this.CheckGrouplessStartOptionNameConflicts();
            this.CheckStartOptionGroupNameConflicts();
            this.CheckStartOptionGroupGrouplessOptionsNameConflicts();
            this.CheckHelpOptionNameConflicts();
        }

        private void CheckGrouplessStartOptionNameConflicts()
        {
            string[] shortNameConflicts = this.GetNameConflicts(this.grouplessOptions.Select(_option => _option.ShortName));
            if (shortNameConflicts.Length > 0)
            {
                string message = $"Conflicting short groupless option name{(shortNameConflicts.Length == 1 ? "" : "s")}: {String.Join(",", shortNameConflicts)}";
                throw new NameConflictException(message);
            }

            string[] longNameConflicts = this.GetNameConflicts(this.grouplessOptions.Select(_option => _option.LongName));
            if (longNameConflicts.Length > 0)
            {
                string message = $"Conflicting short groupless start option name{(longNameConflicts.Length == 1 ? "" : "s")}: {String.Join(",", longNameConflicts)}";
                throw new NameConflictException(message);
            }
        }

        private void CheckStartOptionGroupNameConflicts()
        {
            string[] shortNameConflicts = this.GetNameConflicts(this.optionGroups.Select(_group => _group.ShortName));
            if (shortNameConflicts.Length > 0)
            {
                string message = $"Conflicting short start option group name{(shortNameConflicts.Length == 1 ? "" : "s")}: {String.Join(",", shortNameConflicts)}";
                throw new NameConflictException(message);
            }

            string[] longNameConflicts = this.GetNameConflicts(this.optionGroups.Select(_group => _group.LongName));
            if (longNameConflicts.Length > 0)
            {
                string message = $"Conflicting short start option group name{(longNameConflicts.Length == 1 ? "" : "s")}: {String.Join(",", longNameConflicts)}";
                throw new NameConflictException(message);
            }
        }

        private void CheckStartOptionGroupGrouplessOptionsNameConflicts()
        {
            IEnumerable<string> grouplessShortNames = this.grouplessOptions.Select(_option => _option.ShortName);
            IEnumerable<string> grouplessLongNames = this.grouplessOptions.Select(_option => _option.LongName);

            foreach (StartOptionGroup group in this.optionGroups)
            {
                if (grouplessShortNames.Contains(group.ShortName))
                {
                    throw new NameConflictException($"Short start option name {group.ShortName} is set for a groupless start option and a start option group.");
                }
                else if (grouplessLongNames.Contains(group.LongName))
                {
                    throw new NameConflictException($"Long start option name {group.LongName} is set for a groupless start option and a start option group.");
                }

                IEnumerable<string> shortNames = grouplessShortNames.Concat(group.Options.Select(_option => _option.ShortName));
                string[] innerShortConflicts = this.GetNameConflicts(shortNames);
                if (innerShortConflicts.Length > 0)
                {
                    string message = $"Short start option name{(innerShortConflicts.Length == 1 ? "" : "s")} can't be used in groupless and group {group.LongName}: {String.Join(", ", innerShortConflicts)}";
                    throw new NameConflictException(message);
                }

                IEnumerable<string> longNames = grouplessLongNames.Concat(group.Options.Select(_option => _option.LongName));
                string[] innerLongConflicts = this.GetNameConflicts(longNames);
                if (innerLongConflicts.Length > 0)
                {
                    string message = $"Long start option name{(innerLongConflicts.Length == 1 ? "" : "s")} can't be used in groupless and group {group.LongName}: {String.Join(", ", innerLongConflicts)}";
                    throw new NameConflictException(message);
                }
            }
        }

        private void CheckHelpOptionNameConflicts()
        {
            this.CheckHelpGrouplessOptionNameConflicts();
            this.CheckHelpGroupOptionNameConflicts();
        }

        private void CheckHelpGrouplessOptionNameConflicts()
        {
            IEnumerable<string> shortNames = this.grouplessOptions.Select(_option => _option.ShortName);
            string[] shortHelpConflicts = this.GetHelpOptionNameConflicts(shortNames, true);
            if (shortHelpConflicts.Length > 0)
            {
                bool hasOne = shortHelpConflicts.Length == 1;
                string names = String.Join(", ", shortHelpConflicts);
                throw new NameConflictException($"Short groupless start option name{(hasOne ? "" : "s")} conflict with help option name{(hasOne ? "" : "s")}: {names}");
            }

            IEnumerable<string> longNames = this.grouplessOptions.Select(_option => _option.LongName);
            string[] longHelpConflicts = this.GetHelpOptionNameConflicts(longNames, true);
            if (longHelpConflicts.Length > 0)
            {
                bool hasOne = longHelpConflicts.Length == 1;
                string names = String.Join(", ", longHelpConflicts);
                throw new NameConflictException($"Long groupless start option name{(hasOne ? "" : "s")} conflict with help option name{(hasOne ? "" : "s")}: {names}");
            }
        }

        private void CheckHelpGroupOptionNameConflicts()
        {
            foreach (StartOptionGroup group in this.optionGroups)
            {
                this.CheckHelpGroupOptionNameConflicts(group);
            }
        }

        private void CheckHelpGroupOptionNameConflicts(StartOptionGroup group)
        {
            if (this.helpOptions.Any(_option => _option.IsShortName && group.ShortName.Equals(_option.Name)))
            {
                throw new NameConflictException($"Short start option group name \"{group.ShortName}\" conflicts with a help option name");
            }
            else if (this.helpOptions.Any(_option => !_option.IsShortName && group.LongName.Equals(_option.Name)))
            {
                throw new NameConflictException($"Long start option group name \"{group.LongName}\" conflicts with a help option name");
            }

            IEnumerable<string> shortNames = group.Options.Select(_option => _option.ShortName);
            string[] shortNameConflicts = this.GetHelpOptionNameConflicts(shortNames, true);
            if (shortNameConflicts.Length > 0)
            {
                bool hasOne = shortNameConflicts.Length == 1;
                string names = String.Join(", ", shortNameConflicts);
                throw new NameConflictException($"Short start option name{(hasOne ? "" : "s")} conflict inside of group \"{group.LongName}\" with help option name{(hasOne ? "" : "s")}: {names}");
            }

            IEnumerable<string> longNames = group.Options.Select(_option => _option.LongName);
            string[] longNameConflicts = this.GetHelpOptionNameConflicts(shortNames, false);
            if (longNameConflicts.Length > 0)
            {
                bool hasOne = longNameConflicts.Length == 1;
                string names = String.Join(", ", longNameConflicts);
                throw new NameConflictException($"Long start option name{(hasOne ? "" : "s")} conflict inside of group \"{group.LongName}\" with help option name{(hasOne ? "" : "s")}: {names}");
            }
        }

        private string[] GetHelpOptionNameConflicts(IEnumerable<string> optionNames, bool isShortName)
        {
            IEnumerable<string> names = this.helpOptions.Where(_option => _option.IsShortName == isShortName)
                                                        .Select(_option => _option.Name).Concat(optionNames);
            return this.GetNameConflicts(names);
        }

        private string[] GetNameConflicts(IEnumerable<string> names)
        {
            return names.GroupBy(_name => _name).Where(_grp => _grp.Count() > 1).Select(_grp => _grp.Key).ToArray();
        }
    }
}
