using System;

namespace SecretSanta.Data
{
    public class Assignment
    {
        public int Id { get; set; }

        public User Giver { get; private set; }

        public User Receiver { get; private set; }

        // making EF happy
        private Assignment() { throw new NotSupportedException(nameof(Assignment)+"()"); }

        public Assignment(User giver, User receiver)
        {
            Giver = giver ?? throw new ArgumentNullException(nameof(giver));
            Receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }
    }
}
