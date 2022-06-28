using System;

namespace LunarDoggo.StartOptions
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class StartOptionGroupAttribute : Attribute
    {
        /// <summary>
        /// Defines a new <see cref="StartOptionGroup"/>. This attribute has to be used to decorate
        /// one of the constructors of one of you classes implementing <see cref="IApplicationCommand"/>
        /// </summary>
        /// <param name="longName">Long name of the <see cref="StartOptionGroup"/></param>
        /// <param name="shortName">Short name of the <see cref="StartOptionGroup"/></param>
        public StartOptionGroupAttribute(string longName, string shortName)
        {
            this.ShortName = shortName;
            this.LongName = longName;
        }

        /// <summary>
        /// Gets or sets the value type of the <see cref="StartOption"/> (Switch, Single, Multiple)
        /// </summary>
        public StartOptionValueType ValueType { get; set; } = StartOption.DefaultValueType;
        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the <see cref="IStartOptionValueParser"/> for the <see cref="StartOption"/>
        /// </summary>
        public Type ParserType { get; set; } = StartOption.DefaultValueParser != null ? StartOption.DefaultValueParser.GetType() : null;
        /// <summary>
        /// Gets or sets the description of the <see cref="StartOptionGroup"/> displayed on the help page
        /// </summary>
        public string Description { get; set; } = StartOption.DefaultDescription;

        /// <summary>
        /// Gets or seths whether the <see cref="StartOptionGroup"/> must be provided with a value.
        /// Note: this property only takes effect, if (<see cref="ValueType"/> != <see cref="StartOptionValueType.Switch"/>)
        /// </summary>
        public bool IsValueMandatory { get; set; }

        /// <summary>
        /// Returns the short name of the <see cref="StartOptionGroup"/>
        /// </summary>
        public string ShortName { get; }
        /// <summary>
        /// Returns the long name of the <see cref="StartOptionGroup"/>
        /// </summary>
        public string LongName { get; }
    }
}
