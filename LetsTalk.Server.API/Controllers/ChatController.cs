using LetsTalk.Server.Core.Features.Account.Queries.GetContacts;
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
    public async Task<ActionResult<List<AccountDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var accountId = GetAccountId();
        var query = new GetContactsQuery(accountId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
