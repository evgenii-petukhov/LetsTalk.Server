using AutoMapper;
using LetsTalk.Server.API.Models.UpdateProfile;
using LetsTalk.Server.Core.Features.Account.Commands.UpdateProfileCommand;
using LetsTalk.Server.Core.Features.Account.Queries.GetContacts;
using LetsTalk.Server.Core.Features.Account.Queries.GetProfile;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
public class AccountController(
    IMediator mediator,
    IMapper mapper) : ApiController
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> GetContactsAsync(CancellationToken cancellationToken)
    {
        var accountId = GetAccountId();
        var query = new GetContactsQuery(accountId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("Profile")]
    public async Task<ActionResult<AccountDto>> GetProfileAsync(CancellationToken cancellationToken)
    {
        var accountId = GetAccountId();
        var query = new GetProfileQuery(accountId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<AccountDto>> PutAsync(UpdateProfileRequest model, CancellationToken cancellationToken)
    {
        var cmd = _mapper.Map<UpdateProfileCommand>(model);
        cmd.AccountId = GetAccountId();
        var accountDto = await _mediator.Send(cmd, cancellationToken);
        return Ok(accountDto);
    }
}
