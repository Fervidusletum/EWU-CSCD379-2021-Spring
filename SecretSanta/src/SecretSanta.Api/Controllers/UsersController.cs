using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretSanta.Business;
using SecretSanta.Data;
using SecretSanta.Api.Dto;

namespace SecretSanta.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository Repository { get; }

        public UsersController(IUserRepository repository)
            => Repository = repository ?? throw new System.ArgumentNullException(nameof(repository));

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDtoFull>),StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<UserDtoFull?>> Get()
        {
            List<UserDtoFull?> dtos = new();
            foreach (User u in Repository.List())
            {
                dtos.Add(new UserDtoFull
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                });
            }

            return dtos;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDtoFnLn), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDtoFnLn?> Get(int id)
        {
            User? user = Repository.GetItem(id);

            if (user is null) return NotFound();

            return new UserDtoFnLn
            {
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserDtoFull), StatusCodes.Status200OK)]
        public ActionResult<UserDtoFull?> Post([FromBody] UserDtoFull? user)
        {
            if (user is null || user.Id is null) return BadRequest();

            User newuser = Repository.Create(new User
            {
                Id = (int)user.Id, // casting from nullable int, already checked null above
                FirstName = user.FirstName,
                LastName = user.LastName
            });

            return new UserDtoFull
            {
                Id = newuser.Id,
                FirstName = newuser.FirstName,
                LastName = newuser.LastName
            };
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Put(int id, [FromBody] UserDtoFnLn? user)
        {
            if (user is null) return BadRequest();

            User? foundUser = Repository.GetItem(id);
            if (foundUser is null) return NotFound();

            foundUser.FirstName = user.FirstName ?? "";
            foundUser.LastName = user.LastName ?? "";
            Repository.Save(foundUser);

            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Delete(int id)
        {
            if (Repository.Remove(id)) return Ok();

            return NotFound();
        }
    }
}
