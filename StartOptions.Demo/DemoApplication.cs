using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.HelpPages;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System.Linq;
using System.IO;
using System;

namespace StartOptions.Demo
{
    public class DemoApplication : CommandApplication
    {
        private readonly IHelpPagePrinter helpPrinter = new ConsoleHelpPrinter('\t');

        protected override void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
        {
            this.helpPrinter.Print(settings, helpOptions, groups, grouplessOptions);
        }

        protected override IEnumerable<HelpOption> GetHelpOptions()
        {
            return new HelpOption[]
            {
                new HelpOption("help", false),
                new HelpOption("h", true)
            };
        }

        protected override StartOptionParserSettings GetParserSettings()
        {
            // Require at least one StartOptionGroup to be provided in the cli arguments
            // Use "/" as prefix for long option names like "/add" or "/help"
            // Use "-" as prefix for short option names like "-a" or "-h"
            // Separate values from the corresponding start option with a space
            // Examples start options: 
            // /> .\StartOptions.Demo.exe /add -1 3 /value-2 5
            // /> .\StartOptions.Demo.exe -h
            return new StartOptionParserSettings()
            {
                RequireStartOptionGroup = true,
                ShortOptionNamePrefix = "-",
                LongOptionNamePrefix = "/",
                OptionValueSeparator = ' ',
            };
        }

        protected override Type[] GetCommandTypes()
        {
            return new[] { typeof(AddCommand), typeof(ReadFileCommand) };
        }
    }
}
