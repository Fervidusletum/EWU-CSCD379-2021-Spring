using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretSanta.Web.ViewModels;

namespace SecretSanta.Web.Controllers
{
    public class GroupsController : Controller
    {
        // move into dummy data folder later
        public static List<GroupViewModel> Groups = new()
        {
            new GroupViewModel { Name="SMB" }
        };

        public IActionResult Index()
        {
            return View(Groups);
        }
    }
}
