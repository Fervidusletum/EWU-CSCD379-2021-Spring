using System.Collections.Generic;

namespace SecretSanta.Data
{
    public class TestCollection : List<User>
    {
        public TestCollection()
        {
            Add(new User() { Id = 0, FirstName = "Mario", LastName = "Mario" });
            Add(new User() { Id = 1, FirstName = "Luigi", LastName = "Mario" });
            Add(new User() { Id = 2, FirstName = "Princess", LastName = "Peach" });
        }
    }
}
