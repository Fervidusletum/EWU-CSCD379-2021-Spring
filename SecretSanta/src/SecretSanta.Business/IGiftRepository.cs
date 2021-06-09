using System.Collections.Generic;
using SecretSanta.Data;

namespace SecretSanta.Business
{
    public interface IGiftRepository
    {
        ICollection<Gift> List();
        ICollection<Gift> List(int userId);
        Gift? GetItem(int id);
        bool Remove(int id);
        Gift Create(Gift item);
        void Save(Gift item);
    }
}
