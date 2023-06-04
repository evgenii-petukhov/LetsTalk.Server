using AutoMapper;
using LetsTalk.Server.API.Models.UpdateProfile;
using LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;
using LetsTalk.Server.Core.Features.Profile.Queries.GetProfile;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
public class ProfileController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProfileController(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<AccountDto>> GetAsync(CancellationToken cancellationToken)
    {
        var accountId = GetAccountId();
        var query = new GetProfileQuery(accountId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<AccountDto>> PutAsync(UpdateProfileRequest model, CancellationToken cancellationToken)
    {
        var accountId = GetAccountId();
        var cmd = _mapper.Map<UpdateProfileCommand>(model);
        cmd.AccountId = accountId;
        var account = await _mediator.Send(cmd, cancellationToken);
        return Ok(account);
    }
}
