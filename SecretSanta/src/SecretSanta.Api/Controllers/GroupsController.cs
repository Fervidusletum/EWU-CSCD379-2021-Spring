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
    public class GroupsController : ControllerBase
    {
        private IGroupRepository GroupRepository { get; }
        public IUserRepository UserRepository { get; }
        private ILogger Logger { get; }

        public GroupsController(IGroupRepository repository, IUserRepository userRepository, ILogger logger)
        {
            GroupRepository = repository ?? throw new System.ArgumentNullException(nameof(repository));
            UserRepository = userRepository ?? throw new System.ArgumentNullException(nameof(userRepository));
            Logger = logger?.ForContext<GroupsController>() ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IEnumerable<Dto.Group> Get()
        {
            return GroupRepository.List().Select(x => Dto.Group.ToDto(x)!);
        }

        [HttpGet("{id}")]
        public ActionResult<Dto.Group?> Get(int id)
        {
            Dto.Group? group = Dto.Group.ToDto(GroupRepository.GetItem(id), true);
            if (group is null) return NotFound();
            return group;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult Delete(int id)
        {
            if (GroupRepository.Remove(id))
            {
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Dto.Group), (int)HttpStatusCode.OK)]
        public ActionResult<Dto.Group?> Post([FromBody] Dto.Group group)
        {
            return Dto.Group.ToDto(GroupRepository.Create(Dto.Group.FromDto(group)!));
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult Put(int id, [FromBody] Dto.UpdateGroup? group)
        {
            Data.Group? foundGroup = GroupRepository.GetItem(id);
            if (foundGroup is not null)
            {
                foundGroup.Name = group?.Name ?? "";

                GroupRepository.Save(foundGroup);
                return Ok();
            }
            return NotFound();
        }

        [HttpPut("{id}/remove")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult Remove(int id, [FromBody] int userId)
        {
            Data.Group? foundGroup = GroupRepository.GetItem(id);
            if (foundGroup is not null)
            {
                if (foundGroup.Users.FirstOrDefault(x => x.Id == userId) is { } user)
                {
                    foundGroup.Users.Remove(user);
                    GroupRepository.Save(foundGroup);
                }
                return Ok();
            }
            return NotFound();
        }

        [HttpPut("{id}/add")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult Add(int id, [FromBody] int userId)
        {
            Data.Group? foundGroup = GroupRepository.GetItem(id);
            Data.User? foundUser = UserRepository.GetItem(userId);
            if (foundGroup is not null && foundUser is not null)
            {
                if (!foundGroup.Users.Any(x => x.Id == foundUser.Id))
                {
                    foundGroup.Users.Add(foundUser);
                    GroupRepository.Save(foundGroup);
                }
                return Ok();
            }
            return NotFound();
        }

        [HttpPut("{id}/assign")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public ActionResult CreateAssignments(int id)
        {
            AssignmentResult result = GroupRepository.GenerateAssignments(id);
            if (!result.IsSuccess)
            {
                Logger.Information("Create Assignments failed with: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }
    }
}
