namespace LunarDoggo.StartOptions.Building
{
    public class HelpOptionBuilder
    {
        private readonly string name;

        private bool isShortName;

        /// <summary>
        /// Creates a new builder for a <see cref="HelpOption"/> with the provided name
        /// </summary>
        public HelpOptionBuilder(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Sets whether the <see cref="HelpOption"/> instance's name will use <see cref="Parsing.StartOptionParserSettings.ShortOptionNamePrefix"/>
        /// or <see cref="Parsing.StartOptionParserSettings.LongOptionNamePrefix"/>
        /// </summary>
        public HelpOptionBuilder SetIsShortName(bool isShortName = true)
        {
            this.isShortName = isShortName;
            return this;
        }

        /// <summary>
        /// Sets whether the <see cref="HelpOption"/> instance's name will use <see cref="Parsing.StartOptionParserSettings.LongOptionNamePrefix"/>
        /// or <see cref="Parsing.StartOptionParserSettings.ShortOptionNamePrefix"/>
        /// </summary>
        public HelpOptionBuilder SetIsLongName(bool isLongName = true)
        {
            return this.SetIsShortName(!isLongName);
        }

        /// <summary>
        /// Returns a new instance of <see cref="HelpOption"/> with all stored values
        /// </summary>
        public HelpOption Build()
        {
            return new HelpOption(this.name, this.isShortName);
        }
    }
}
