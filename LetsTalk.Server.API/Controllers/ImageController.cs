using LetsTalk.Server.Core.Features.Profile.Queries.GetProfile;
using LetsTalk.Server.Dto.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> Get(int id)
        {
            return Ok();
        }
    }
}
