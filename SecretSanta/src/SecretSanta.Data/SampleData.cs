using System;
using System.Collections.Generic;

namespace SecretSanta.Data
{
    public static class SampleData
    {
        public static List<Group> CreateSampleData()
        {
            Group groupParty = new Group()
            {
                Name = "IntelliTect Christmas Party"
            };

            List<User> usersParty = new()
            {
                new User
                {
                    FirstName = "Inigo",
                    LastName = "Montoya"
                },
                new User
                {
                    FirstName = "Princess",
                    LastName = "Buttercup"
                },
                new User
                {
                    FirstName = "Prince",
                    LastName = "Humperdink"
                }
            };

            foreach(User u in usersParty)
            {
                u.Gifts.Add(new Gift
                {
                    Title = $"Gift {Guid.NewGuid()}",
                    Priority = 1
                });
            }
            groupParty.Users.AddRange(usersParty);

            groupParty.Assignments.AddRange(new List<Assignment> {
                new Assignment { Giver = usersParty[0], Receiver = usersParty[1] },
                new Assignment { Giver = usersParty[1], Receiver = usersParty[2] },
                new Assignment { Giver = usersParty[2], Receiver = usersParty[0] }
            });


            Group groupFriends = new()
            {
                Name = "Friends"
            };

            List<User> usersFriends = new()
            {
                new User
                {
                    FirstName = "Count",
                    LastName = "Rugen"
                },
                new User
                {
                    FirstName = "Miracle",
                    LastName = "Max"
                }
            };

            foreach (User u in usersFriends)
            {
                u.Gifts.Add(new Gift
                {
                    Title = $"Gift {Guid.NewGuid()}",
                    Priority = 1
                });
            }
            groupFriends.Users.AddRange(usersFriends);

            return new List<Group>() { groupParty, groupFriends };
        }
    }
}
