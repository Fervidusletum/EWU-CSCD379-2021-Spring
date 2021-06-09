using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretSanta.Api.Dto
{
    public class Gift
    {
        public int Id { get; set; }
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; } = "";
        public string? Url { get; set; } = "";
        public int Priority { get; set; }

        public static Gift? ToDto(Data.Gift? gift)
        {
            if (gift is null) return null;

            return new Gift
            {
                Id = gift.Id,
                ReceiverId = gift.ReceiverId,
                Receiver = User.ToDto(gift.Receiver),
                Title = gift.Title,
                Description = gift.Description,
                Url = gift.Url,
                Priority = gift.Priority
            };
        }

        public static Data.Gift? FromDto(Gift? gift)
        {
            if (gift is null) return null;

            return new Data.Gift
            {
                Id = gift.Id,
                ReceiverId = gift.ReceiverId,
                Receiver = User.FromDto(gift.Receiver),
                Title = gift.Title ?? "",
                Description = gift.Description,
                Url = gift.Url,
                Priority = gift.Priority
            };
        }
    }
}
