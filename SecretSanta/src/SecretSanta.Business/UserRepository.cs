using System;
using System.Collections.Generic;
using System.Linq;
using SecretSanta.Data;

namespace SecretSanta.Business
{
    public class UserRepository : IUserRepository
    {
        private ICollection<User> Users { get; }
        public UserRepository(ICollection<User> userCollection)
            => Users = userCollection ?? throw new ArgumentNullException(nameof(userCollection));

        public ICollection<User> List() => Users;

        public User? GetItem(int id) => Users.FirstOrDefault(user => user.Id == id);

        public User Create(User newUser)
        {
            if (newUser is null) throw new ArgumentNullException(nameof(newUser));

            Users.Add(newUser);
            return newUser;
        }

        public bool Remove(int id)
        {
            User? foundUser = GetItem(id);
            if (foundUser is null) return false;

            Users.Remove(foundUser);
            return true;
        }

        public void Save(User user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            Remove(user.Id);
            Create(user);
        }
    }
}
