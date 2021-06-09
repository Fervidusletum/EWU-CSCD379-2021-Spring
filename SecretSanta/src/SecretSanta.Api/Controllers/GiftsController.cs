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
    public class GiftsController : ControllerBase
    {
        IGiftRepository Repository { get; }

        public GiftsController(IGiftRepository repository)
        {
            Repository = repository ?? throw new System.ArgumentNullException(nameof(repository));
        }

        [HttpGet("{giftid}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Dto.Gift),(int)HttpStatusCode.OK)]
        public ActionResult<Dto.Gift?> Get(int giftid)
        {
            Data.Gift? gift = Repository.GetItem(giftid);
            if (gift is null) return NotFound();

            return Dto.Gift.ToDto(gift);
        }

        [HttpGet("byuser/{userid}")]
        public IEnumerable<Dto.Gift?> GetByUser(int userid)
            => Repository.List(userid).Select(g => Dto.Gift.ToDto(g));

        [HttpDelete("{giftid}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult Delete(int giftid)
        {
            if (Repository.Remove(giftid))
            {
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Dto.Gift), (int)HttpStatusCode.OK)]
        public ActionResult<Dto.Gift?> Post([FromBody] Dto.Gift gift)
        {

            return Dto.Gift.ToDto(Repository.Create(Dto.Gift.FromDto(gift)!));
        }

        [HttpPut("{giftid}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult Put(int giftid, [FromBody] Dto.UpdateGift? gift)
        {
            if (gift is null) return BadRequest();

            Data.Gift? foundGift = Repository.GetItem(giftid);
            if (foundGift is not null)
            {
                if (gift.Title is not null) foundGift.Title = gift.Title;
                if (gift.Description is not null) foundGift.Description = gift.Description;
                if (gift.Priority is not null) foundGift.Priority = gift.Priority.Value;
                if (gift.Url is not null) foundGift.Url = gift.Url;

                Repository.Save(foundGift);
                return Ok();
            }

            return NotFound();
        }
    }
}
