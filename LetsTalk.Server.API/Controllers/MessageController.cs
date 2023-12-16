using AutoMapper;
using LetsTalk.Server.API.Models;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;
using LetsTalk.Server.Core.Features.Message.Queries.GetMessages;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
public class MessageController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly MessagingSettings _messagingSettings;

    public MessageController(
        IMediator mediator,
        IMapper mapper,
        IOptions<MessagingSettings> messagingSettings)
    {
        _mediator = mediator;
        _mapper = mapper;
        _messagingSettings = messagingSettings.Value;
    }

    [HttpGet("{accountId}")]
    public async Task<ActionResult<List<MessageDto>>> GetAsync(string accountId, [FromQuery(Name="page")] int pageIndex = 0, CancellationToken cancellationToken = default)
    {
        var senderId = GetAccountId();
        var query = new GetMessagesQuery(senderId, accountId, pageIndex, _messagingSettings.MessagesPerPage);
        var messageDtos = await _mediator.Send(query, cancellationToken);
        return Ok(messageDtos);
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> PostAsync(CreateMessageRequest request, CancellationToken cancellationToken)
    {
        var cmd = _mapper.Map<CreateMessageCommand>(request);
        cmd.SenderId = GetAccountId();
        var response = await _mediator.Send(cmd, cancellationToken);
        return Ok(response.Dto);
    }

    [HttpPut("MarkAsRead")]
    public async Task<ActionResult> MarkAsReadAsync(string messageId, CancellationToken cancellationToken)
    {
        var cmd = new ReadMessageCommand
        {
            MessageId = messageId,
            RecipientId = GetAccountId()
        };
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }

    [HttpPut("MarkAllAsRead")]
    public async Task<ActionResult> MarkAllAsReadAsync(string messageId, CancellationToken cancellationToken)
    {
        var cmd = new ReadMessageCommand
        {
            MessageId = messageId,
            RecipientId = GetAccountId(),
            UpdatePreviousMessages = true
        };
        await _mediator.Send(cmd, cancellationToken);
        return Ok();
    }
}
