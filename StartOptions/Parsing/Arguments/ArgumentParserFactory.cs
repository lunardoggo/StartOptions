using System;

namespace LunarDoggo.StartOptions.Parsing.Arguments
{
    internal class ArgumentParserFactory
    {
        public static ArgumentParserFactory Instance { get; } = new ArgumentParserFactory();

        private ArgumentParserFactory()
        { }

        public BaseArgumentParser GetParser(StartOptionParserSettings settings)
        {
            if (Char.IsWhiteSpace(settings.OptionValueSeparator))
            {
                return new SeparatorBasedArgumentParser(settings);
            }
            else
            {
                return new ArrayBasedArgumentParser(settings);
            }
        }
    }
}
