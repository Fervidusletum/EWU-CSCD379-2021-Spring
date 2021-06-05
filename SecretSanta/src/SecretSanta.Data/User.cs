using System.Collections.Generic;

namespace SecretSanta.Data
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public List<Group> UserGroups { get; } = new();
        public List<Gift> UserGifts { get; } = new();
    }
}
