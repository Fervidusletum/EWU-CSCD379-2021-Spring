using System.Collections.Generic;

namespace SecretSanta.Api.Dto
{
    public class Group
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<User> Users { get; } = new();
        public List<Assignment> Assignments { get; } = new();

        public static Group? ToDto(Data.Group? group, bool includeChildObjects = false)
        {
            if (group is null) return null;
            var rv = new Group
            {
                Id = group.Id,
                Name = group.Name
            };
            if (includeChildObjects)
            {
                foreach(Data.User? user in group.Users)
                {
                    if (User.ToDto(user) is { } dtoUser)
                    {
                        rv.Users.Add(dtoUser);
                    }
                }
                foreach(Data.Assignment? assignent in group.Assignments)
                {
                    if (Assignment.ToDto(assignent) is { } dtoAssignment)
                    {
                        rv.Assignments.Add(dtoAssignment);
                    }
                }
            }
            return rv;
        }

        public static Data.Group? FromDto(Group? group)
        {
            if (group is null) return null;
            return new Data.Group
            {
                Id = group.Id,
                Name = group.Name ?? "",
            };
        }
    }
}
