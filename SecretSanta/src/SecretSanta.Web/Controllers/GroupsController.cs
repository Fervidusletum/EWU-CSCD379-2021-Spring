using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretSanta.Web.ViewModels;
using SecretSanta.Web.Data;

namespace SecretSanta.Web.Controllers
{
    public class GroupsController : Controller
    {
        public IActionResult Index()
        {
            return View(MockData.Groups);
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
                MockData.AddGroup(viewModel);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        public IActionResult Edit(int id) => View(MockData.GetGroup(id)); 

        [HttpPost]
        public IActionResult Edit(GroupViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                MockData.UpdateGroup(viewModel);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            MockData.DeleteGroup(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
