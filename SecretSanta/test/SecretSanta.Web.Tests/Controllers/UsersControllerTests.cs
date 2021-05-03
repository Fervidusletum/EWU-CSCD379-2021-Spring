using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Web.Api;
using SecretSanta.Web.Tests.Api;

namespace SecretSanta.Web.Tests
{
    [TestClass]
    public class UsersControllerTests
    {
        private WebApplicationFactory Factory { get; } = new();

        [TestMethod]
        public async Task Index_WithEvents_InvokesGetAllAsync()
        {
            TestableUsersClient usersClient = Factory.Client;
            usersClient.GetAllAsyncReturnUserList.Add(
                new UserDtoFull { Id = 0, FirstName = "F1", LastName = "L1" });
            usersClient.GetAllAsyncReturnUserList.Add(
                new UserDtoFull { Id = 1, FirstName = "F2", LastName = "L2" });
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync("/users/");

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(1, usersClient.GetAllAsyncInvokeCount);
        }

        [TestMethod]
        public void MakeMoreTests()
        {
            Assert.Fail();
        }
    }
}
