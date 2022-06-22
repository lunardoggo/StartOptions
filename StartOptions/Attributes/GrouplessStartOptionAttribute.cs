using System;

namespace LunarDoggo.StartOptions
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class GrouplessStartOptionAttribute : StartOptionAttribute
    {
        /// <summary>
        /// Defines a new groupless <see cref="StartOption"/>. This attribute has to be used to decorate either your <see cref="CommandApplication"/>'s class,
        /// one of your classes implementing <see cref="IApplicationCommand"/> or a constructor parameter in one of your classes implementing <see cref="IApplicationCommand"/>
        /// </summary>
        public GrouplessStartOptionAttribute(string longName, string shortName) : base(longName, shortName)
        { }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class GrouplessStartOptionReferenceAttribute : Attribute
    {
        /// <summary>
        /// Defines a reference to a previously defined <see cref="GrouplessStartOptionAttribute"/>
        /// </summary>
        /// <param name="associatedLongName">Long name of the associated startoption provided in the application's constructor</param>
        public GrouplessStartOptionReferenceAttribute(string associatedLongName)
        {
            this.AssociatedLongName = associatedLongName;
        }

        /// <summary>
        /// Long name of the referenced <see cref="GrouplessStartOptionAttribute"/>
        /// </summary>
        public string AssociatedLongName { get; }
    }
}
