using LunarDoggo.StartOptions.DependencyInjection;
using StartOptions.Tests.Mocks.Dependencies;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;
using Xunit;

namespace StartOptions.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void TestGetServicesExceptions()
        {
            SimpleDependencyProvider provider = new SimpleDependencyProvider(true);
            Assert.Throws<KeyNotFoundException>(() => provider.GetDependency<string>());
            Assert.Throws<ArgumentNullException>(() => provider.AddSingleton<object>(null));

            provider.AddSingleton("value");
            Assert.Throws<ArgumentException>(() => provider.AddSingleton("abc"));

            provider = new SimpleDependencyProvider(false);
            Assert.Null(provider.GetDependency<object>());
        }

        [Fact]
        public void TestGetServices()
        {
            IDependencyProvider provider = this.GetDependencyProvider();

            IPAddress loopback = provider.GetDependency<IPAddress>();
            IDatabase database = provider.GetDependency<IDatabase>();
            string value = provider.GetDependency<string>();

            Assert.Equal(IPAddress.Loopback, loopback);
            Assert.Equal("value", value);

            Assert.NotNull(database);
            Assert.Equal(3, database.GetUsers().Count());
        }

        private IDependencyProvider GetDependencyProvider()
        {
            SimpleDependencyProvider provider = new SimpleDependencyProvider(false);
            provider.AddSingleton<IDatabase>(new MockDatabase());
            provider.AddSingleton(IPAddress.Loopback);
            provider.AddSingleton("value");
            return provider;
        }
    }
}
