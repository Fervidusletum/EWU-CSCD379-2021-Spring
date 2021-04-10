using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecretSanta.Web.ViewModels;

namespace SecretSanta.Web.Data
{
    public class MockData
    {
        public static List<UserViewModel> Users = new()
        {
            new UserViewModel { Id = 0, FirstName = "Mario", LastName = "Mario" },
            new UserViewModel { Id = 1, FirstName = "Luigi", LastName = "Mario" }
        };

        public static List<GroupViewModel> Groups = new()
        {
            new GroupViewModel { Id = 0, Name = "SMB" }
        };

        public static List<GiftViewModel> Gifts = new()
        {
            new GiftViewModel { Id = 0, Title = "Star", Description = "Grants temporary invincibility.", Priority = 0, URL="" },
            new GiftViewModel { Id = 0, Title = "Fire Flower", Description = "Spits fireballs.", Priority = 1, URL = "www.somewebsitethatsellsfireflowers.com" }
        };
    }
}
