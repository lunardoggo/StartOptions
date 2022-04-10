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
        private readonly StartOptionParserSettings settings;
        private readonly HelpOption[] helpOptions;

        public ReflectionHelper(IEnumerable<HelpOption> helpOptions, StartOptionParserSettings settings)
        {
            this.helpOptions = helpOptions.ToArray();
            this.settings = settings.Clone();
        }

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
                StartOptionAttribute attribute = parameter.GetCustomAttribute<StartOptionAttribute>();
                StartOption option = allOptions.SingleOrDefault(_option => _option.LongName.Equals(attribute.LongName));

                if (option != null)
                {
                    switch (option.ValueType)
                    {
                        case StartOptionValueType.Switch:
                            values.Add(true);
                            break;
                        case StartOptionValueType.Multiple:
                        case StartOptionValueType.Single:
                            values.Add(option.GetValue<object>());
                            break;
                    }
                }
                else if (attribute.ValueType == StartOptionValueType.Switch)
                {
                    values.Add(false);
                }
                else
                {
                    values.Add(parameter.ParameterType.IsByRef ? null : parameter.DefaultValue);
                }
            }

            return values.ToArray();
        }

        public ApplicationStartOptions GetStartOptions(params Type[] types)
        {
            List<StartOptionGroupsParameter> parameters = new List<StartOptionGroupsParameter>();
            foreach (Type type in types)
            {
                parameters.Add(this.GetStartOptions(type, type.GetTypeInfo()));
            }
            ApplicationStartOptions options = new ApplicationStartOptions(parameters.Select(_param => _param.Groups).SelectMany(_grp => _grp).ToArray(),
                                                                          parameters.Select(_param => _param.GrouplessOptions).SelectMany(_opt => _opt).Distinct(StartOptionComparer.Instance).ToArray(),
                                                                          this.helpOptions, this.settings);
            this.ValidateStartOptionNames(options.StartOptionGroups, options.GrouplessStartOptions);
            return options;
        }

        private StartOptionGroupsParameter GetStartOptions(Type type, TypeInfo typeInfo)
        {
            this.ValidateTypeInfo(type, typeInfo);

            ConstructorInfo[] constructors = typeInfo.DeclaredConstructors.Where(_constructor => _constructor.GetCustomAttribute<StartOptionGroupAttribute>(true) != null && _constructor.IsPublic && !_constructor.IsStatic).ToArray();

            List<StartOptionGroup> groups = new List<StartOptionGroup>();
            List<StartOption> grouplessOptions = new List<StartOption>();

            foreach (ConstructorInfo constructor in constructors)
            {
                StartOptionGroupParameter? group = this.GetStartOptionGroup(type, constructor);

                if (group.HasValue)
                {
                    grouplessOptions.AddRange(group.Value.GrouplessOptions);
                    groups.Add(group.Value.Group);

                    if (!this.constructorCache.ContainsKey(group.Value.Group.LongName))
                    {
                        this.constructorCache.Add(group.Value.Group.LongName, constructor);
                    }
                }
            }

            return new StartOptionGroupsParameter(groups, grouplessOptions);
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

        private StartOptionGroupParameter? GetStartOptionGroup(Type type, ConstructorInfo constructor)
        {
            StartOptionGroupAttribute attribute = constructor.GetCustomAttribute<StartOptionGroupAttribute>();

            if (attribute != null)
            {
                List<StartOption> grouplessOptions = new List<StartOption>();
                List<StartOption> options = new List<StartOption>();

                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    StartOptionParameter option = this.GetStartOption(type, parameter);

                    if (option.IsGrouplessOption)
                    {
                        grouplessOptions.Add(option.Option);
                    }
                    else
                    {
                        options.Add(option.Option);
                    }
                }

                StartOptionGroup group = new StartOptionGroup(attribute.LongName, attribute.ShortName, attribute.Description, options);
                return new StartOptionGroupParameter(group, grouplessOptions);
            }
            return null;
        }

        private StartOptionParameter GetStartOption(Type type, ParameterInfo parameter)
        {
            StartOptionAttribute attribute = parameter.GetCustomAttribute<StartOptionAttribute>();
            if (attribute == null)
            {
                throw new NotSupportedException("All constructor parameters must be decorated with the StartOptionAttribute.", this.GetTypeInQuestionException(type));
            }

            IStartOptionValueParser parser = StartOptionValueParserRegistry.GetParser(attribute.ParserType);
            StartOption option = new StartOption(attribute.LongName, attribute.ShortName, attribute.Description, parser, attribute.ValueType, attribute.Mandatory);
            return new StartOptionParameter(option, attribute.IsGrouplessOption);
        }

        private Exception GetTypeInQuestionException(Type type)
        {
            return new Exception("Type in question: " + type.GetTypeInfo().FullName);
        }

        private struct StartOptionParameter
        {
            public StartOptionParameter(StartOption option, bool isGrouplessOption)
            {
                this.IsGrouplessOption = isGrouplessOption;
                this.Option = option;
            }

            public bool IsGrouplessOption { get; set; }
            public StartOption Option { get; set; }
        }

        private struct StartOptionGroupParameter
        {
            public StartOptionGroupParameter(StartOptionGroup group, List<StartOption> grouplessOptions)
            {
                this.GrouplessOptions = grouplessOptions;
                this.Group = group;
            }

            public List<StartOption> GrouplessOptions { get; set; }
            public StartOptionGroup Group { get; set; }
        }

        private struct StartOptionGroupsParameter
        {
            public StartOptionGroupsParameter(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
            {
                this.GrouplessOptions = grouplessOptions;
                this.Groups = groups;
            }

            public IEnumerable<StartOption> GrouplessOptions { get; set; }
            public IEnumerable<StartOptionGroup> Groups { get; set; }
        }

        private class StartOptionComparer : IEqualityComparer<StartOption>
        {
            public static StartOptionComparer Instance { get; }
            static StartOptionComparer()
            {
                StartOptionComparer.Instance = new StartOptionComparer();
            }

            public bool Equals(StartOption x, StartOption y)
            {
                return x == null && y == null ||
                       x.IsRequired == y.IsRequired
                    && x.LongName.Equals(y.LongName)
                    && x.ShortName.Equals(y.ShortName)
                    && x.Description.Equals(y.Description)
                    && x.ValueType == y.ValueType
                    && (x.ParserType == null && y.ParserType == null || x.ParserType == y.ParserType);
            }

            public int GetHashCode(StartOption option)
            {
                if (option == null)
                {
                    return 0;
                }

                unchecked
                {
                    return option.LongName.GetHashCode() * option.ShortName.GetHashCode() * option.Description.GetHashCode()
                         * option.IsRequired.GetHashCode() * option.ValueType.GetHashCode() * (option.ParserType?.FullName ?? String.Empty).GetHashCode();
                }
            }
        }
    }
}
