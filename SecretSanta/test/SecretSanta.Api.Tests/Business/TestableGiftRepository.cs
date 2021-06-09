using System.Collections.Generic;
using SecretSanta.Business;
using SecretSanta.Data;
using System.Linq;

namespace SecretSanta.Api.Tests.Business
{
    class TestableGiftRepository : IGiftRepository
    {
        private Dictionary<int, Gift> Gifts { get; } = new();

        public Gift Create(Gift item)
        {
            Gifts.Add(item.Id, item);
            return item;
        }

        public Gift? GetItem(int id)
        {
            Gifts.TryGetValue(id, out Gift? rv);
            return rv;
        }

        public ICollection<Gift> List() => Gifts.Values;

        public ICollection<Gift> List(int userId)
        {
            return Gifts.Values.Where(g => g.ReceiverId == userId).ToList();
        }

        public bool Remove(int id) => Gifts.Remove(id);

        public void Save(Gift item) => Gifts[item.Id] = item;
    }
}
