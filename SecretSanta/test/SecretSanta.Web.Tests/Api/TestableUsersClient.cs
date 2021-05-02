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
        public Task DeleteAsync(int id)
            => throw new NotImplementedException();

        public Task DeleteAsync(int id, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task<ICollection<UserDtoFull>> GetAllAsync()
            => throw new NotImplementedException();

        public Task<ICollection<UserDtoFull>> GetAllAsync(CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task<UserDtoFnLn> GetAsync(int id)
            => throw new NotImplementedException();

        public Task<UserDtoFnLn> GetAsync(int id, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task<UserDtoFull> PostAsync(UserDtoFull user)
            => throw new NotImplementedException();

        public Task<UserDtoFull> PostAsync(UserDtoFull user, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task PutAsync(int id, UserDtoFnLn user)
            => throw new NotImplementedException();

        public Task PutAsync(int id, UserDtoFnLn user, CancellationToken cancellationToken)
            => throw new NotImplementedException();
    }
}
