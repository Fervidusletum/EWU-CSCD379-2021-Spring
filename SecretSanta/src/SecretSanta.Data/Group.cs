using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SecretSanta.Data
{
    [Index(nameof(Name), IsUnique = true)]
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public List<User> Users { get; } = new();
        public List<Assignment> Assignments { get; } = new();
    }
}
