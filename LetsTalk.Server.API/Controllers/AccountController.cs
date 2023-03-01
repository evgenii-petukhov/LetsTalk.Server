using LetsTalk.Server.API.Attributes;
using LetsTalk.Server.Core.Features.Account.Queries.GetAccount;
using LetsTalk.Server.Core.Features.Account.Queries.GetAccounts;
using LetsTalk.Server.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountDto>>> Get()
        {
            var accountId = (int)HttpContext.Items["AccountId"]!;
            var query = new GetAccountsQuery(accountId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<ActionResult<AccountDto>> GetMe()
        {
            var accountId = (int)HttpContext.Items["AccountId"]!;
            var query = new GetAccountQuery(accountId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
