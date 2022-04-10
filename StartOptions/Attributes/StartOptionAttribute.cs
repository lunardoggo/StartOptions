using LunarDoggo.StartOptions.Parsing.Values;
using System;

namespace LunarDoggo.StartOptions
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class StartOptionAttribute : Attribute
    {
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
        public StartOptionValueType ValueType { get; set; } = StartOptionValueType.Switch;
        /// <summary>
        /// Gets or sets the description of the <see cref="StartOption"/> displayed on the help page
        /// </summary>
        public string Description { get; set; } = String.Empty;
        /// <summary>
        /// Gets or sets whether the <see cref="StartOption"/> is groupless (true) or is part of a <see cref="StartOptionGroup"/> (false)
        /// </summary>
        public bool IsGrouplessOption { get; set; } = false;
        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the <see cref="IStartOptionValueParser"/> for the <see cref="StartOption"/>
        /// </summary>
        public Type ParserType { get; set; } = null;
        /// <summary>
        /// Gets or sets whether the <see cref="StartOption"/> has to be set. If this <see cref="StartOption"/> is groupless,
        /// it will always have to be set by users, if you set it to true. If it is part of a <see cref="StartOptionGroup"/>
        /// it is only required if the group is selected by the user
        /// </summary>
        public bool Mandatory { get; set; } = false;

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
