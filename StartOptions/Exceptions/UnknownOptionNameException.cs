using System;

namespace LunarDoggo.StartOptions.Exceptions
{
    public class UnknownOptionNameException : Exception
    {
        public UnknownOptionNameException(string message) : base(message)
        { }
    }
}
