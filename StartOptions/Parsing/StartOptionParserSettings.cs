namespace LunarDoggo.StartOptions.Parsing
{
    public class StartOptionParserSettings : IClonable<StartOptionParserSettings>
    {
        /// <summary>
        /// Gets or sets whether an <see cref="System.Exception"/> should be thrown if
        /// the <see cref="StartOptionParser"/> encounters an unknown <see cref="StartOption"/>
        /// </summary>
        public bool ThrowErrorOnUnknownOption { get; set; } = true;
        /// <summary>
        /// Gets or sets whether a <see cref="StartOptionGroup"/> must be selected by users in the command line arguments
        /// </summary>
        public bool RequireStartOptionGroup { get; set; } = false;
        /// <summary>
        /// Gets or sets the prefix for the short name of <see cref="StartOption"/>s
        /// </summary>
        public string ShortOptionNamePrefix { get; set; } = "-";
        /// <summary>
        /// Gets or sets the prefix for the long name of <see cref="StartOption"/>s
        /// </summary>
        public string LongOptionNamePrefix { get; set; } = "--";
        /// <summary>
        /// Gets or sets the <see cref="char"/> used to separate multiple values of <see cref="StartOption"/>s that
        /// use <see cref="StartOptionValueType.Multiple"/> as <see cref="StartOption.ValueType"/>
        /// </summary>
        public char MultipleValueSeparator { get; set; } = ',';
        /// <summary>
        /// Gets or sets the <see cref="char"/> used to separate <see cref="StartOption"/> names and <see cref="StartOption"/> values
        /// </summary>
        public char OptionValueSeparator { get; set; } = '=';

        /// <summary>
        /// Returns a copy of this <see cref="StartOptionParserSettings"/> instance
        /// </summary>
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
