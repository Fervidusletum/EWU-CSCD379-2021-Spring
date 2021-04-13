using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretSanta.Web.ViewModels;
using SecretSanta.Web.Data;

namespace SecretSanta.Web.Controllers
{
    public class GiftsController : Controller
    {
        public IActionResult Index() => View(MockData.Gifts); 

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(GiftViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                MockData.AddGift(viewModel);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }
        public IActionResult Edit(int id) => View(MockData.GetGift(id));

        [HttpPost]
        public IActionResult Edit(GiftViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                MockData.UpdateGift(viewModel);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            MockData.DeleteGift(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
