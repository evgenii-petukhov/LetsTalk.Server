using AutoMapper;
using LetsTalk.Server.API.Attributes;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Core.Features.Message.Queries.GetMessages;
using LetsTalk.Server.Models.Message;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public MessageController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<MessageDto>>> Get(int recipientId)
        {
            var senderId = (int)HttpContext.Items["AccountId"]!;
            var query = new GetMessagesQuery(senderId, recipientId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> Post(CreateMessageRequest request)
        {
            var cmd = _mapper.Map<CreateMessageCommand>(request);
            cmd.SenderId = (int)HttpContext.Items["AccountId"]!;
            var result = await _mediator.Send(cmd);
            return Ok(result);
        }
    }
}
