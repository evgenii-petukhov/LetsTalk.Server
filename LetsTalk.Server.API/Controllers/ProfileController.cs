using AutoMapper;
using LetsTalk.Server.API.Models.Profile;
using LetsTalk.Server.API.Core.Features.Profile.Commands.UpdateProfile;
using LetsTalk.Server.API.Core.Features.Profile.Queries.GetProfile;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LetsTalk.Server.SignPackage.Abstractions;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.API.Validation;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
public class ProfileController(
    IMediator mediator,
    IMapper mapper,
    ISignPackageService signPackageService) : ApiController
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly ISignPackageService _signPackageService = signPackageService;

    [HttpGet]
    public async Task<ActionResult<ProfileDto>> GetProfileAsync(CancellationToken cancellationToken)
    {
        var accountId = GetAccountId();
        var query = new GetProfileQuery(accountId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<ProfileDto>> PutAsync(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProfileRequestValidator(_signPackageService);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var cmd = _mapper.Map<UpdateProfileCommand>(request);
        cmd.AccountId = GetAccountId();
        var accountDto = await _mediator.Send(cmd, cancellationToken);
        return Ok(accountDto);
    }
}
