using System.Collections.Generic;
using System;

namespace LunarDoggo.StartOptions.Reflection
{
    internal static class CommandReflectionHelper
    {
        public static IApplicationCommand Instantiate(StartOptionGroup group, IEnumerable<StartOption> grouplessOptions)
        {
            throw new NotImplementedException();
        }

        public static StartOptionGroup[] GetStartOptionGroupsInAssembly()
        {
            throw new NotImplementedException();
        }

        public static StartOption[] GetGrouplessStartOptionsInAssembly()
        {
            throw new NotImplementedException();
        }
    }
}
