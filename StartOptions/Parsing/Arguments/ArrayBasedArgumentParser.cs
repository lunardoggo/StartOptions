using System.Collections.Generic;

namespace LunarDoggo.StartOptions.Parsing.Arguments
{
    internal class ArrayBasedArgumentParser : BaseArgumentParser
    {
        public ArrayBasedArgumentParser(StartOptionParserSettings settings) : base(settings)
        { }

        public override IEnumerable<ParsedStartArgument> Parse(string[] args)
        {
            foreach(string arg in args)
            {
                if(this.GetHasValue(arg))
                {
                    yield return this.GetParsedStartArgumentWithValue(arg);
                }
                else
                {
                    yield return this.GetValuelessParsedStartArgument(arg);
                }
            }
        }

        private bool GetHasValue(string arg)
        {
            return arg.IndexOf(this.settings.OptionValueSeparator) != -1;
        }

        private ParsedStartArgument GetParsedStartArgumentWithValue(string arg)
        {
            string[] parts = arg.Split(new char[] { this.settings.OptionValueSeparator });
            return this.GetParsedStartArgumentWithValue(parts[0], parts[1]);
            
        }
    }
}
