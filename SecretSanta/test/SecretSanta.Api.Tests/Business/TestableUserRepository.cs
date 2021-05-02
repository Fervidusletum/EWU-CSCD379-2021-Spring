using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSanta.Business;
using SecretSanta.Data;

namespace SecretSanta.Api.Tests.Business
{
    public class TestableUserRepository : IUserRepository
    {
        public User Create(User item)
            => throw new NotImplementedException();

        public User? GetItem(int id)
            => throw new NotImplementedException();

        public ICollection<User> List()
            => throw new NotImplementedException();

        public bool Remove(int id)
            => throw new NotImplementedException();

        public void Save(User item)
            => throw new NotImplementedException();
    }
}
