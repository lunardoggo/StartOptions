using System.Collections.Generic;
using System.Linq;
using System;

namespace StartOptions.Tests.Mocks.Dependencies
{
    public interface IDatabase
    {
        public IEnumerable<User> GetUsers();
        public void RemoveUser(Guid id);
        public void AddUser(User user);
        public User GetUser(Guid id);
    }

    public class MockDatabase : IDatabase
    {
        private List<User> users = new List<User>();

        public MockDatabase()
        {
            this.users.Add(new User() { Id = Guid.NewGuid(), Username = "john.doe", DisplayName = "John Doe" } );
            this.users.Add(new User() { Id = Guid.NewGuid(), Username = "evie.bishop", DisplayName = "Evie Bishop" } );
            this.users.Add(new User() { Id = Guid.NewGuid(), Username = "sophia.lawson", DisplayName = "Sophia Lawson" } );
            //These names were picked at random from a list of some random names :)
        }

        public IEnumerable<User> GetUsers()
        {
            return this.users.AsEnumerable();
        }

        public void RemoveUser(Guid id)
        {
            User user = this.GetUser(id);
            if (user != null)
            {
                this.users.Remove(user);
            }
        }

        public void AddUser(User user)
        {
            this.users.Add(user);
        }

        public User GetUser(Guid id)
        {
            return this.users.SingleOrDefault(_user => _user.Id == id);
        }
    }

    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
    }
}
