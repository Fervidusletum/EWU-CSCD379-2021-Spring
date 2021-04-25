using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretSanta.Api.Dto;
using SecretSanta.Business;
using SecretSanta.Data;


namespace SecretSanta.Api.Controllers
{
    public class UserController : Controller
    {
        private IUserRepository UserRepository { get; }
        public UserController(IUserRepository userRepo)
            => UserRepository = userRepo ?? throw new ArgumentNullException(nameof(userRepo));

        // /api/users
        [HttpGet]
        public IEnumerable<User> Get() => UserRepository.List();

        // /api/users/<index>
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            User? user = UserRepository.GetItem(id);

            if (user is null) return NotFound();

            return user;
        }


        // /api/users/<index>
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (UserRepository.Remove(id)) return Ok();

            return NotFound();
        }

        // /api/users
        [HttpPost]
        public ActionResult<User?> Post([FromBody] User? newUser)
        {
            if (newUser is null) return BadRequest();

            return UserRepository.Create(newUser);
        }

        // /api/users/<id>
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] DtoUser? updatedUser)
        {
            if (updatedUser is null) return BadRequest();

            User? foundUser = UserRepository.GetItem(id);
            if (foundUser is null) return NotFound();

            if (!string.IsNullOrWhiteSpace(updatedUser.FirstName))
            {
                foundUser.FirstName = updatedUser.FirstName;
            }
            if (!string.IsNullOrWhiteSpace(updatedUser.LastName))
            {
                foundUser.LastName = updatedUser.LastName;
            }

            UserRepository.Save(foundUser);

            return Ok();
        }
    }
}
