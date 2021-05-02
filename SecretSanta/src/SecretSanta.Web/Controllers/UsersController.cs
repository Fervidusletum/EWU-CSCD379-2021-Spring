using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using SecretSanta.Web.ViewModels;
using SecretSanta.Web.Api;

namespace SecretSanta.Web.Controllers
{
    public class UsersController : Controller
    {
        public IUsersClient Client { get; }

        public UsersController(IUsersClient client)
            => Client = client ?? throw new ArgumentNullException(nameof(client));

        public async Task<IActionResult> Index()
        {
            ICollection<UserDtoFull> users = await Client.GetAllAsync();
            List<UserViewModel> viewModelUsers = new();
            foreach(UserDtoFull u in users)
            {
                if (u.Id is null) continue;

                viewModelUsers.Add(new UserViewModel
                {
                    Id = (int)u.Id, // casting from nullable int, this entry is skipped if null
                    FirstName = u.FirstName,
                    LastName = u.LastName
                });
            }
            return View(viewModelUsers);
        }

        public IActionResult Create()
            => View();

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await Client.PostAsync(new UserDtoFull
                {
                    Id = viewModel.Id,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName
                });

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            UserDtoFnLn usr = await Client.GetAsync(id);

            return View(new UserViewModel
            {
                Id = id,
                FirstName = usr.FirstName,
                LastName = usr.LastName
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await Client.PutAsync(viewModel.Id, new UserDtoFnLn
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName
                });
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await Client.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
