using System;

namespace LunarDoggo.StartOptions.Exceptions
{
    public class MissingStartOptionReferenceException : Exception
    {
        public MissingStartOptionReferenceException(string message) : base(message)
        { }
    }
}
