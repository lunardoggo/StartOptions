using LunarDoggo.StartOptions.DependencyInjection;
using LunarDoggo.StartOptions.Reflection;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LunarDoggo.StartOptions
{
    public abstract class CommandApplication : AbstractApplication
    {
        private readonly Dictionary<string, Action<object>> grouplessOptionHandlers = new Dictionary<string, Action<object>>();
        private ReflectionHelper helper;

        protected override ApplicationStartOptions GetApplicationStartOptions()
        {
            StartOptionParserSettings settings = this.GetParserSettings();
            IEnumerable<HelpOption> helpOptions = this.GetHelpOptions();
            Type[] commandTypes = this.GetCommandTypes().Concat(new[] { this.GetType() }).ToArray();
            this.helper = new ReflectionHelper(helpOptions, settings, this.GetDependencyProvider());
            return this.helper.GetStartOptions(commandTypes);
        }

        protected override void Run(StartOptionGroup selectedGroup, IEnumerable<StartOption> selectedGrouplessOptions)
        {
            IApplicationCommand command = this.helper.Instantiate(new ParsedStartOptions(base.options, selectedGroup, selectedGrouplessOptions, false));

            if (this.grouplessOptionHandlers.Count > 0)
            {
                foreach (StartOption option in selectedGrouplessOptions)
                {
                    if (this.grouplessOptionHandlers.ContainsKey(option.LongName))
                    {
                        object value = null;
                        if (option.HasValue)
                        {
                            value = option.ValueType == StartOptionValueType.Multiple ? option.GetValue<object[]>() : option.GetValue<object>();
                        }

                        this.grouplessOptionHandlers[option.LongName]?.Invoke(value);
                    }
                }
            }

            if (command != null)
            {
                command.Execute();
            }
        }

        /// <summary>
        /// Adds an action to handle the groupless <see cref="StartOption"/> with the given long name.
        /// This method should be used for groupless <see cref="StartOption"/>s of type <see cref="StartOptionValueType.Multiple"/>,
        /// additionally make sure that the type <see cref="{T}"/> matches the value type of the <see cref="StartOption"/>
        /// </summary>
        public virtual void AddGlobalGrouplessStartOptionHandler<T>(string longName, Action<T[]> action)
        {
            this.AddGlobalGrouplessStartOptionHandler(longName, _value => action.Invoke(((object[])_value).Cast<T>().ToArray()));
        }

        /// <summary>
        /// Adds an action to handle the groupless <see cref="StartOption"/> with the given long name.
        /// This method should be used for groupless <see cref="StartOption"/>s of type <see cref="StartOptionValueType.Single"/>,
        /// additionally make sure that the type <see cref="{T}"/> matches the value type of the <see cref="StartOption"/>
        /// </summary>
        public virtual void AddGlobalGrouplessStartOptionHandler<T>(string longName, Action<T> action)
        {
            this.AddGlobalGrouplessStartOptionHandler(longName, _value =>
            {
                if (!(_value is T))
                {
                    throw new ArgumentException($"Could not cast value ot type {_value.GetType().FullName} to type {typeof(T).FullName}");
                }
                action.Invoke((T)_value);
            });
        }

        /// <summary>
        /// Adds an action to handle the groupless <see cref="StartOption"/> with the given long name.
        /// This method should be used for groupless <see cref="StartOption"/>s of type <see cref="StartOptionValueType.Switch"/>
        /// </summary>
        public virtual void AddGlobalGrouplessStartOptionHandler(string longName, Action action)
        {
            this.AddGlobalGrouplessStartOptionHandler(longName, _value => action.Invoke());
        }

        /// <summary>
        /// Adds an action to handle the groupless <see cref="StartOption"/> with the given long name
        /// </summary>
        protected virtual void AddGlobalGrouplessStartOptionHandler(string longName, Action<object> action)
        {
            if(this.grouplessOptionHandlers.ContainsKey(longName))
            {
                throw new InvalidOperationException("There can only be one groupless start option handler per groupless start option");
            }
            this.grouplessOptionHandlers.Add(longName, action);
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
