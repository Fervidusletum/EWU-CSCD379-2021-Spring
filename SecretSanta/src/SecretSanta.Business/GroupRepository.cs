using System;
using System.Collections.Generic;
using SecretSanta.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DbContext = SecretSanta.Data.DbContext;

namespace SecretSanta.Business
{
    public class GroupRepository : IGroupRepository
    {
        private DbContext Context { get; }

        public GroupRepository(DbContext context)
            => Context = context ?? throw new ArgumentNullException(nameof(context));

        public Group Create(Group item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Context.Groups.Add(item);
            Context.SaveChanges();
            return item;
        }

        // https://docs.microsoft.com/en-us/ef/core/querying/related-data/eager#eager-loading
        public Group? GetItem(int id)
            => List().FirstOrDefault<Group>(g => g.Id == id);

        public ICollection<Group> List()
            => Context.Groups
                .Include(group => group.Users)
                .ThenInclude(user => user.Gifts)
                .Include(group => group.Assignments)
                .ToList();

        public bool Remove(int id)
        {
            Group? group = GetItem(id);
            if (group is null) return false;

            Context.Groups.Remove(group);
            Context.SaveChanges();
            return true;
        }

        public void Save(Group item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Context.Groups.Update(item);
            Context.SaveChanges();
        }

        public AssignmentResult GenerateAssignments(int groupId)
        {
            Group? group = GetItem(groupId);
            if (group is null) return AssignmentResult.Error("Group not found");

            Random random = new();
            var groupUsers = new List<User>(group.Users);

            if (groupUsers.Count < 3)
            {
                return AssignmentResult.Error($"Group {group.Name} must have at least three users");
            }

            var users = new List<User>();
            //Put the users in a random order
            while(groupUsers.Count > 0)
            {
                int index = random.Next(groupUsers.Count);
                users.Add(groupUsers[index]);
                groupUsers.RemoveAt(index);
            }

            //The assignments are created by linking the current user to the next user.
            group.Assignments.Clear();
            for(int i = 0; i < users.Count; i++)
            {
                int endIndex = (i + 1) % users.Count;
                group.Assignments.Add(new() { Giver = users[i], Receiver = users[endIndex] });
            }

            Save(group);
            return AssignmentResult.Success();
        }
    }
}
