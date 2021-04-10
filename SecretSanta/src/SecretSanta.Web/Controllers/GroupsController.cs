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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(GroupViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Groups.Add(viewModel);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }
    }
}
