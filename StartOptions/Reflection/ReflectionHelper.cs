using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace LunarDoggo.StartOptions.Reflection
{
    internal class ReflectionHelper
    {
        private readonly StartOptionParserSettings settings;
        private readonly HelpOption[] helpOptions;

        public ReflectionHelper(IEnumerable<HelpOption> helpOptions, StartOptionParserSettings settings)
        {
            this.helpOptions = helpOptions.ToArray();
            this.settings = settings.Clone();
        }

        public IApplicationCommand Instantiate(ParsedStartOptions parsedOptions)
        {
            throw new NotImplementedException();
        }

        public ApplicationStartOptions GetStartOptions(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            if (!typeof(IApplicationCommand).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                throw new InvalidOperationException("The type has to inherit from IApplicationCommand.", this.GetTypeInQuestionException(type));
            }
            if(typeInfo.IsAbstract)
            {
                throw new InvalidOperationException("The type must not be abstract.", this.GetTypeInQuestionException(type));
            }
            if(typeInfo.IsGenericType)
            {
                throw new NotSupportedException("The type must not be generic.", this.GetTypeInQuestionException(type));
            }

            ConstructorInfo[] constructors = typeInfo.DeclaredConstructors.Where(_constructor => _constructor.GetCustomAttribute<StartOptionGroupAttribute>(true) != null && _constructor.IsPublic && !_constructor.IsStatic).ToArray();

            List<StartOptionGroup> groups = new List<StartOptionGroup>();
            List<StartOption> grouplessOptions = new List<StartOption>();

            foreach (var constructor in constructors)
            {
                StartOptionGroupParameter? group = this.GetStartOptionGroup(type, constructor);

                if(group.HasValue)
                {
                    grouplessOptions.AddRange(group.Value.GrouplessOptions);
                    groups.Add(group.Value.Group);
                }
            }

            StartOptionParserValidator validator = new StartOptionParserValidator(this.settings, groups, grouplessOptions, this.helpOptions);
            validator.CheckNameConflicts();

            return new ApplicationStartOptions(groups.ToArray(), grouplessOptions.ToArray(), this.helpOptions, this.settings);
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

                    if(option.IsGrouplessOption)
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

            //TODO: Parser!
            StartOption option = new StartOption(attribute.LongName, attribute.ShortName, attribute.Description, null, attribute.ValueType, attribute.Mandatory);
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
    }
}
