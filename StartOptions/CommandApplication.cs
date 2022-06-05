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
            IApplicationCommand command = this.helper.Instantiate(new ParsedStartOptions(base.options, selectedGroup, selectedGrouplessOptions, false));
            if(command == null)
            {
                throw new NullReferenceException("Couldn't instantiate an IApplicationCommand with the provided parameters");
            }
            command.Execute();
        }

        /// <summary>
        /// Returns the <see cref="StartOptionParserSettings"/> the application should use
        /// </summary>
        protected virtual StartOptionParserSettings GetParserSettings()
        {
            return new StartOptionParserSettings();
        }

        /// <summary>
        /// Returns all <see cref="HelpOption"/>s supported by the application
        /// </summary>
        protected virtual IEnumerable<HelpOption> GetHelpOptions()
        {
            return StartOptionParser.DefaultHelpOptions;
        }

        /// <summary>
        /// Returns types of commands the application supports. All types must implement
        /// <see cref="IApplicationCommand"/>, must not be abstract and must contain at least one constructor
        /// decorated with <see cref="StartOptionGroupAttribute"/>
        /// </summary>
        protected abstract Type[] GetCommandTypes();

        /// <summary>
        /// Returns the <see cref="IDependencyProvider"/> to fill your command's constructor parameters that aren't
        /// decorated with a <see cref="StartOptionAttribute"/>
        /// </summary>
        protected abstract IDependencyProvider GetDependencyProvider();
    }
}
