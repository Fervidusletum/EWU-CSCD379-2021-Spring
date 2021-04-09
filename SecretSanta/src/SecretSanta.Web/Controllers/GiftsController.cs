using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretSanta.Web.ViewModels;

namespace SecretSanta.Web.Controllers
{
    public class GiftsController : Controller
    {
        // move into dummy data folder later
        public static List<GiftViewModel> Gifts = new()
        {
            new GiftViewModel { Id=0, Title="Star", Description="Grants temporary invincibility.", Priority=0},
            new GiftViewModel { Id=0, Title="Fire Flower", Description="Spits fireballs.", Priority=1, URL="www.somewebsitethatsellsfireflowers.com" }
        };

        public IActionResult Index()
        {
            return View(Gifts);
        }
    }
}
