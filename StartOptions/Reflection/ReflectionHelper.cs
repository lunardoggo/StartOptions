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

            foreach (ParameterInfo parameter in parameters)
            {
                GrouplessStartOptionReferenceAttribute referenceAttribute = parameter.GetCustomAttribute<GrouplessStartOptionReferenceAttribute>();
                StartOptionGroupValueAttribute groupAttribute = parameter.GetCustomAttribute<StartOptionGroupValueAttribute>();
                StartOptionAttribute optionAttribute = parameter.GetCustomAttribute<StartOptionAttribute>();

                if (optionAttribute != null)
                {
                    StartOption option = this.GetBaseStartOption(optionAttribute.LongName, allOptions) as StartOption;
                    values.Add(this.GetStartOptionConstructorParameterValue(parameter, optionAttribute.ValueType, option));
                }
                else if (groupAttribute != null)
                {
                    StartOptionGroup group = options.ParsedOptionGroup;
                    values.Add(this.GetStartOptionConstructorParameterValue(parameter, group.ValueType, group));
                }
                else if (referenceAttribute != null)
                {
                    string name = referenceAttribute.AssociatedLongName;
                    StartOption option = this.GetBaseStartOption(name, allOptions) as StartOption;
                    StartOptionValueType? valueType = option?.ValueType;
                    if (!valueType.HasValue)
                    {
                        valueType = (this.GetBaseStartOption(name, options.ApplicationStartOptions.GrouplessStartOptions) as StartOption).ValueType;
                    }
                    values.Add(this.GetStartOptionConstructorParameterValue(parameter, valueType.Value, option));
                }
                else if (this.dependencyProvider != null)
                {
                    values.Add(this.dependencyProvider.GetDependency(parameter.ParameterType));
                }
            }

            return values.ToArray();
        }

        private BaseStartOption GetBaseStartOption(string longName, IEnumerable<BaseStartOption> source)
        {
            return source.SingleOrDefault(_option => _option.LongName.Equals(longName));
        }

        private object GetStartOptionConstructorParameterValue(ParameterInfo parameter, StartOptionValueType attributeValueType, BaseStartOption option)
        {
            if (option != null)
            {
                switch (option.ValueType)
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
            else if (attributeValueType == StartOptionValueType.Switch)
            {
                return false;
            }

            return parameter.ParameterType.GetTypeInfo().IsValueType ? Activator.CreateInstance(parameter.ParameterType) : null;
        }

        /// <summary>
        /// Returns the <see cref="ApplicationStartOptions"/> from all constructors of the given types which
        /// are decorated with the <see cref="StartOptionGroupAttribute"/>
        /// </summary>
        /// <exception cref="MissingStartOptionReferenceException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="NameConflictException"></exception>
        /// <exception cref="InvalidNameException"></exception>
        public ApplicationStartOptions GetStartOptions(params Type[] types)
        {
            Dictionary<string, List<Type>> startOptionReferences = new Dictionary<string, List<Type>>();
            List<StartOptionGroup> groups = new List<StartOptionGroup>();
            List<StartOption> options = new List<StartOption>();

            foreach (Type type in types)
            {
                this.ValidateTypeInfo(type, type.GetTypeInfo());
                this.ProcessTypeOptions(type, ref groups, ref options, ref startOptionReferences);
            }

            ApplicationStartOptions startOptions = new ApplicationStartOptions(groups, options.Distinct(StartOptionComparer.Instance), this.helpOptions, this.settings);
            this.ValidateStartOptionNames(startOptions.StartOptionGroups, startOptions.GrouplessStartOptions, startOptionReferences);
            return startOptions;
        }

        private void ProcessTypeOptions(Type type, ref List<StartOptionGroup> groups, ref List<StartOption> options, ref Dictionary<string, List<Type>> startOptionReferences)
        {
            if(typeof(CommandApplication).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                IEnumerable<GrouplessStartOptionAttribute> grouplessOptions = type.GetTypeInfo().GetCustomAttributes<GrouplessStartOptionAttribute>();
                options.AddRange(grouplessOptions.Select(_option => this.GetStartOption(_option)));
            }
            else
            {
                this.GetStartOptionsFromConstructors(type, ref groups, ref options, ref startOptionReferences);
            }
        }

        private void GetStartOptionsFromConstructors(Type type, ref List<StartOptionGroup> groups, ref List<StartOption> options, ref Dictionary<string, List<Type>> startOptionReferences)
        {
            List<StartOption> grouplessOptionCache = new List<StartOption>();
            List<StartOption> groupOptionCache = new List<StartOption>();

            foreach (ConstructorInfo constructor in type.GetTypeInfo().DeclaredConstructors.Where(_constructor => this.IsFeasableConstructor(_constructor)))
            {
                StartOptionGroupAttribute attribute = constructor.GetCustomAttribute<StartOptionGroupAttribute>();
                if (attribute != null)
                {
                    ConstructorStartOptions constructorOptions = this.GetConstructorStartOptions(attribute, constructor, ref groupOptionCache, ref grouplessOptionCache, ref startOptionReferences);
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

        private ConstructorStartOptions GetConstructorStartOptions(StartOptionGroupAttribute attribute, ConstructorInfo constructor, ref List<StartOption> groupOptionCache,
                                                                   ref List<StartOption> grouplessOptionCache, ref Dictionary<string, List<Type>> grouplessStartOptionReferences)
        {
            grouplessOptionCache.Clear();
            groupOptionCache.Clear();

            foreach (ParameterInfo parameter in constructor.GetParameters())
            {
                GrouplessStartOptionReferenceAttribute referenceAttribute = parameter.GetCustomAttribute<GrouplessStartOptionReferenceAttribute>();
                StartOptionGroupValueAttribute groupAttribute = parameter.GetCustomAttribute<StartOptionGroupValueAttribute>();
                StartOptionAttribute optionAttribute = this.GetSingleStartOptionAttribute(parameter);

                if (referenceAttribute == null && optionAttribute == null && groupAttribute == null && this.dependencyProvider == null)
                {
                    throw new InvalidOperationException("All constructor parameters must be decorated with the StartOptionAttribute unless a IDependencyProvider is provided");
                }
                else if (optionAttribute != null)
                {
                    StartOption option = this.GetStartOption(optionAttribute);
                    if (optionAttribute is GrouplessStartOptionAttribute)
                    {
                        grouplessOptionCache.Add(option);
                    }
                    else
                    {
                        groupOptionCache.Add(option);
                    }
                }
                else if (referenceAttribute != null)
                {
                    Type type = constructor.DeclaringType;
                    if (!grouplessStartOptionReferences.ContainsKey(referenceAttribute.AssociatedLongName))
                    {
                        grouplessStartOptionReferences.Add(referenceAttribute.AssociatedLongName, new List<Type>());
                    }
                    if (!grouplessStartOptionReferences[referenceAttribute.AssociatedLongName].Contains(type))
                    {
                        grouplessStartOptionReferences[referenceAttribute.AssociatedLongName].Add(type);
                    }
                }
            }

            IStartOptionValueParser parser = StartOptionValueParserRegistry.GetParser(attribute.ParserType);
            return new ConstructorStartOptions()
            {
                Group = new StartOptionGroup(attribute.LongName, attribute.ShortName, attribute.Description, parser, attribute.ValueType, groupOptionCache, attribute.IsValueMandatory),
                Options = grouplessOptionCache
            };
        }

        private StartOptionAttribute GetSingleStartOptionAttribute(ParameterInfo parameter)
        {
            IEnumerable<StartOptionAttribute> attributes = parameter.GetCustomAttributes<StartOptionAttribute>();
            if(attributes?.Count() > 1)
            {
                throw new AmbiguousMatchException($"There can only be one StartOptionAttribute or GrouplessStartOptionAttribute assignet to each parameter (parameter: {parameter.Name})");
            }
            return attributes.SingleOrDefault();
        }

        private StartOption GetStartOption(StartOptionAttribute attribute)
        {
            return new StartOption(attribute.LongName, attribute.ShortName, attribute.Description, StartOptionValueParserRegistry.GetParser(attribute.ParserType), attribute.ValueType, attribute.IsMandatory);
        }

        private void ValidateTypeInfo(Type type, TypeInfo typeInfo)
        {
            bool isApplication = typeof(CommandApplication).GetTypeInfo().IsAssignableFrom(typeInfo);
            bool isCommand = typeof(IApplicationCommand).GetTypeInfo().IsAssignableFrom(typeInfo);

            if (!isApplication && !isCommand)
            {
                throw new InvalidOperationException("The type has to implement IApplicationCommand or inherit from CommandApplication.", this.GetTypeInQuestionException(type));
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

        private void ValidateStartOptionNames(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> options, Dictionary<string, List<Type>> startOptionReferences)
        {
            StartOptionParserValidator validator = new StartOptionParserValidator(this.settings, groups, options, this.helpOptions);
            validator.CheckAreGrouplessStartOptionReferencesValid(startOptionReferences);
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
