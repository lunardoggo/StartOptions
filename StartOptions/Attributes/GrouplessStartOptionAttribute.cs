using System;

namespace LunarDoggo.StartOptions.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class GrouplessStartOptionAttribute : Attribute
    {
        /// <param name="associatedLongName">Long name of the associated startoption provided in the application's constructor</param>
        public GrouplessStartOptionAttribute(string associatedLongName)
        {
            this.AssociatedLongName = associatedLongName;
        }

        public string AssociatedLongName { get; }
    }
}
