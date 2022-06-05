using System;

namespace LunarDoggo.StartOptions
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class GrouplessStartOptionAttribute : StartOptionAttribute
    {
        public GrouplessStartOptionAttribute(string longName, string shortName) : base(longName, shortName)
        { }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class GrouplessStartOptionReferenceAttribute : Attribute
    {
        /// <param name="associatedLongName">Long name of the associated startoption provided in the application's constructor</param>
        public GrouplessStartOptionReferenceAttribute(string associatedLongName)
        {
            this.AssociatedLongName = associatedLongName;
        }

        public string AssociatedLongName { get; }
    }
}
