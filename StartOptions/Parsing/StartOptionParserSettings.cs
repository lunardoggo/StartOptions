namespace LunarDoggo.StartOptions.Parsing
{
    public class StartOptionParserSettings : IClonable<StartOptionParserSettings>
    {
        public bool ThrowErrorOnUnknownOption { get; set; } = true;
        public bool RequireStartOptionGroup { get; set; } = false;
        public string ShortOptionNamePrefix { get; set; } = "-";
        public string LongOptionNamePrefix { get; set; } = "--";
        public char MultipleValueSeparator { get; set; } = ',';
        public char OptionValueSeparator { get; set; } = '=';

        public StartOptionParserSettings Clone()
        {
            return new StartOptionParserSettings()
            {
                ThrowErrorOnUnknownOption = this.ThrowErrorOnUnknownOption,
                RequireStartOptionGroup = this.RequireStartOptionGroup,
                MultipleValueSeparator = this.MultipleValueSeparator,
                ShortOptionNamePrefix = this.ShortOptionNamePrefix,
                LongOptionNamePrefix = this.LongOptionNamePrefix,
                OptionValueSeparator = this.OptionValueSeparator
            };
        }
    }
}
