using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SecretSanta.Data
{
    [Index(nameof(Title), nameof(ReceiverId))]
    public class Gift
    {
        public int Id { get; set; }
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; } = "";
        public string? Url { get; set; } = "";
        public int Priority { get; set; }
    }
}
