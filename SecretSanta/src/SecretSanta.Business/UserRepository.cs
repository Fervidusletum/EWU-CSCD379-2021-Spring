using System.Collections.Generic;
using SecretSanta.Data;
using System.Linq;

namespace SecretSanta.Business
{
    public class UserRepository : IUserRepository
    {
        private DbContext Context { get; }

        public UserRepository(DbContext context)
            => Context = context;

        public User Create(User item)
        {
            if (item is null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            MockData.Users[item.Id] = item;
            return item;
        }

        public User? GetItem(int id)
        {
            /*
            if (MockData.Users.TryGetValue(id, out User? user))
            {
                return user;
            }
            return null;
            */
            return Context.Users.FirstOrDefault<User>(u => u.Id == id);
        }

        public ICollection<User> List()
        {
            //return MockData.Users.Values;
            return Context.Users.ToList();
        }

        public bool Remove(int id)
        {
            return MockData.Users.Remove(id);
        }

        public void Save(User item)
        {
            if (item is null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            MockData.Users[item.Id] = item;
        }
    }
}
