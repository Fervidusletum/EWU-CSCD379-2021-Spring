using System.Collections.Generic;
using SecretSanta.Web.ViewModels;

namespace SecretSanta.Web.Data
{
    public static class MockData
    {
        public static List<UserViewModel> Users = new List<UserViewModel>
        {
            new UserViewModel { Id = 1, FirstName = "Inigo", LastName = "Montoya" },
            new UserViewModel { Id = 2, FirstName = "Princess", LastName = "Buttercup" },
            new UserViewModel { Id = 3, FirstName = "Prince", LastName = "Humperdink" },
            new UserViewModel { Id = 4, FirstName = "Count", LastName = "Rugen" },
            new UserViewModel { Id = 5, FirstName = "Miracle", LastName = "Max" },
        };

        public static List<GroupViewModel> Groups = new List<GroupViewModel>
        {
            new GroupViewModel { Id = 1, Name = "GoonSquad" },
            new GroupViewModel { Id = 2, Name = "Rodents Of Unusual Size" },
        };

        public static List<GiftViewModel> Gifts = new List<GiftViewModel>
        {
            new GiftViewModel { Id = 1, Title = "Drone", Description = "A fun little way to get a birds eye view", Url="https://www.google.com", Priority = 2, UserId = 1 },
            new GiftViewModel { Id = 2, Title = "Rat Poison", Description = "Hopefully a way to get rid of those unusual size rodents", Url="https://www.google.com", Priority = 1, UserId = 1 },
            new GiftViewModel { Id = 3, Title = "Drone", Description = "A fun little way to get a birds eye view", Url="https://www.google.com", Priority = 2, UserId = 2 },
            new GiftViewModel { Id = 4, Title = "Rat Poison", Description = "Hopefully a way to get rid of those unusual size rodents", Url="https://www.google.com", Priority = 1, UserId = 2 },
        };
    }
}
