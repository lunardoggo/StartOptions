using LunarDoggo.StartOptions;

namespace StartOptions.Tests.Mocks.Commands
{
    public class GrouplessOptionReferenceCommand : IApplicationCommand
    {
        [StartOptionGroup("group", "grp")]
        public GrouplessOptionReferenceCommand([StartOption("option", "opt")] bool option,
                                               [GrouplessStartOptionReference("verbose")] bool verbose)
        {
            this.Verbose = verbose;
        }

        public void Execute()
        { }

        public bool Verbose { get; }
    }
}
