using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SecretSanta.Data
{
    [Index(nameof(GroupId), nameof(GiverId), nameof(ReceiverId))]
    public class Assignment
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int GiverId { get; set; }
        public User Giver { get; set; }
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }
    }
}
