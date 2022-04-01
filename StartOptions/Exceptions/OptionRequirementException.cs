using System;

namespace LunarDoggo.StartOptions.Exceptions
{
    public class OptionRequirementException : Exception
    {
        public OptionRequirementException(string message) : base(message)
        { }
    }
}
