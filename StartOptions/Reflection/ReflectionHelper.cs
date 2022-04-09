using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace LunarDoggo.StartOptions.Reflection
{
    internal static class ReflectionHelper
    {
        public static IApplicationCommand Instantiate(StartOptionGroup group, IEnumerable<StartOption> grouplessOptions)
        {
            throw new NotImplementedException();
        }

        public static ApplicationStartOptions GetStartOptions(Type type)
        {
            throw new NotImplementedException();
        }

    }
}
