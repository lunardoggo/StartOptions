using LunarDoggo.StartOptions.DependencyInjection;
using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Exceptions;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace LunarDoggo.StartOptions.Reflection
{
    internal class ReflectionHelper
    {
        private readonly Dictionary<string, ConstructorInfo> constructorCache = new Dictionary<string, ConstructorInfo>();
        private readonly IDependencyProvider dependencyProvider;
        private readonly StartOptionParserSettings settings;
        private readonly HelpOption[] helpOptions;

        public ReflectionHelper(IEnumerable<HelpOption> helpOptions, StartOptionParserSettings settings, IDependencyProvider provider)
        {
            this.helpOptions = helpOptions.ToArray();
            this.settings = settings.Clone();
            this.dependencyProvider = provider;
        }

        /// <summary>
        /// Creates an instance of an <see cref="IApplicationCommand"/> with the provided <see cref="ParsedStartOptions"/>
        /// </summary>
        /// <exception cref="OptionRequirementException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public IApplicationCommand Instantiate(ParsedStartOptions parsedOptions)
        {
            if (parsedOptions.ParsedOptionGroup != null && this.constructorCache.TryGetValue(parsedOptions.ParsedOptionGroup.LongName, out ConstructorInfo constructor))
            {
                return constructor.Invoke(this.GetConstructorParameters(constructor.GetParameters(), parsedOptions)) as IApplicationCommand;
            }
            else if (this.settings.RequireStartOptionGroup)
            {
                throw new OptionRequirementException("At least one StartOptionGroup must be selected");
            }
            return null;
        }

        private object[] GetConstructorParameters(ParameterInfo[] parameters, ParsedStartOptions options)
        {
            StartOption[] allOptions = options.ParsedOptionGroup.Options.Concat(options.ParsedGrouplessOptions).ToArray();
            List<object> values = new List<object>();

            foreach(ParameterInfo parameter in parameters)
            {
                StartOptionAttribute attribute = parameter.GetCustomAttribute<StartOptionAttribute>();
                if(attribute != null)
                {
                    StartOption option = allOptions.SingleOrDefault(_option => _option.LongName.Equals(attribute.LongName));
                    values.Add(this.GetStartOptionConstructorParameterValue(parameter, attribute, option));
                }
                else if(this.dependencyProvider != null)
                {
                    values.Add(this.dependencyProvider.GetDependency(parameter.ParameterType));
                }
            }

            return values.ToArray();
        }

        private object GetStartOptionConstructorParameterValue(ParameterInfo parameter, StartOptionAttribute attribute, StartOption option)
        {
            if(option != null)
            {
                switch(option.ValueType)
                {
                    case StartOptionValueType.Switch:
                        return true;
                    case StartOptionValueType.Multiple:
                    case StartOptionValueType.Single:
                        return option.GetValue<object>();
                    default:
                        throw new NotImplementedException($"The StartOptionValueType \"{option.ValueType}\" isn't implemented yet");
                }
            }
            else if(attribute.ValueType == StartOptionValueType.Switch)
            {
                return false;
            }
            return parameter.ParameterType.IsByRef ? null : parameter.DefaultValue;
        }

        /// <summary>
        /// Returns the <see cref="ApplicationStartOptions"/> from all constructors of the given types which
        /// are decorated with the <see cref="StartOptionGroupAttribute"/>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="NameConflictException"></exception>
        /// <exception cref="InvalidNameException"></exception>
        public ApplicationStartOptions GetStartOptions(params Type[] types)
        {
            List<StartOptionGroup> groups = new List<StartOptionGroup>();
            List<StartOption> options = new List<StartOption>();

            foreach(Type type in types)
            {
                this.ValidateTypeInfo(type, type.GetTypeInfo());
                this.ProcessTypeOptions(type, ref groups, ref options);
            }

            ApplicationStartOptions startOptions = new ApplicationStartOptions(groups, options.Distinct(StartOptionComparer.Instance), this.helpOptions, this.settings);
            this.ValidateStartOptionNames(startOptions.StartOptionGroups, startOptions.GrouplessStartOptions);
            return startOptions;
        }

        private void ProcessTypeOptions(Type type, ref List<StartOptionGroup> groups, ref List<StartOption> options)
        {
            List<StartOption> grouplessOptionCache = new List<StartOption>();
            List<StartOption> groupOptionCache = new List<StartOption>();

            foreach (ConstructorInfo constructor in type.GetTypeInfo().DeclaredConstructors.Where(_constructor => this.IsFeasableConstructor(_constructor)))
            {
                StartOptionGroupAttribute attribute = constructor.GetCustomAttribute<StartOptionGroupAttribute>();
                if(attribute != null)
                {
                    ConstructorStartOptions constructorOptions = this.GetConstructorStartOptions(attribute, constructor, ref groupOptionCache, ref grouplessOptionCache);
                    if (constructorOptions.Options.Any())
                    {
                        options.AddRange(constructorOptions.Options);
                    }
                    groups.Add(constructorOptions.Group);
                    if (!this.constructorCache.ContainsKey(constructorOptions.Group.LongName))
                    {
                        this.constructorCache.Add(constructorOptions.Group.LongName, constructor);
                    }
                }
            }
        }

        private bool IsFeasableConstructor(ConstructorInfo constructor)
        {
            return constructor.GetCustomAttribute<StartOptionGroupAttribute>(true) != null && constructor.IsPublic && !constructor.IsStatic;
        }

        private ConstructorStartOptions GetConstructorStartOptions(StartOptionGroupAttribute attribute, ConstructorInfo constructor, ref List<StartOption> groupOptionCache, ref List<StartOption> grouplessOptionCache)
        {
            grouplessOptionCache.Clear();
            groupOptionCache.Clear();

            foreach(ParameterInfo parameter in constructor.GetParameters())
            {
                StartOptionAttribute optionAttribute = parameter.GetCustomAttribute<StartOptionAttribute>();
                if(optionAttribute == null && this.dependencyProvider == null)
                {
                    throw new InvalidOperationException("All constructor parameters must be decorated with the StartOptionAttribute unless a IDependencyProvider is provided");
                }
                else if(optionAttribute != null)
                {
                    StartOption option = this.GetStartOption(optionAttribute);
                    if(optionAttribute.IsGrouplessOption)
                    {
                        grouplessOptionCache.Add(option);
                    }
                    else
                    {
                        groupOptionCache.Add(option);
                    }
                }
            }

            return new ConstructorStartOptions()
            {
                Group = new StartOptionGroup(attribute.LongName, attribute.ShortName, attribute.Description, groupOptionCache),
                Options = grouplessOptionCache
            };
        }

        private StartOption GetStartOption(StartOptionAttribute attribute)
        {
            return new StartOption(attribute.LongName, attribute.ShortName, attribute.Description, StartOptionValueParserRegistry.GetParser(attribute.ParserType), attribute.ValueType, attribute.Mandatory);
        }

        private void ValidateTypeInfo(Type type, TypeInfo typeInfo)
        {
            if (!typeof(IApplicationCommand).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                throw new InvalidOperationException("The type has to inherit from IApplicationCommand.", this.GetTypeInQuestionException(type));
            }
            if (typeInfo.IsAbstract)
            {
                throw new InvalidOperationException("The type must not be abstract.", this.GetTypeInQuestionException(type));
            }
            if (typeInfo.IsGenericType)
            {
                throw new NotSupportedException("The type must not be generic.", this.GetTypeInQuestionException(type));
            }
        }

        private void ValidateStartOptionNames(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> options)
        {
            StartOptionParserValidator validator = new StartOptionParserValidator(this.settings, groups, options, this.helpOptions);
            validator.CheckNameConflicts();
        }

        private Exception GetTypeInQuestionException(Type type)
        {
            return new Exception("Type in question: " + type.GetTypeInfo().FullName);
        }

        private struct ConstructorStartOptions
        {
            public IEnumerable<StartOption> Options { get; set; }
            public StartOptionGroup Group { get; set; }
        }
    }
}
