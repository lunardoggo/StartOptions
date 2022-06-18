using LunarDoggo.StartOptions;

namespace StartOptions.Tests.Mocks.Commands
{
    public class DuplicateAttributeCommand : IApplicationCommand
    {
        [StartOptionGroup("duplicate", "d")]
        public DuplicateAttributeCommand([GrouplessStartOption("groupless1", "g1")][GrouplessStartOption("groupless2", "g2")]bool isSet)
        {}

        public void Execute()
        { }
    }
}