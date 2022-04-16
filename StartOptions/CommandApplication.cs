using LunarDoggo.StartOptions.DependencyInjection;
using LunarDoggo.StartOptions.Reflection;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using System;

namespace LunarDoggo.StartOptions
{
    public abstract class CommandApplication : AbstractApplication
    {
        private ReflectionHelper helper;

        protected override ApplicationStartOptions GetApplicationStartOptions()
        {
            StartOptionParserSettings settings = this.GetParserSettings();
            IEnumerable<HelpOption> helpOptions = this.GetHelpOptions();
            Type[] commandTypes = this.GetCommandTypes();
            this.helper = new ReflectionHelper(helpOptions, settings, this.GetDependencyProvider());
            return this.helper.GetStartOptions(commandTypes);
        }

        protected override void Run(StartOptionGroup selectedGroup, IEnumerable<StartOption> selectedGrouplessOptions)
        {
            IApplicationCommand command = this.helper.Instantiate(new ParsedStartOptions(selectedGroup, selectedGrouplessOptions, false));
            if(command == null)
            {
                throw new NullReferenceException("Couldn't instantiate an IApplicationCommand with the provided parameters");
            }
            Console.WriteLine(command.GetType().FullName);
            command.Execute();
        }

        protected virtual StartOptionParserSettings GetParserSettings()
        {
            return new StartOptionParserSettings();
        }

        protected virtual IEnumerable<HelpOption> GetHelpOptions()
        {
            return StartOptionParser.DefaultHelpOptions;
        }

        protected abstract IDependencyProvider GetDependencyProvider();
        protected abstract Type[] GetCommandTypes();
    }
}
