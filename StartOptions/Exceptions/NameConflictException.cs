using System;

namespace LunarDoggo.StartOptions.Exceptions
{
    public class NameConflictException : Exception
    {
        public NameConflictException(string message) : base(message)
        { }
    }
}
