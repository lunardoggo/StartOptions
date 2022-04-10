using System;

namespace LunarDoggo.StartOptions
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class StartOptionGroupAttribute : Attribute
    {
        /// <param name="longName">Long name of the <see cref="StartOptionGroup"/></param>
        /// <param name="shortName">Short name of the <see cref="StartOptionGroup"/></param>
        public StartOptionGroupAttribute(string longName, string shortName)
        {
            this.ShortName = shortName;
            this.LongName = longName;
        }

        /// <summary>
        /// Gets or sets the description of the <see cref="StartOptionGroup"/> displayed on the help page
        /// </summary>
        public string Description { get; set; }

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
