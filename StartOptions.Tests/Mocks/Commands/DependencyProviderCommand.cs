using StartOptions.Tests.Mocks.Dependencies;
using LunarDoggo.StartOptions;
using System;

namespace StartOptions.Tests.Mocks.Commands
{
    public class DependencyProviderCommand : IApplicationCommand
    {
        private readonly string username, displayName;
        private readonly IDatabase database;

        [StartOptionGroup("addUser", "a")]
        public DependencyProviderCommand([StartOption("username", "u", ValueType = StartOptionValueType.Single)]string username,
                                         [StartOption("displayName", "d", ValueType = StartOptionValueType.Single)]string displayName,
                                         IDatabase database)
        {
            this.displayName = displayName;
            this.username = username;
            this.database = database;
        }

        public void Execute()
        {
            this.database.AddUser(new User() { Id = Guid.NewGuid(), Username = this.username, DisplayName = this.displayName });
        }
    }
}
