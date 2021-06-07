using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SecretSanta.Data
{
    [Index(nameof(FirstName), nameof(LastName))]
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public List<Group> Groups { get; } = new();
        public List<Gift> Gifts { get; } = new();
    }
}
