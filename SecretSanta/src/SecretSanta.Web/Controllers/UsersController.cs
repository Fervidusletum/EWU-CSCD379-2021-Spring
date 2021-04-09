using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretSanta.Web.ViewModels;

namespace SecretSanta.Web.Controllers
{
    public class UsersController : Controller
    {
        // move into dummy data folder later
        public static List<UserViewModel> Users = new()
        {
            new UserViewModel {Id=0, FirstName="Mario", LastName="Mario"},
            new UserViewModel {Id=1, FirstName="Luigi", LastName="Mario"}
        };

        public IActionResult Index()
        {
            return View(Users);
        }
    }
}
