using System;

namespace LunarDoggo.StartOptions.Exceptions
{
    public class InvalidNameException : Exception
    {
        public InvalidNameException(string message) : base(message)
        { }
    }
}
