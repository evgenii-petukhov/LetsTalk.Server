using LetsTalk.Server.API.Attributes;
using LetsTalk.Server.Core.Features.Account.Queries.GetAccounts;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class AccountController : ApiController
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var accountId = GetAccountId();
        var query = new GetAccountsQuery(accountId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
