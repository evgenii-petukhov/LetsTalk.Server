using LetsTalk.Server.Core.Features.Chat.Queries.GetChats;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
public class ChatController(
    IMediator mediator) : ApiController
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<ChatDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var accountId = GetAccountId();
        var query = new GetChatsQuery(accountId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
