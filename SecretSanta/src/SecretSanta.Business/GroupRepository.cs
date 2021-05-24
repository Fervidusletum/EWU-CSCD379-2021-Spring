using System;
using System.Collections.Generic;
using SecretSanta.Data;
using System.Linq;
using MoreLinq;

namespace SecretSanta.Business
{
    public class GroupRepository : IGroupRepository
    {
        public Group Create(Group item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            MockData.Groups[item.Id] = item;
            return item;
        }

        public Group? GetItem(int id)
        {
            if (MockData.Groups.TryGetValue(id, out Group? group))
            {
                return group;
            }
            return null;
        }

        public ICollection<Group> List()
        {
            return MockData.Groups.Values;
        }

        public bool Remove(int id)
        {
            return MockData.Groups.Remove(id);
        }

        public void Save(Group item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            MockData.Groups[item.Id] = item;
        }

        public AssignmentResult GenerateAssignments(int id)
        {
            Group? group = GetItem(id);
            if (group is null) throw new ArgumentNullException(nameof(id));
            if (group.Users.Count < 3) return AssignmentResult.Error("Need at least 3 users in the group to generate assignments");

            group.Assignments.Clear();

            List<User> shuffled = group.Users.Shuffle().ToList();
            shuffled.Add(shuffled.First()); // add first user to end too, so we dont need extra circular link logic

            for (int i = 0; i < shuffled.Count-1; i++)
            {
                group.Assignments.Add(new Assignment(shuffled[i], shuffled[i+1]));
            }

            return AssignmentResult.Success();
        }
    }
}
