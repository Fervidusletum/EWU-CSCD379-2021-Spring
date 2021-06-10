using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SecretSanta.Web.Api;

namespace SecretSanta.Web.Tests.Api
{
    internal class TestableUsersClient : IUsersClient
    {
        private Dictionary<int, User> Users { get; } = new();

        public int DeleteInvocationCount { get; set; }
        public Task DeleteAsync(int id)
        {
            DeleteInvocationCount++;
            Users.Remove(id);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public int GetAllInvocationCount { get; set; }
        public Task<ICollection<User>> GetAllAsync()
        {
            GetAllInvocationCount++;
            return Task.FromResult<ICollection<User>>(Users.Values);
        }

        public Task<ICollection<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public int GetInvocationCount { get; set; }
        public Task<User?> GetAsync(int id)
        {
            GetInvocationCount++;
            Users.TryGetValue(id, out User? user);
            return Task.FromResult(user);
        }

        public Task<User> GetAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public int PostInvocationCount { get; set; }
        public Task<User> PostAsync(User user)
        {
            PostInvocationCount++;
            Users.Add(user.Id, user);
            return Task.FromResult(user);
        }

        public Task<User> PostAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public int PutInvocationCount { get; set; }
        public Task PutAsync(int id, UpdateUser updateUser)
        {
            PutInvocationCount++;
            if (Users.TryGetValue(id, out User? user))
            {
                user.FirstName = updateUser.FirstName;
                user.LastName = updateUser.LastName;
            }
            return Task.CompletedTask;
        }

        public Task PutAsync(int id, UpdateUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Assignment>> GetAssignmentsAsync(int userid)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Assignment>> GetAssignmentsAsync(int userid, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
