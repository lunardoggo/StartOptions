﻿using LunarDoggo.StartOptions.Interfaces;

namespace LunarDoggo.StartOptions.Parsing
{
    public class StartOptionParserSettings : IClonable<StartOptionParserSettings>
    {
        public bool ThrowErrorOnUnknownOption { get; set; } = true;
        public string ShortOptionNamePrefix { get; set; } = "-";
        public string LongOptionNamePrefix { get; set; } = "--";
        public char MultipleValueSeparator { get; set; } = ',';
        public char OptionValueSeparator { get; set; } = '=';

        public StartOptionParserSettings Clone()
        {
            return new StartOptionParserSettings()
            {
                ThrowErrorOnUnknownOption = this.ThrowErrorOnUnknownOption,
                MultipleValueSeparator = this.MultipleValueSeparator,
                ShortOptionNamePrefix = this.ShortOptionNamePrefix,
                LongOptionNamePrefix = this.LongOptionNamePrefix,
                OptionValueSeparator = this.OptionValueSeparator
            };
        }
    }
}
