﻿using LunarDoggo.StartOptions.Parsing.Values;
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
    public class DemoApplication : AbstractApplication
    {
        protected override void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
        {
            new ConsoleHelpPrinter('\t').Print(settings, helpOptions, groups, grouplessOptions);
        }

        protected override void Run(StartOptionGroup selectedGroup, IEnumerable<StartOption> selectedGrouplessOptions)
        {
            if(selectedGrouplessOptions.Any(_option => _option.ShortName.Equals("v")))
            {
                Console.WriteLine("Verbose option was toggled");
            }

            switch(selectedGroup.ShortName)
            {
                case "r":
                    this.RunReadFile(selectedGroup);
                    break;
                case "a":
                    this.RunAddNumbers(selectedGroup);
                    break;
            }
        }

        private void RunAddNumbers(StartOptionGroup group)
        {
            StartOption firstOption = group.GetOptionByShortName("1");
            StartOption secondOption = group.GetOptionByShortName("2");

            if (firstOption.HasValue && secondOption.HasValue)
            {
                int first = firstOption.GetValue<int>();
                int second = secondOption.GetValue<int>();

                Console.WriteLine("{0} + {1} = {2}", first, second, first + second);
            }
            else
            {
                if (!firstOption.HasValue)
                {
                    Console.WriteLine("Please provide the first number for the addition");
                }
                if(!secondOption.HasValue)
                {
                    Console.WriteLine("Please provide the second number for the addition");
                }
            }
        }

        private void RunReadFile(StartOptionGroup group)
        {
            string path = group.GetOptionByShortName("p").GetValue<string>();
            if (File.Exists(path))
            {
                Console.WriteLine("Contents of file \"{0}\":\n", path);
                Console.WriteLine(File.ReadAllText(path));
            }
            else
            {
                Console.WriteLine("File \"{0}\" not found", path);
            }
        }

        protected override ApplicationStartOptions GetApplicationStartOptions()
        {
            return new ApplicationStartOptions(this.GetStartOptionGroups(), this.GetGrouplessStartOptions());
        }

        private IEnumerable<StartOptionGroup> GetStartOptionGroups()
        {
            return new[]
            {
                new StartOptionGroupBuilder("read", "r").SetDescription("Reads the specified file to the console")
                    .AddOption("path", "p", (_option) => _option.SetDescription("Path to the file").SetValueType(StartOptionValueType.Single).SetRequired())
                    .Build(),
                new StartOptionGroupBuilder("add", "a").SetDescription("Adds two integers together")
                    .AddOption("value-1", "1", (_option) => _option.SetDescription("First value").SetValueType(StartOptionValueType.Single).SetValueParser(new Int32OptionValueParser()).SetRequired())
                    .AddOption("value-2", "2", (_option) => _option.SetDescription("Second value").SetValueType(StartOptionValueType.Single).SetValueParser(new Int32OptionValueParser()).SetRequired())
                    .Build()
            };
        }

        private IEnumerable<StartOption> GetGrouplessStartOptions()
        {
            return new[]
            {
                new StartOptionBuilder("verbose", "v").SetDescription("Enable verbose output").Build()
            };
        }
    }
}
