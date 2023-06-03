using LetsTalk.Server.Core.Features.Image.GetImage;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ImageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImageDto>> GetAsync(int id)
        {
            var cmd = new GetImageQuery(id);
            var response = await _mediator.Send(cmd);

            return Ok(response);
        }
    }
}
