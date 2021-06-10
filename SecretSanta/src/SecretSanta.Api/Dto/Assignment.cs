using System.Collections.Generic;
using System.Linq;

namespace SecretSanta.Api.Dto
{
    public class Assignment
    {
        public Group? ForGroup { get; set; }
        public User? Giver { get; set; }
        public User? Receiver { get; set; }
        public List<Gift?> RequestedGifts { get; set; }

        public static Assignment? ToDto(Data.Assignment? assignment)
        {
            if (assignment is null) return null;
            return new Assignment
            {
                ForGroup = Group.ToDto(assignment.Group),
                Giver = User.ToDto(assignment.Giver),
                Receiver = User.ToDto(assignment.Receiver),
                RequestedGifts = assignment.Receiver.Gifts
                    .Select(g => Gift.ToDto(g))
                    .ToList()
            };
        }
    }
}
