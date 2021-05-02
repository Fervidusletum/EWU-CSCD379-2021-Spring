using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretSanta.Api.Dto
{
    public class UserDtoFull
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; } = "";
        public string? LastName { get; set; } = "";
    }
}
