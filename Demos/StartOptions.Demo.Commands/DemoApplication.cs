using LunarDoggo.StartOptions.DependencyInjection;
using LunarDoggo.StartOptions.HelpPages;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System;

namespace StartOptions.Demo
{
    [GrouplessStartOption("verbose", "v", Description = "Enables verbose output")]
    public class DemoApplication : CommandApplication
    {
        private const string appDescription = "This app is intended as a demonstration of the attribute based workflow with the LunarDoggo.StartOptions library.";
        private readonly IHelpPagePrinter helpPrinter = new ConsoleHelpPrinter(new ConsoleHelpPrinterSettings()
        {
            AppDescription = appDescription,
            PrintValueTypes = true,
            Indentation = "  "
        });

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
                ShortOptionNamePrefix = "--",
                LongOptionNamePrefix = "/",
                OptionValueSeparator = ' ',
            };
        }

        protected override IDependencyProvider GetDependencyProvider()
        {
            return null;
        }

        protected override Type[] GetCommandTypes()
        {
            return new[] { typeof(AddCommand), typeof(ReadFileCommand), typeof(SumCommand) };
        }
    }
}
