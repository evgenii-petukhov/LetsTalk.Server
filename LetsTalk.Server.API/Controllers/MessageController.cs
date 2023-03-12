using AutoMapper;
using LetsTalk.Server.Abstractions.Authentication;
using LetsTalk.Server.Abstractions.SignalR;
using LetsTalk.Server.API.Attributes;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;
using LetsTalk.Server.Core.Features.Message.Queries.GetMessages;
using LetsTalk.Server.Models.Message;
using LetsTalk.Server.SignalR.Services;
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
        private readonly INotificationService _notificationService;

        public MessageController(
            IMediator mediator,
            IMapper mapper,
            INotificationService notificationService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _notificationService = notificationService;
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
            var message = await _mediator.Send(cmd);

            var messageToNotify = message.GetClone();
            messageToNotify.IsMine = false;
            _ = _notificationService.SendMessageNotification(request.RecipientId, messageToNotify);
            return Ok(message);
        }

        [HttpPut("MarkAsRead")]
        public async Task<ActionResult> MarkAsRead(MarkAsReadRequest request)
        {
            var cmd = _mapper.Map<ReadMessageCommand>(request);
            cmd.RecipientId = (int)HttpContext.Items["AccountId"]!;
            await _mediator.Send(cmd);
            return Ok();
        }
    }
}
