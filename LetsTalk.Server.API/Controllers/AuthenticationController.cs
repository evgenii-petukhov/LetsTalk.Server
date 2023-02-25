using AutoMapper;
using LetsTalk.Server.API.Models.Authentication;
using LetsTalk.Server.Core.Features.Authentication.Commands;
using LetsTalk.Server.Models.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AuthenticationController(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest model)
    {
        var result = await _mediator.Send(_mapper.Map<LoginCommand>(model));

        Console.WriteLine("Login post");

        return Ok(new LoginResponse());
    }
}
