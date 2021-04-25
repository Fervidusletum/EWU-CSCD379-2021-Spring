using System;
using System.Collections.Generic;
using System.Linq;

namespace SecretSanta.Data
{
    public static class MockData
    {
        public static List<User> Users { get; } = new()
        {
            new User() { Id = 1, FirstName = "Mario", LastName = "Mario" },
            new User() { Id = 2, FirstName = "Luigi", LastName = "Mario" },
            new User() { Id = 3, FirstName = "Princess", LastName = "Peach" }
        };
    }
}
