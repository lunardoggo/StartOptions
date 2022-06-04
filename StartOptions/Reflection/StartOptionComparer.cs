using System.Collections.Generic;
using System;

namespace LunarDoggo.StartOptions.Reflection
{
    public class StartOptionComparer : IEqualityComparer<StartOption>
    {
        public static StartOptionComparer Instance { get; }
        static StartOptionComparer()
        {
            StartOptionComparer.Instance = new StartOptionComparer();
        }

        public bool Equals(StartOption x, StartOption y)
        {
            return x == null && y == null ||
                   x.IsMandatory == y.IsMandatory
                && x.LongName.Equals(y.LongName)
                && x.ShortName.Equals(y.ShortName)
                && x.Description.Equals(y.Description)
                && x.ValueType == y.ValueType
                && (x.ParserType == null && y.ParserType == null || x.ParserType == y.ParserType);
        }

        public int GetHashCode(StartOption option)
        {
            if (option == null)
            {
                return 0;
            }

            unchecked
            {
                return option.LongName.GetHashCode() * option.ShortName.GetHashCode() * option.Description.GetHashCode()
                     * option.IsMandatory.GetHashCode() * option.ValueType.GetHashCode() * (option.ParserType?.FullName ?? String.Empty).GetHashCode();
            }
        }
    }
}
