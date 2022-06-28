using LunarDoggo.StartOptions;
using System;

namespace StartOptions.Tests.Mocks.Commands
{
    public class SearchCommand : IApplicationCommand
    {
        [StartOptionGroup("search", "s", ValueType = StartOptionValueType.Single, IsValueMandatory = false)]
        public SearchCommand([StartOptionGroupValue] string searchTerm)
        {
            this.SearchTerm = searchTerm;
        }

        public string SearchTerm { get; }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
