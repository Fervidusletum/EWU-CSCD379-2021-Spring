using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecretSanta.Web.ViewModels;

namespace SecretSanta.Web.Data
{
    public class MockData
    {
        private static int _NextId = 0;
        private static int GenerateId { get => _NextId++; } //return then increment

        public static void AddUser(UserViewModel user)
        {
            user.Id = GenerateId;
            Users.Add(user);
        }

        public static UserViewModel GetUser(int id) => Users.Find(user => user.Id==id);

        public static void UpdateUser(UserViewModel userupdate)
            => Users[Users.FindIndex(user => user.Id == userupdate.Id)] = userupdate;

        public static void DeleteUser(int id)
        {
            Gifts.RemoveAll(gift => gift.UserId == id);
            Users.RemoveAll(user => user.Id == id);
        }

        public static List<UserViewModel> Users = new()
        {
            new UserViewModel { Id = GenerateId, FirstName = "Mario", LastName = "Mario" },
            new UserViewModel { Id = GenerateId, FirstName = "Luigi", LastName = "Mario" }
        };



        public static void AddGift(GiftViewModel gift)
        {
            gift.Id = GenerateId;
            Gifts.Add(gift);
        }

        public static GiftViewModel GetGift(int id) => Gifts.Find(gift => gift.Id == id);

        public static void UpdateGift(GiftViewModel giftupdate)
            => Gifts[Gifts.FindIndex(gift => gift.Id == giftupdate.Id)] = giftupdate;

        public static void DeleteGift(int id) => Gifts.RemoveAll(gift => gift.Id == id);

        public static List<GiftViewModel> Gifts = new()
        {
            new GiftViewModel { Id = GenerateId, UserId = Users[1].Id, Title = "Star", Description = "Grants temporary invincibility.", Priority = 0, URL = "" },
            new GiftViewModel { Id = GenerateId, UserId = Users[0].Id, Title = "Fire Flower", Description = "Spits fireballs.", Priority = 1, URL = "http:\\\\www.somewebsitethatsellsfireflowers.com" }
        };



        public static void AddGroup(GroupViewModel group)
        {
            group.Id = GenerateId;
            Groups.Add(group);
        }

        public static GroupViewModel GetGroup(int id) => Groups.Find(group => group.Id == id);

        public static void UpdateGroup(GroupViewModel groupupdate)
            => Groups[Groups.FindIndex(group => group.Id == groupupdate.Id)] = groupupdate;

        public static void DeleteGroup(int id) => Groups.RemoveAll(group => group.Id == id);

        public static List<GroupViewModel> Groups = new()
        {
            new GroupViewModel { Id = GenerateId, Name = "SMB" }
        };
    }
}
