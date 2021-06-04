using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Data
{
    public class Gift
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; } = "";
        public string? Url { get; set; } = "";
        public int Priority { get; set; }
        public int UserId { get; set; }
    }
}
