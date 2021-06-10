using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SecretSanta.Business;

namespace SecretSanta.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository Repository { get; }

        public UsersController(IUserRepository repository)
        {
            Repository = repository ?? throw new System.ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        public IEnumerable<Dto.User> Get()
        {
            return Repository.List().Select(x => Dto.User.ToDto(x)!);
        }

        [HttpGet("{id}")]
        public ActionResult<Dto.User?> Get(int id)
        {
            Dto.User? user = Dto.User.ToDto(Repository.GetItem(id));
            if (user is null) return NotFound();
            return user;
        }

        [HttpGet("{userid}/assignments")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Dto.Assignment>),(int)HttpStatusCode.OK)]
        public ActionResult<IEnumerable<Dto.Assignment?>> GetAssignments(int userid)
        {
            Data.User? user = Repository.GetItem(userid);
            if (user is null) return NotFound();
            
            List<Dto.Assignment?> assignments = user.Groups
                .SelectMany(group => group.Assignments, (group, assignment) => assignment)
                .Where(assignment => assignment.GiverId == user.Id)
                .Select(assignment => Dto.Assignment.ToDto(assignment))
                .ToList();

            return assignments;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult Delete(int id)
        {
            if (Repository.Remove(id))
            {
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Dto.User), (int)HttpStatusCode.OK)]
        public ActionResult<Dto.User?> Post([FromBody] Dto.User user)
        {
            return Dto.User.ToDto(Repository.Create(Dto.User.FromDto(user)!));
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult Put(int id, [FromBody] Dto.UpdateUser? user)
        {
            Data.User? foundUser = Repository.GetItem(id);
            if (foundUser is not null)
            {
                foundUser.FirstName = user?.FirstName ?? "";
                foundUser.LastName = user?.LastName ?? "";

                Repository.Save(foundUser);
                return Ok();
            }

            return NotFound();
        }
    }
}
