using System.Collections.Generic;
using SecretSanta.Data;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using DbContext = SecretSanta.Data.DbContext;

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

        // https://docs.microsoft.com/en-us/ef/core/querying/related-data/eager#eager-loading
        public User? GetItem(int id)
            => List().FirstOrDefault<User>(u => u.Id == id);

        public ICollection<User> List()
            => Context.Users
                .Include(user => user.Gifts)
                .ToList();

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
