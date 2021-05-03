using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SecretSanta.Web.Api;

namespace SecretSanta.Web.Tests.Api
{
    public class TestableUsersClient : IUsersClient
    {
        public int GetAllAsyncInvokeCount { get; set; }
        public List<UserDtoFull> GetAllAsyncReturnUserList { get; } = new();
        public Task<ICollection<UserDtoFull>> GetAllAsync()
        {
            GetAllAsyncInvokeCount++;
            return Task.FromResult<ICollection<UserDtoFull>>(GetAllAsyncReturnUserList);
        }

        public int GetAsyncInvokeCount { get; set; }
        public UserDtoFnLn GetAsyncReturnUser { get; set; }
        public Task<UserDtoFnLn> GetAsync(int id)
        {
            GetAsyncInvokeCount++;
            return Task.FromResult<UserDtoFnLn>(GetAsyncReturnUser);
        }

        public int PostAsyncInvokeCount { get; set; }
        public List<UserDtoFull> PostAsyncInvokeParams { get; } = new();
        public Task<UserDtoFull> PostAsync(UserDtoFull user)
        {
            PostAsyncInvokeCount++;
            PostAsyncInvokeParams.Add(user);
            return Task.FromResult(user);
        }

        public int PutAsyncInvokeCount { get; set; }
        public Task PutAsync(int id, UserDtoFnLn user)
        {
            PutAsyncInvokeCount++;
            return Task.CompletedTask;
        }

        public int DeleteAsyncInvokeCount { get; set; }
        public Task DeleteAsync(int id)
        {
            DeleteAsyncInvokeCount++;
            return Task.CompletedTask;
        }

        public Task<ICollection<UserDtoFull>> GetAllAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<UserDtoFnLn> GetAsync(int id, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<UserDtoFull> PostAsync(UserDtoFull user, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task PutAsync(int id, UserDtoFnLn user, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task DeleteAsync(int id, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
