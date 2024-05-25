using LetsTalk.Server.API.Core.Features.Account.Queries.GetAccounts;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
public class AccountController(
    IMediator mediator) : ApiController
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var accountId = GetAccountId();
        var query = new GetAccountsQuery(accountId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
