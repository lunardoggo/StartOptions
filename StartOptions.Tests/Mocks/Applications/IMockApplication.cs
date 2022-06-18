using System.Collections.Generic;
using LunarDoggo.StartOptions;

namespace StartOptions.Tests.Mocks.Applications
{
    public interface IMockApplication
    {
        IEnumerable<StartOption> GrouplessStartOptions { get; }
        StartOptionGroup Group { get; }

        void Run(string[] args);
    }
}