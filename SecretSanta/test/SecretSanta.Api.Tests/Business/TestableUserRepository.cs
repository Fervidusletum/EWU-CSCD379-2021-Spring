using System.Collections.Generic;
using SecretSanta.Business;
using SecretSanta.Data;

namespace SecretSanta.Api.Tests.Business
{
    public class TestableUserRepository : IUserRepository
    {
        public User? CreateParamItem { get; set; }
        public User? CreateReturnUser { get; set; }
        public User Create(User item)
        {
            CreateParamItem = item;
            return CreateReturnUser!;
        }

        public int GetItemParamId { get; set; }
        public User? GetItemReturnUser { get; set; }
        public User? GetItem(int id)
        {
            GetItemParamId = id;
            return GetItemReturnUser;
        }

        public List<User?> ListReturnUserCollection { get; } = new();
        public ICollection<User> List()
            => ListReturnUserCollection!;

        public int RemoveParamId { get; set; }
        public bool RemoveReturnBool { get; set; }
        public bool Remove(int id)
        {
            RemoveParamId = id;
            return RemoveReturnBool;
        }

        public User? SaveParamItem { get; set; }
        public void Save(User item)
            => SaveParamItem = item;
    }
}
