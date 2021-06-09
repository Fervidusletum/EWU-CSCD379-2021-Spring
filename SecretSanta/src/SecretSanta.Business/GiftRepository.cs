using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecretSanta.Data;
using DbContext = SecretSanta.Data.DbContext;

namespace SecretSanta.Business
{
    public class GiftRepository : IGiftRepository
    {
        private DbContext Context { get; }

        public GiftRepository(DbContext dbContext)
            => Context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public Gift Create(Gift item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            Context.Gifts.Add(item);
            Context.SaveChanges();
            return item;
        }

        public Gift? GetItem(int id)
            => List().FirstOrDefault<Gift>(g => g.Id == id);

        public ICollection<Gift> List()
            => Context.Gifts.ToList();

        public ICollection<Gift> List(int userId)
            => Context.Gifts
                .Where<Gift>(g => g.ReceiverId == userId)
                .ToList();

        public bool Remove(int id)
        {
            Gift? gift = GetItem(id);
            if (gift is null) return false;

            Context.Gifts.Remove(gift);
            Context.SaveChanges();
            return true;
        }

        public void Save(Gift item)
        {
            if (item is null) throw new System.ArgumentNullException(nameof(item));

            Context.Gifts.Update(item);
            Context.SaveChanges();
        }
    }
}
