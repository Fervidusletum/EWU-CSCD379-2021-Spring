using System.Collections.Generic;
using SecretSanta.Data;
using System.Linq;
using System;

namespace SecretSanta.Business
{
    public class UserRepository : IUserRepository
    {
        private DbContext Context { get; }

        public UserRepository(DbContext context)
            => Context = context ?? throw new ArgumentNullException(nameof(context));

        public User Create(User item)
        {
            if (item is null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            Context.Users.Add(item);
            Context.SaveChanges();
            return item;
        }

        public User? GetItem(int id)
        {
            return Context.Users.FirstOrDefault<User>(u => u.Id == id);
        }

        public ICollection<User> List()
        {
            return Context.Users.ToList();
        }

        public bool Remove(int id)
        {
            User? user = GetItem(id);
            if (user is null) return false;

            Context.Users.Remove(user);
            Context.SaveChanges();
            return true;
        }

        public void Save(User item)
        {
            if (item is null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            Context.Users.Update(item);
            Context.SaveChanges();
        }
    }
}
