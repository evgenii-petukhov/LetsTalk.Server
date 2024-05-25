using AutoMapper;
using LetsTalk.Server.API.Models.Profile;
using LetsTalk.Server.API.Core.Features.Profile.Commands.UpdateProfile;
using LetsTalk.Server.API.Core.Features.Profile.Queries.GetProfile;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
public class ProfileController(
    IMediator mediator,
    IMapper mapper) : ApiController
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<ProfileDto>> GetProfileAsync(CancellationToken cancellationToken)
    {
        var accountId = GetAccountId();
        var query = new GetProfileQuery(accountId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<ProfileDto>> PutAsync(UpdateProfileRequest model, CancellationToken cancellationToken)
    {
        var cmd = _mapper.Map<UpdateProfileCommand>(model);
        cmd.AccountId = GetAccountId();
        var accountDto = await _mediator.Send(cmd, cancellationToken);
        return Ok(accountDto);
    }
}
