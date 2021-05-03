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


        #region INDEX() TESTS
        [TestMethod]
        public async Task Index_WithUsers_InvokesGetAllAsync()
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
        public void Index_RetreivesValidDtoList_CreatesValidViewModelList()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Index_GivenANullDto_DoesntExplode()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Index_GivenANullIndex_DoesntExplode()
        {
            Assert.Fail();
        }
        #endregion INDEX() TESTS


        #region CREATE(USERVIEWMODEL) TESTS
        [TestMethod]
        public void Create_GivenValidModel_InvokesPostAsync()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Create_GivenValidModel_RedirectsToIndex()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Create_GivenValidModel_ReturnsValidViewModel()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Create_GivenInvalidModel_ReturnsSameViewModel()
        {
            Assert.Fail();
        }
        #endregion CREATE(USERVIEWMODEL) TESTS


        #region EDIT(ID) TESTS
        [TestMethod]
        public void Edit_GivenId_InvokesGetAsyncUsingId()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Edit_GivenId_ReturnsValidViewModel()
        {
            Assert.Fail();
        }
        #endregion EDIT(ID) TESTS


        #region EDIT(USERVIEWMODEL) TESTS
        [TestMethod]
        public void Edit_GivenValidViewModel_InvokesPutAsync()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Edit_GivenValidViewModel_RedirectsToIndex()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Edit_GivenValidViewModel_PutsValidDto()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Edit_GivenInvalidViewModel_ReturnsSameViewModel()
        {
            Assert.Fail();
        }
        #endregion EDIT(USERVIEWMODEL) TESTS


        #region DELETE(ID) TESTS
        [TestMethod]
        public void Delete_GivenId_InvokesDeleteAsync()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Delete_GivenId_RedirectsToIndex()
        {
            Assert.Fail();
        }
        #endregion DELETE(ID) TESTS
    }
}
