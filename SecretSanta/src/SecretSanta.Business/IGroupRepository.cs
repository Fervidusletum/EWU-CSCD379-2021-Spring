using System.Collections.Generic;
using System.Threading.Tasks;
using SecretSanta.Data;

namespace SecretSanta.Business
{
    public interface IGroupRepository
    {
        ICollection<Group> List();
        Group? GetItem(int id);
        Task<Group?> GetItemAsync(int id);
        bool Remove(int id);
        Group Create(Group item);
        void Save(Group item);
        AssignmentResult GenerateAssignments(int groupId);
    }

}
