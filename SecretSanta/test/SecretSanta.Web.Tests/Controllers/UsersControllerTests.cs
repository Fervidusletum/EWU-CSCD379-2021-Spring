using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Web.Api;
using SecretSanta.Web.Tests.Api;
using SecretSanta.Web.ViewModels;

namespace SecretSanta.Web.Tests
{
    [TestClass]
    public class UsersControllerTests
    {
        private WebApplicationFactory Factory { get; } = new();


        #region INDEX() TESTS
        [TestMethod]
        public async Task Index_WithUsers_InvokesGetAllAsync()
        {
            TestableUsersClient usersClient = Factory.Client;
            usersClient.GetAllAsyncReturnUserList.Add(
                new UserDtoFull { Id = 0, FirstName = "F1", LastName = "L1" });
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync(new Uri("/users/", UriKind.Relative));

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(1, usersClient.GetAllAsyncInvokeCount);
        }

        [TestMethod]
        public async Task Index_GivenANullDto_DoesntExplode()
        {
            TestableUsersClient usersClient = Factory.Client;
            usersClient.GetAllAsyncReturnUserList.Add(null!);
            HttpClient client = Factory.CreateClient();

            await client.GetAsync(new Uri("/users/", UriKind.Relative));

            return; // successful return is a passed assert
        }

        [TestMethod]
        public async Task Index_GivenANullIndex_DoesntExplode()
        {
            TestableUsersClient usersClient = Factory.Client;
            usersClient.GetAllAsyncReturnUserList.Add(
                new UserDtoFull { Id = null!, FirstName = "F1", LastName = "L1" });
            HttpClient client = Factory.CreateClient();

            await client.GetAsync(new Uri("/users/", UriKind.Relative));

            return; // successful return is a passed assert
        }
        #endregion INDEX() TESTS


        #region CREATE(USERVIEWMODEL) TESTS
        [TestMethod]
        public async Task Create_GivenValidModel_InvokesPostAsync()
        {
            TestableUsersClient usersClient = Factory.Client;
            HttpClient client = Factory.CreateClient();
            Dictionary<string, string?> values = new() { { nameof(UserViewModel.Id), "0" } };
            
            HttpResponseMessage response = await client.PostAsync(new Uri("/users/create/", UriKind.Relative),
                new FormUrlEncodedContent(values));

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(1, usersClient.PostAsyncInvokeCount, "Incorrect number of invocations.");
            Assert.IsTrue(usersClient.PostAsyncParams.Any(), "No params passed in.");
        }

        [TestMethod]
        public async Task Create_GivenValidModel_SendsValidDtos()
        {
            TestableUsersClient usersClient = Factory.Client;
            HttpClient client = Factory.CreateClient();
            Dictionary<string, string?> values = new()
            {
                { nameof(UserViewModel.Id), "0" },
                { nameof(UserViewModel.FirstName), "f" },
                { nameof(UserViewModel.LastName), "l" }
            };

            HttpResponseMessage response = await client.PostAsync(new Uri("/users/create/", UriKind.Relative),
                new FormUrlEncodedContent(values));

            response.EnsureSuccessStatusCode();
            UserDtoFull? result = usersClient.PostAsyncParams.FirstOrDefault();
            Assert.IsNotNull(result, "No param received.");
            Assert.AreEqual<int>(int.Parse(values["Id"]!), result!.Id ?? -1);
            Assert.AreEqual<string>(values["FirstName"]!, result!.FirstName);
            Assert.AreEqual<string>(values["LastName"]!, result!.LastName);
        }

        [TestMethod]
        public async Task Create_GivenInvalidModel_DoesntInvokePostAsync()
        {
            TestableUsersClient usersClient = Factory.Client;
            HttpClient client = Factory.CreateClient();
            Dictionary<string, string?> values = new()
            {
                { nameof(UserViewModel.Id), null! },
                { nameof(UserViewModel.FirstName), null! },
                { nameof(UserViewModel.LastName), null! }
            };

            HttpResponseMessage response = await client.PostAsync(new Uri("/users/create/", UriKind.Relative),
                new FormUrlEncodedContent(values));

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(0, usersClient.PostAsyncInvokeCount, "Incorrect number of invocations.");
        }
        #endregion CREATE(USERVIEWMODEL) TESTS


        #region EDIT(ID) TESTS
        [TestMethod]
        public async Task Edit_GivenId_InvokesGetAsyncUsingId()
        {
            TestableUsersClient usersClient = Factory.Client;
            HttpClient client = Factory.CreateClient();
            usersClient.GetAsyncReturnUser = new UserDtoFnLn();

            HttpResponseMessage response = await client.PutAsync(new Uri("/users/edit/0", UriKind.Relative), null!);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(1, usersClient.GetAsyncInvokeCount, "Incorrect number of invocations.");
        }
        #endregion EDIT(ID) TESTS


        #region EDIT(USERVIEWMODEL) TESTS
        [TestMethod]
        public async Task Edit_GivenValidViewModel_InvokesPutAsync()
        {
            TestableUsersClient usersClient = Factory.Client;
            HttpClient client = Factory.CreateClient();
            int id = 0;
            Dictionary<string, string?> values = new() { { nameof(UserViewModel.Id), id + "" }, };

            HttpResponseMessage response = await client.PostAsync(new Uri("/users/edit/" + id, UriKind.Relative),
                new FormUrlEncodedContent(values));

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(1, usersClient.PutAsyncInvokeCount, "Incorrect number of invocations.");
        }

        [TestMethod]
        public async Task Edit_GivenValidViewModel_SendsValidDto()
        {
            TestableUsersClient usersClient = Factory.Client;
            HttpClient client = Factory.CreateClient();
            int id = 0;
            Dictionary<string, string?> values = new()
            {
                { nameof(UserViewModel.Id), id + "" },
                { nameof(UserViewModel.FirstName), "f" },
                { nameof(UserViewModel.LastName), "l" }
            };

            HttpResponseMessage response = await client.PostAsync(new Uri("/users/edit/" + id, UriKind.Relative),
                new FormUrlEncodedContent(values));

            response.EnsureSuccessStatusCode();
            UserDtoFnLn? result = usersClient.PutAsyncParamUser;
            Assert.AreEqual<string>(values!["FirstName"]!, result?.FirstName ?? "");
            Assert.AreEqual<string>(values!["LastName"]!, result?.LastName ?? "");
        }
        #endregion EDIT(USERVIEWMODEL) TESTS


        #region DELETE(ID) TESTS
        [TestMethod]
        public async Task Delete_GivenId_InvokesDeleteAsync()
        {
            TestableUsersClient usersClient = Factory.Client;
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.PostAsync(new Uri("/users/delete/0", UriKind.Relative),
                new FormUrlEncodedContent(new Dictionary<string, string?>()));

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(1, usersClient.DeleteAsyncInvokeCount, "Incorrect number of invocations.");
        }
        #endregion DELETE(ID) TESTS
    }
}
