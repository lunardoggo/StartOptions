using LunarDoggo.StartOptions.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LunarDoggo.StartOptions.Parsing.Arguments
{
    internal class SeparatorBasedArgumentParser : BaseArgumentParser
    {
        public SeparatorBasedArgumentParser(StartOptionParserSettings settings) : base(settings)
        { }

        public override IEnumerable<ParsedStartArgument> Parse(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if(i < args.Length - 1 && !this.IsArgumentValue(args[i + 1]))
                {
                    yield return this.GetParsedStartArgumentWithValue(arg, args[i + 1]);
                    i++;
                }
                else
                {
                    yield return this.GetValuelessParsedStartArgument(arg);
                }
            }
        }

        private bool IsArgumentValue(string arg)
        {
            return    arg.StartsWith(this.settings.ShortOptionNamePrefix)
                   || arg.StartsWith(this.settings.LongOptionNamePrefix);
        }
    }
}
