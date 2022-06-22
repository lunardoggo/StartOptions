using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions.Parsing.Values;
using System;

namespace LunarDoggo.StartOptions
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class StartOptionAttribute : Attribute
    {
        /// <summary>
        /// Defines a new <see cref="StartOption"/>. This attribute has to decorate a constructor parameter of
        /// one of your classes implementing <see cref="IApplicationCommand"/>
        /// </summary>
        /// <param name="longName">Long name of the <see cref="StartOption"/></param>
        /// <param name="shortName">Short name of the <see cref="StartOption"/></param>
        public StartOptionAttribute(string longName, string shortName)
        {
            this.ShortName = shortName;
            this.LongName = longName;
        }

        /// <summary>
        /// Gets or sets the value type of the <see cref="StartOption"/> (Switch, Single, Multiple)
        /// </summary>
        public StartOptionValueType ValueType { get; set; } = StartOption.DefaultValueType;
        /// <summary>
        /// Gets or sets the description of the <see cref="StartOption"/> displayed on the help page
        /// </summary>
        public string Description { get; set; } = StartOption.DefaultDescription;
        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the <see cref="IStartOptionValueParser"/> for the <see cref="StartOption"/>
        /// </summary>
        public Type ParserType { get; set; } = StartOption.DefaultValueParser != null ? StartOption.DefaultValueParser.GetType() : null;
        /// <summary>
        /// Gets or sets whether the <see cref="StartOption"/> has to be set. If this <see cref="StartOption"/> is groupless,
        /// it will always have to be set by users, if you set it to true. If it is part of a <see cref="StartOptionGroup"/>
        /// it is only required if the group is selected by the user
        /// </summary>
        public bool IsMandatory { get; set; } = StartOption.DefaultIsMandatory;

        /// <summary>
        /// Gets or sets whether the <see cref="StartOption"/> has to be set. If this <see cref="StartOption"/> is groupless,
        /// it will always have to be set by users, if you set it to true. If it is part of a <see cref="StartOptionGroup"/>
        /// it is only required if the group is selected by the user
        /// </summary>
        [Obsolete("Mandatory will be removed in a future version, use IsMandatory instead")]
        public bool Mandatory { get => this.IsMandatory; set => this.IsMandatory = value; }

        /// <summary>
        /// Returns the short name of the <see cref="StartOption"/>
        /// </summary>
        public string ShortName { get; }
        /// <summary>
        /// Returns the long name of the <see cref="StartOption"/>
        /// </summary>
        public string LongName { get; }
    }
}
