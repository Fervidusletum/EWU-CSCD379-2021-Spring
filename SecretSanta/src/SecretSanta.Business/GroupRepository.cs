using System;
using System.Collections.Generic;
using SecretSanta.Data;
using System.Linq;
using MoreLinq;

namespace SecretSanta.Business
{
    public class GroupRepository : IGroupRepository
    {
        public delegate List<User> Shuffler(ICollection<User> list);
        private Shuffler ShuffleList { get; }
        public GroupRepository() : this(null) { }
        public GroupRepository(Shuffler? shuffle)
            => ShuffleList = shuffle ?? ( (ICollection<User> list) => list.Shuffle().ToList() );

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
            if (group?.Users is null) return AssignmentResult.Error("Group not found");
            if (group.Users.Count < 3) return AssignmentResult.Error($"Group {group?.Name} must have at least three users");

            group.Assignments.Clear();

            
            List<User> shuffled = ShuffleList(group.Users);
            shuffled.Add(shuffled.First()); // add first user to end too, so we dont need extra circular link logic
            
            for (int i = 0; i < shuffled.Count-1; i++)
            {
                // should not be possible to get giver==receiver unless there's duplicate entries
                // probably should be handled in add, but I'll add a check here for the assignment requirement
                // cant assume people with the same name are the same person, so just checking id
                if (shuffled[i].Id == shuffled[i + 1].Id) continue;

                group.Assignments.Add(new Assignment(shuffled[i], shuffled[i+1]));
            }

            return AssignmentResult.Success();
        }
    }
}
