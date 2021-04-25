using System;
using System.Collections.Generic;
using System.Linq;
using SecretSanta.Data;

namespace SecretSanta.Business
{
    public class UserRepository : IUserRepository
    {
        public ICollection<User> List() => MockData.Users;

        public User? GetItem(int id) => MockData.Users.FirstOrDefault(user => user.Id == id);

        public User Create(User newUser)
        {
            MockData.Users.Add(newUser);
            return newUser;
        }

        public bool Remove(int id)
        {
            User? foundUser = GetItem(id);
            if (foundUser is null) return false;

            MockData.Users.Remove(foundUser);
            return true;
        }

        public void Save(User user)
        {
            Remove(user.Id);
            Create(user);
        }
    }
}
